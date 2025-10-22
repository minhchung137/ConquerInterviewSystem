using ConquerInterviewBO.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Interfaces
{
    public interface ISubscriptionPlanRepository
    {
        Task<SubscriptionPlan> CreateAsync(SubscriptionPlan plan);
        Task UpdateAsync(SubscriptionPlan plan);
        Task<bool> DeleteAsync(int planId);
        Task<SubscriptionPlan?> GetByIdAsync(int planId);
        Task<List<SubscriptionPlan>> GetAllAsync();
    }
}