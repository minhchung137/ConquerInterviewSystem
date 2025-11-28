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

        public OrderService(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            var order = await _orderRepo.CreateOrderAsync(request.UserId, request.PlanId);

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