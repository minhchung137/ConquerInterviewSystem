using ConquerInterviewBO.DTOs.Responses;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Interfaces
{
    public interface IUserSubscriptionService
    {

        Task<List<UserSubscriptionResponse>> GetAllSubscriptionsAsync();
        Task<List<UserSubscriptionResponse>> GetSubscriptionsByUserIdAsync(int userId);
        Task<RevenueResponse> GetRevenueReportAsync();
    }
}