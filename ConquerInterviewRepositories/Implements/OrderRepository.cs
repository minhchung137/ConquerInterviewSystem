using ConquerInterviewBO.Common; // Cần cho AppException
using ConquerInterviewBO.Commons; // Cần cho AppErrorCode
using ConquerInterviewBO.Models;
using ConquerInterviewDAO;
using ConquerInterviewRepositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Implements
{
    public class OrderRepository : IOrderRepository
    {
        // (Không cần constructor nếu dùng DAO Singleton)

        public async Task<Order> CreateOrderAsync(int userId, int planId)
        {
            // 1. Kiểm tra User có tồn tại không
            var user = await UserDAO.Instance.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(AppErrorCode.UserNotFound);
            }

            // 2. Kiểm tra Plan có tồn tại và active không
            var plan = await SubscriptionPlanDAO.Instance.GetByIdAsync(planId);
            if (plan == null)
            {
                throw new AppException(AppErrorCode.SubscriptionPlanNotFound);
            }
            if (plan.IsActive != true)
            {
                throw new AppException(AppErrorCode.PlanIsInactive);
            }

            // 3. Tạo Order mới
            var newOrder = new Order
            {
                UserId = userId,
                PlanId = planId,
                TotalAmount = plan.Price, // Lấy giá từ Plan
                Status = "Pending",      // Trạng thái chờ thanh toán
                CreatedAt = DateTime.UtcNow
            };

            // 4. Lưu vào DB
            return await OrderDAO.Instance.CreateAsync(newOrder);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await OrderDAO.Instance.GetByIdAsync(orderId);
        }
    }
}