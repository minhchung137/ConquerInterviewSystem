using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewRepositories.Interfaces;
using ConquerInterviewServices.Interfaces;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Implements
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IUserSubscriptionRepository _repo;

        public UserSubscriptionService(IUserSubscriptionRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<UserSubscriptionResponse>> GetAllSubscriptionsAsync()
        {
            return await _repo.GetAllSubscriptionsAsync();
        }

        public async Task<List<UserSubscriptionResponse>> GetSubscriptionsByUserIdAsync(int userId)
        {
            return await _repo.GetSubscriptionsByUserIdAsync(userId);
        }
        public async Task<RevenueResponse> GetRevenueReportAsync()
        {
            return await _repo.GetRevenueReportAsync();
        }
    }
}