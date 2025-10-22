using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConquerInterviewAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionPlanController : ControllerBase
    {
        private readonly ISubscriptionPlanService _service;
        private readonly ILogger<SubscriptionPlanController> _logger;

        public SubscriptionPlanController(ISubscriptionPlanService service, ILogger<SubscriptionPlanController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả các gói đăng ký
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllPlans()
        {
            _logger.LogInformation("[GET] /api/SubscriptionPlan");
            try
            {
                var plans = await _service.GetAllPlansAsync();
                return StatusCode((int)ResponseStatus.Success, APIResponse<List<PlanResponse>>.Success(plans, "Success"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong [GET] /api/SubscriptionPlan");
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        /// <summary>
        /// Lấy một gói đăng ký theo ID
        /// </summary>
        [HttpGet("{planId}")]
        public async Task<IActionResult> GetPlanById(int planId)
        {
            _logger.LogInformation("[GET] /api/SubscriptionPlan/{PlanId}", planId);
            try
            {
                var plan = await _service.GetPlanByIdAsync(planId);
                if (plan == null)
                {
                    _logger.LogWarning("Không tìm thấy Plan ID: {PlanId}", planId);
                    return StatusCode((int)ResponseStatus.NotFound, APIResponse<string>.Fail(AppErrorCode.SubscriptionPlanNotFound, ResponseStatus.NotFound));
                }
                return StatusCode((int)ResponseStatus.Success, APIResponse<PlanResponse>.Success(plan, "Success"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong [GET] /api/SubscriptionPlan/{PlanId}", planId);
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        /// <summary>
        /// Tạo một gói đăng ký mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePlan([FromBody] CreatePlanRequest request)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode((int)ResponseStatus.BadRequest, APIResponse<string>.Fail(AppErrorCode.InvalidInput, ResponseStatus.BadRequest));
            }

            _logger.LogInformation("[POST] /api/SubscriptionPlan - PlanName: {PlanName}", request.PlanName);
            try
            {
                var newPlan = await _service.CreatePlanAsync(request);
                return StatusCode((int)ResponseStatus.Created, APIResponse<PlanResponse>.Success(newPlan, "Plan created", ResponseStatus.Created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong [POST] /api/SubscriptionPlan");
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        /// <summary>
        /// Cập nhật một gói đăng ký
        /// </summary>
        [HttpPut("{planId}")]
        public async Task<IActionResult> UpdatePlan(int planId, [FromBody] UpdatePlanRequest request)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode((int)ResponseStatus.BadRequest, APIResponse<string>.Fail(AppErrorCode.InvalidInput, ResponseStatus.BadRequest));
            }

            _logger.LogInformation("[PUT] /api/SubscriptionPlan/{PlanId}", planId);
            try
            {
                var updatedPlan = await _service.UpdatePlanAsync(planId, request);
                if (updatedPlan == null)
                {
                    _logger.LogWarning("Không tìm thấy Plan ID: {PlanId} để cập nhật", planId);
                    return StatusCode((int)ResponseStatus.NotFound, APIResponse<string>.Fail(AppErrorCode.SubscriptionPlanNotFound, ResponseStatus.NotFound));
                }
                return StatusCode((int)ResponseStatus.Success, APIResponse<PlanResponse>.Success(updatedPlan, "Plan updated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong [PUT] /api/SubscriptionPlan/{PlanId}", planId);
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        /// <summary>
        /// Xóa một gói đăng ký
        /// </summary>
        [HttpDelete("{planId}")]
        public async Task<IActionResult> DeletePlan(int planId)
        {
            _logger.LogInformation("[DELETE] /api/SubscriptionPlan/{PlanId}", planId);
            try
            {
                var success = await _service.DeletePlanAsync(planId);
                if (!success)
                {
                    _logger.LogWarning("Không tìm thấy Plan ID: {PlanId} để xóa", planId);
                    return StatusCode((int)ResponseStatus.NotFound, APIResponse<string>.Fail(AppErrorCode.SubscriptionPlanNotFound, ResponseStatus.NotFound));
                }
                return StatusCode((int)ResponseStatus.Success, APIResponse<string>.Success(null, "Plan deleted"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong [DELETE] /api/SubscriptionPlan/{PlanId}", planId);
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }
    }
}