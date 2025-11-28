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
        private readonly PayOS _payOS; 
        private readonly OrderDAO _orderDAO;
        private readonly PaymentDAO _paymentDAO;
        private readonly UserSubscriptionDAO _userSubDAO;
        private readonly ConquerInterviewDbContext _context;
        private readonly UserDAO _userDAO;

        public PaymentRepository(PayOS payOS, OrderDAO orderDAO, PaymentDAO paymentDAO, UserSubscriptionDAO userSubDAO, UserDAO userDAO, ConquerInterviewDbContext context)
        {
            _payOS = payOS;
            _orderDAO = orderDAO;
            _paymentDAO = paymentDAO;
            _userSubDAO = userSubDAO;
            _context = context;
            _userDAO = userDAO;
        }

        public async Task<PaymentLinkResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request)
        {
            var order = await _orderDAO.GetByIdAsync(request.OrderId);
            if (order == null) throw new AppException(AppErrorCode.OrderNotFound);
            if (order.Status == "Completed") throw new Exception("Order already paid");

            var item = new ItemData(order.Plan.PlanName, 1, (int)order.Plan.Price);
            var items = new List<ItemData> { item };

            var paymentData = new PaymentData(
                orderCode: order.OrderId,
                amount: (int)order.TotalAmount,
                description: $"Thanh toán gói Premium {order.OrderId}",
                items: items,
                cancelUrl: request.CancelUrl,
                returnUrl: request.ReturnUrl
            );

            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

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

            int orderId = (int)webhookBody.data.orderCode;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (webhookBody.code == "00")
                {
                    await _paymentDAO.UpdateStatusAsync(orderId, "Paid");
                    var order = await _orderDAO.GetByIdAsync(orderId);

                    if (order != null && order.Status != "Completed")
                    {
                        order.Status = "Completed";
                        var user = await _context.Users.FindAsync(order.UserId);
                        if (user != null)
                        {
                            user.Status = true;
                        }

                        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
                        var durationDays = order.Plan.DurationDays;
                        var endDate = startDate.AddDays(durationDays);

                        var userSub = new UserSubscription
                        {
                            UserId = order.UserId,
                            PlanId = order.PlanId,
                            StartDate = startDate,
                            EndDate = endDate,
                            Status = "Active"
                        };
                        await _userSubDAO.CreateAsync(userSub);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    await _paymentDAO.UpdateStatusAsync(orderId, "Failed");

                    var order = await _orderDAO.GetByIdAsync(orderId);
                    if (order != null && order.Status == "Pending")
                    {
                        order.Status = "Failed";
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _orderDAO.GetByIdAsync(orderId);
            if (order == null) throw new AppException(AppErrorCode.OrderNotFound);

            if (order.Status == "Completed") throw new Exception("Cannot cancel completed order");

            try
            {
                await _payOS.cancelPaymentLink(orderId);
            }
            catch
            {
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _paymentDAO.UpdateStatusAsync(orderId, "Cancelled");

                order.Status = "Cancelled";

                var user = await _context.Users.FindAsync(order.UserId);
                if (user != null)
                {
                    user.Status = false; 
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<PaymentResponse>> GetAllPaymentsAsync()
        {
            var payments = await _paymentDAO.GetAllAsync();
            return payments.Select(p => new PaymentResponse
            {
                PaymentId = p.PaymentId,
                OrderId = p.OrderId,
                Amount = p.Amount,
                Provider = p.Provider,
                Status = p.Status,
                PaidAt = p.PaidAt,
                TransactionId = p.TransactionId
            }).ToList();
        }

        public async Task<List<PaymentResponse>> GetPaymentsByUserIdAsync(int userId)
        {
            var user = await _userDAO.GetByIdAsync(userId);
            if (user == null) throw new AppException(AppErrorCode.UserNotFound);
            var payments = await _paymentDAO.GetByUserIdAsync(userId);
            return payments.Select(p => new PaymentResponse
            {
                PaymentId = p.PaymentId,
                OrderId = p.OrderId,
                Amount = p.Amount,
                Provider = p.Provider,
                Status = p.Status,
                PaidAt = p.PaidAt,
                TransactionId = p.TransactionId
            }).ToList();
        }

    }
}