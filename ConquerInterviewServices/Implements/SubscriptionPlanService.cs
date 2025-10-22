using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewBO.Models;
using ConquerInterviewRepositories.Interfaces;
using ConquerInterviewServices.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Implements
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly ISubscriptionPlanRepository _repo;

        // Giả sử bạn đã tiêm (Inject) Repository
        public SubscriptionPlanService(ISubscriptionPlanRepository repo)
        {
            _repo = repo;
        }

        public async Task<PlanResponse> CreatePlanAsync(CreatePlanRequest request)
        {
            // 1. Map từ Request DTO sang Model
            var plan = new SubscriptionPlan
            {
                PlanName = request.PlanName,
                Price = request.Price,
                DurationDays = request.DurationDays,
                IsActive = true // Mặc định là active khi mới tạo
            };

            // 2. Gọi Repo để tạo
            var createdPlan = await _repo.CreateAsync(plan);

            // 3. Map từ Model sang Response DTO
            return MapToResponse(createdPlan);
        }

        public async Task<bool> DeletePlanAsync(int planId)
        {
            return await _repo.DeleteAsync(planId);
        }

        public async Task<List<PlanResponse>> GetAllPlansAsync()
        {
            var plans = await _repo.GetAllAsync();

            // Map danh sách Model sang danh sách Response DTO
            return plans.Select(MapToResponse).ToList();
        }

        public async Task<PlanResponse?> GetPlanByIdAsync(int planId)
        {
            var plan = await _repo.GetByIdAsync(planId);
            if (plan == null)
            {
                return null;
            }
            return MapToResponse(plan);
        }

        public async Task<PlanResponse?> UpdatePlanAsync(int planId, UpdatePlanRequest request)
        {
            // 1. Tìm plan hiện có
            var existingPlan = await _repo.GetByIdAsync(planId);
            if (existingPlan == null)
            {
                return null; // Không tìm thấy
            }

            // 2. Cập nhật thuộc tính
            existingPlan.PlanName = request.PlanName;
            existingPlan.Price = request.Price;
            existingPlan.DurationDays = request.DurationDays;
            existingPlan.IsActive = request.IsActive;

            // 3. Gọi Repo để lưu thay đổi
            await _repo.UpdateAsync(existingPlan);

            // 4. Trả về DTO
            return MapToResponse(existingPlan);
        }

        // Hàm helper để map Model sang Response DTO
        private PlanResponse MapToResponse(SubscriptionPlan plan)
        {
            return new PlanResponse
            {
                PlanId = plan.PlanId,
                PlanName = plan.PlanName,
                Price = plan.Price,
                DurationDays = plan.DurationDays,
                IsActive = plan.IsActive ?? false // Chuyển đổi bool? sang bool
            };
        }
    }
}