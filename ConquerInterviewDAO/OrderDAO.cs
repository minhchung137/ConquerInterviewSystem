using ConquerInterviewBO.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ConquerInterviewDAO
{
    public class OrderDAO
    {
        private readonly ConquerInterviewDbContext _context;
        private static OrderDAO instance;

        public static OrderDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new OrderDAO();
                }
                return instance;
            }
        }

        public OrderDAO()
        {
            _context = new ConquerInterviewDbContext();
        }

        // Create
        public async Task<Order> CreateAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        // Read (By ID)
        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Plan) // Tải thông tin Plan
                .Include(o => o.User) // Tải thông tin User
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}