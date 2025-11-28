using ConquerInterviewBO.Models;
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
    }
}