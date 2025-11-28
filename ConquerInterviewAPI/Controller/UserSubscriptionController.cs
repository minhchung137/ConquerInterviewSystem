using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConquerInterviewAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserSubscriptionController : ControllerBase
    {
        private readonly IUserSubscriptionService _service;
        private readonly ILogger<UserSubscriptionController> _logger;

        public UserSubscriptionController(IUserSubscriptionService service, ILogger<UserSubscriptionController> logger)
        {
            _service = service;
            _logger = logger;
        }

       
        [HttpGet("subscription/all")]
        public async Task<IActionResult> GetAllSubscriptions()
        {
            try
            {
                var result = await _service.GetAllSubscriptionsAsync();
                return StatusCode((int)ResponseStatus.Success, APIResponse<List<UserSubscriptionResponse>>.Success(result, "Get all subscriptions success"));
            }
            catch (Exception ex)
            {
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError));
            }
        }

        [HttpGet("subscription/user/{userId}")]
        public async Task<IActionResult> GetSubscriptionsByUser(int userId)
        {
            try
            {
                var result = await _service.GetSubscriptionsByUserIdAsync(userId);
                return StatusCode((int)ResponseStatus.Success, APIResponse<List<UserSubscriptionResponse>>.Success(result, "Get user subscriptions success"));
            }
            catch (Exception ex)
            {
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError));
            }
        }
        [HttpGet("revenue-report")]
        public async Task<IActionResult> GetRevenueReport()
        {
            try
            {
                var result = await _service.GetRevenueReportAsync();
                return StatusCode((int)ResponseStatus.Success,
                    APIResponse<RevenueResponse>.Success(result, "Get revenue report success"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue report");
                return StatusCode((int)ResponseStatus.InternalServerError,
                    APIResponse<string>.Fail(AppErrorCode.InternalError));
            }
        }
    }
}