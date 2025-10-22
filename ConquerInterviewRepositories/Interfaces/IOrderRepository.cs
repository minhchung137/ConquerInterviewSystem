using ConquerInterviewBO.Models;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(int userId, int planId);
        Task<Order?> GetOrderByIdAsync(int orderId);
    }
}