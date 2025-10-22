using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewBO.Models;
using ConquerInterviewRepositories.Interfaces;
using ConquerInterviewServices.Interfaces;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;

        // Giả sử bạn đã tiêm (Inject) Repository
        public OrderService(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            // 1. Gọi Repo để tạo Order
            // Repo sẽ lo việc validate UserId và PlanId
            var order = await _orderRepo.CreateOrderAsync(request.UserId, request.PlanId);

            // 2. Map sang Response DTO
            return MapToResponse(order);
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return null;
            }
            return MapToResponse(order);
        }

        // Hàm helper để map Model sang Response DTO
        private OrderResponse MapToResponse(Order order)
        {
            return new OrderResponse
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                PlanId = order.PlanId,
                TotalAmount = order.TotalAmount,
                Status = order.Status ?? "Unknown",
                CreatedAt = order.CreatedAt ?? DateTime.MinValue
            };
        }
    }
}