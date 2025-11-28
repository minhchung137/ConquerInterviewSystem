using ConquerInterviewBO.Common; 
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.Models;
using ConquerInterviewDAO;
using ConquerInterviewRepositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Implements
{
    public class OrderRepository : IOrderRepository
    {

        public async Task<Order> CreateOrderAsync(int userId, int planId)
        {
            var user = await UserDAO.Instance.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(AppErrorCode.UserNotFound);
            }

            var plan = await SubscriptionPlanDAO.Instance.GetByIdAsync(planId);
            if (plan == null)
            {
                throw new AppException(AppErrorCode.SubscriptionPlanNotFound);
            }
            if (plan.IsActive != true)
            {
                throw new AppException(AppErrorCode.PlanIsInactive);
            }
            var newOrder = new Order
            {
                UserId = userId,
                PlanId = planId,
                TotalAmount = plan.Price,
                Status = "Pending",    
                CreatedAt = DateTime.UtcNow
            };

            return await OrderDAO.Instance.CreateAsync(newOrder);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await OrderDAO.Instance.GetByIdAsync(orderId);
        }
    }
}