using ConquerInterviewBO.Common; // Cần cho APIResponse
using ConquerInterviewBO.Commons; // Cần cho AppErrorCode
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConquerInterviewAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonalizationController : ControllerBase
    {
        private readonly IPersonalizationService _service;
        private readonly ILogger<PersonalizationController> _logger;

        public PersonalizationController(IPersonalizationService service, ILogger<PersonalizationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy lộ trình học tập cá nhân hóa cho một user
        /// </summary>
        [HttpGet("user/{customerId}")]
        public async Task<IActionResult> Get(int customerId)
        {
            _logger.LogInformation("[GET] /api/Personalization/user/{CustomerId}", customerId);
            try
            {
                var res = await _service.GetPersonalizationByUserIdAsync(customerId);
                return StatusCode((int)ResponseStatus.Success, APIResponse<List<PersonalizationResponse>>.Success(res, "Success"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong [GET] /api/Personalization/user/{CustomerId}", customerId);
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        /// <summary>
        /// Tạo một lộ trình học tập mới dựa trên các báo cáo
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PersonalizationRequest request)
        {
            _logger.LogInformation("[POST] /api/Personalization cho CustomerId: {CustomerId}", request.CustomerId);
            try
            {
                var res = await _service.CreatePersonalizationAsync(request);
                _logger.LogInformation("Tạo lộ trình thành công cho CustomerId: {CustomerId}", request.CustomerId);
                return StatusCode((int)ResponseStatus.Created, APIResponse<List<PersonalizationResponse>>.Success(res, "Personalization path created", ResponseStatus.Created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong [POST] /api/Personalization");
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        [Authorize]
        [HttpPost("{sessionId}/create-path")]
        public async Task<IActionResult> CreatePersonalizationSessionId(int sessionId)
        {
            // 1. Lấy User ID từ Claims
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int currentUserId))
            {
                _logger.LogWarning("Unauthorized access attempt: User ID not found in token.");
                return Unauthorized(APIResponse<string>.Fail(AppErrorCode.UnauthorizedAccess, ResponseStatus.Unauthorized));
            }

            _logger.LogInformation("[POST] /api/Personalization/{SessionId}/create-path cho CustomerId: {CustomerId}", sessionId, currentUserId);

            try
            {
                // 2. Gọi Service để tạo và lưu lộ trình cá nhân hóa
                var res = await _service.CreatePersonalizationSessionIdAsync(sessionId, currentUserId);

                _logger.LogInformation("Tạo lộ trình cá nhân hóa thành công cho SessionId: {SessionId}", sessionId);

                return StatusCode((int)ResponseStatus.Created,
                    APIResponse<List<PersonalizationResponse>>.Success(res, "Personalization path created", ResponseStatus.Created));
            }
            catch (AppException ex) when (ex.ErrorCode == AppErrorCode.UserNotFound || ex.ErrorCode == AppErrorCode.UserNotFound)
            {
                // Xử lý lỗi quyền truy cập hoặc không tìm thấy dữ liệu (ví dụ: Session không tồn tại)
                _logger.LogWarning("Lỗi nghiệp vụ khi tạo lộ trình: {Message}", ex.Message);
                return StatusCode((int)ResponseStatus.Forbidden,
                    APIResponse<string>.Fail(ex.ErrorCode, ResponseStatus.Forbidden));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong [POST] /api/Personalization/{SessionId}/create-path", sessionId);
                return StatusCode((int)ResponseStatus.InternalServerError,
                    APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        [Authorize(Roles = "CUSTOMER, STAFF, ADMIN")]
        [HttpGet("my-history")]
        public async Task<IActionResult> GetPersonalizationHistory()
        {
            // 1. Lấy User ID từ Claims
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized(APIResponse<string>.Fail(AppErrorCode.UnauthorizedAccess, ResponseStatus.Unauthorized));
            }

            try
            {
                // 2. Gọi Service để lấy lịch sử
                var history = await _service.GetPersonalizationHistoryAsync(currentUserId);

                return StatusCode((int)ResponseStatus.Success,
                    APIResponse<List<PersionalAllResponse>>.Success(history, "Fetched personalization history successfully"));
            }
            catch (AppException ex) when (ex.ErrorCode == AppErrorCode.UserNotFound)
            {
                return NotFound(APIResponse<string>.Fail(AppErrorCode.UserNotFound, ResponseStatus.NotFound));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi lấy lịch sử cá nhân hóa.");
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }
    }
}