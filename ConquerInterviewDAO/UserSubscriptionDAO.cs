using ConquerInterviewBO.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ConquerInterviewDAO
{
    public class UserSubscriptionDAO
    {
        private readonly ConquerInterviewDbContext _context;

        public UserSubscriptionDAO(ConquerInterviewDbContext context)
        {
            _context = context;
        }

        public async Task<UserSubscription> CreateAsync(UserSubscription sub)
        {
            await _context.UserSubscriptions.AddAsync(sub);
            await _context.SaveChangesAsync();
            return sub;
        }
        public async Task<List<UserSubscription>> GetAllAsync()
        {
            return await _context.UserSubscriptions
                .Include(s => s.Plan)
                .OrderByDescending(s => s.SubscriptionId)
                .ToListAsync();
        }

        public async Task<List<UserSubscription>> GetByUserIdAsync(int userId)
        {
            return await _context.UserSubscriptions
                .Include(s => s.Plan)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SubscriptionId)
                .ToListAsync();
        }
        public async Task<List<UserSubscription>> GetAllWithDetailsAsync()
        {
            return await _context.UserSubscriptions
                .Include(s => s.User) 
                .Include(s => s.Plan) 
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();
        }
    }
}