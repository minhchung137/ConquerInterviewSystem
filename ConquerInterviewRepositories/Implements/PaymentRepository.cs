using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewBO.Models;
using ConquerInterviewDAO;
using ConquerInterviewRepositories.Interfaces;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Implements
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PayOS _payOS; // Class PayOS nằm trong Net.PayOS
        private readonly OrderDAO _orderDAO;
        private readonly PaymentDAO _paymentDAO;
        private readonly UserSubscriptionDAO _userSubDAO;
        private readonly ConquerInterviewDbContext _context;

        public PaymentRepository(PayOS payOS, OrderDAO orderDAO, PaymentDAO paymentDAO, UserSubscriptionDAO userSubDAO, ConquerInterviewDbContext context)
        {
            _payOS = payOS;
            _orderDAO = orderDAO;
            _paymentDAO = paymentDAO;
            _userSubDAO = userSubDAO;
            _context = context;
        }

        public async Task<PaymentLinkResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request)
        {
            var order = await _orderDAO.GetByIdAsync(request.OrderId);
            if (order == null) throw new AppException(AppErrorCode.OrderNotFound);
            if (order.Status == "Completed") throw new Exception("Order already paid");

            // 1. Tạo ItemData (Tên, Số lượng, Giá)
            var item = new ItemData(order.Plan.PlanName, 1, (int)order.Plan.Price);
            var items = new List<ItemData> { item };

            // 2. Tạo PaymentData
            var paymentData = new PaymentData(
                orderCode: order.OrderId,
                amount: (int)order.TotalAmount,
                description: $"Thanh toan don {order.OrderId}",
                items: items,
                cancelUrl: request.CancelUrl,
                returnUrl: request.ReturnUrl
            );

            // 3. Gọi PayOS tạo link (bản 1.0.6 hàm này là chữ thường)
            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

            // 4. Lưu DB
            var payment = new Payment
            {
                OrderId = order.OrderId,
                Provider = "PayOS",
                TransactionId = createPayment.paymentLinkId,
                Amount = order.TotalAmount,
                Status = "Pending",
                PaidAt = null
            };
            await _paymentDAO.CreateAsync(payment);

            return new PaymentLinkResponse
            {
                OrderId = order.OrderId,
                PaymentUrl = createPayment.checkoutUrl,
                QrCode = createPayment.qrCode
            };
        }

        public async Task ProcessWebhookAsync(WebhookType webhookBody)
        {
            try
            {
                _payOS.verifyPaymentWebhookData(webhookBody);
            }
            catch (Exception)
            {
                throw new Exception("Invalid Webhook Signature");
            }

            if (webhookBody.code == "00")
            {
                int orderId = (int)webhookBody.data.orderCode;

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await _paymentDAO.UpdateStatusAsync(orderId, "Paid");

                    var order = await _orderDAO.GetByIdAsync(orderId);
                    if (order != null && order.Status != "Completed")
                    {
                        order.Status = "Completed";
                        await _context.SaveChangesAsync();

                        // Logic kích hoạt gói
                        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
                        var endDate = startDate.AddDays(order.Plan.DurationDays);

                        var userSub = new UserSubscription
                        {
                            UserId = order.UserId,
                            PlanId = order.PlanId,
                            StartDate = startDate,
                            EndDate = endDate,
                            Status = "Active"
                        };
                        await _userSubDAO.CreateAsync(userSub);
                    }
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}