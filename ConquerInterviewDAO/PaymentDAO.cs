using ConquerInterviewBO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ConquerInterviewDAO
{
    public class PaymentDAO
    {
        private readonly ConquerInterviewDbContext _context;

        public PaymentDAO(ConquerInterviewDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task UpdateStatusAsync(int orderId, string status)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
            if (payment != null)
            {
                payment.Status = status;
                if (status == "Paid") payment.PaidAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}