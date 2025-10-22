using ConquerInterviewBO.Models;
using ConquerInterviewDAO;
using ConquerInterviewRepositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Implements
{
    public class SubscriptionPlanRepository : ISubscriptionPlanRepository
    {
        public async Task<SubscriptionPlan> CreateAsync(SubscriptionPlan plan)
        {
            return await SubscriptionPlanDAO.Instance.CreateAsync(plan);
        }

        public async Task<bool> DeleteAsync(int planId)
        {
            var plan = await SubscriptionPlanDAO.Instance.GetByIdAsync(planId);
            if (plan == null)
            {
                return false; // Không tìm thấy để xóa
            }
            await SubscriptionPlanDAO.Instance.DeleteAsync(plan);
            return true;
        }

        public async Task<List<SubscriptionPlan>> GetAllAsync()
        {
            return await SubscriptionPlanDAO.Instance.GetAllAsync();
        }

        public async Task<SubscriptionPlan?> GetByIdAsync(int planId)
        {
            return await SubscriptionPlanDAO.Instance.GetByIdAsync(planId);
        }

        public async Task UpdateAsync(SubscriptionPlan plan)
        {
            await SubscriptionPlanDAO.Instance.UpdateAsync(plan);
        }
    }
}