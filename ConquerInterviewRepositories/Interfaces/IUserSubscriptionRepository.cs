using ConquerInterviewBO.DTOs.Responses;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Interfaces
{
    public interface IUserSubscriptionRepository
    {

        Task<List<UserSubscriptionResponse>> GetAllSubscriptionsAsync();
        Task<List<UserSubscriptionResponse>> GetSubscriptionsByUserIdAsync(int userId);
        Task<RevenueResponse> GetRevenueReportAsync();
    }
}