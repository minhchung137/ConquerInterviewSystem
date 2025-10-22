using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Interfaces
{
    public interface ISubscriptionPlanService
    {
        Task<List<PlanResponse>> GetAllPlansAsync();
        Task<PlanResponse?> GetPlanByIdAsync(int planId);
        Task<PlanResponse> CreatePlanAsync(CreatePlanRequest request);
        Task<PlanResponse?> UpdatePlanAsync(int planId, UpdatePlanRequest request);
        Task<bool> DeletePlanAsync(int planId);
    }
}