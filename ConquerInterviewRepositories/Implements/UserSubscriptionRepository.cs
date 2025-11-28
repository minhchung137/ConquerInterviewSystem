using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewDAO;
using ConquerInterviewRepositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Implements
{
    public class UserSubscriptionRepository : IUserSubscriptionRepository
    {
        private readonly UserSubscriptionDAO _dao;
        private readonly UserDAO _userDAO;

        public UserSubscriptionRepository(UserSubscriptionDAO dao, UserDAO userDAO)
        {
            _dao = dao;
            _userDAO = userDAO;
        }

        public async Task<List<UserSubscriptionResponse>> GetAllSubscriptionsAsync()
        {
            var subs = await _dao.GetAllAsync();
            return subs.Select(s => new UserSubscriptionResponse
            {
                SubscriptionId = s.SubscriptionId,
                UserId = s.UserId,
                PlanName = s.Plan.PlanName,
                Price = s.Plan.Price,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Status = s.Status
            }).ToList();
        }

        public async Task<List<UserSubscriptionResponse>> GetSubscriptionsByUserIdAsync(int userId)
        {
            var user = await _userDAO.GetByIdAsync(userId);
            if (user == null) throw new AppException(AppErrorCode.UserNotFound);
            var subs = await _dao.GetByUserIdAsync(userId);
            return subs.Select(s => new UserSubscriptionResponse
            {
                SubscriptionId = s.SubscriptionId,
                UserId = s.UserId,
                PlanName = s.Plan.PlanName,
                Price = s.Plan.Price,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Status = s.Status
            }).ToList();
        }
        public async Task<RevenueResponse> GetRevenueReportAsync()
        {
            var subs = await _dao.GetAllWithDetailsAsync();

            var details = subs.Select(s => new SubscriptionDetailResponse
            {
                SubscriptionId = s.SubscriptionId,
                UserEmail = s.User.Email,      
                PlanName = s.Plan.PlanName,    
                Price = s.Plan.Price,
                StartDate = s.StartDate,
                Status = s.Status
            }).ToList();

            var totalRevenue = subs
                .Where(s => s.Status == "Active" || s.Status == "Expired")
                .Sum(s => s.Plan.Price);

            return new RevenueResponse
            {
                TotalRevenue = totalRevenue,
                TotalSubscriptions = details.Count,
                Details = details
            };
        }
    }
}