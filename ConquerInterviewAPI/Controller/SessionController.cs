using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims; // Thêm thư viện logging

namespace ConquerInterviewAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly ILogger<SessionController> _logger; // Khai báo logger

        public SessionController(ISessionService sessionService, ILogger<SessionController> logger) // Inject ILogger
        {
            _sessionService = sessionService;
            _logger = logger; // Gán logger
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartSessionRequest request)
        {
            _logger.LogInformation("Nhận được request tại endpoint [POST] /api/Session/start cho User ID: {UserId}", request.UserId);
            try
            {
                var res = await _sessionService.StartSessionAsync(request);
                _logger.LogInformation("Xử lý thành công [POST] /api/Session/start cho Session ID: {SessionId}", res.SessionId);
                return StatusCode((int)ResponseStatus.Created, APIResponse<StartSessionResponse>.Success(res, "Session started", ResponseStatus.Created));
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "Lỗi nghiệp vụ trong Start: ErrorCode={ErrorCode}", ex.ErrorCode);
                return StatusCode((int)ResponseStatus.BadRequest, APIResponse<string>.Fail(ex.ErrorCode, ResponseStatus.BadRequest));
            }
            catch (Exception ex)
            {
                // ĐÂY LÀ PHẦN QUAN TRỌNG NHẤT
                _logger.LogError(ex, "Lỗi không xác định xảy ra trong [POST] /api/Session/start.");
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        [HttpGet("{sessionId}/questions")]
        public async Task<IActionResult> GetQuestions(int sessionId)
        {
            _logger.LogInformation("Nhận được request tại endpoint [GET] /api/Session/{SessionId}/questions", sessionId);
            try
            {
                var res = await _sessionService.GetQuestionsBySessionAsync(sessionId);
                return StatusCode((int)ResponseStatus.Success, APIResponse<List<QuestionResponse>>.Success(res, "Success"));
            }
            catch (AppException ex) when (ex.ErrorCode == AppErrorCode.SessionNotFound)
            {
                _logger.LogWarning("Không tìm thấy session khi gọi [GET] /api/Session/{SessionId}/questions", sessionId);
                return StatusCode((int)ResponseStatus.NotFound, APIResponse<string>.Fail(AppErrorCode.SessionNotFound, ResponseStatus.NotFound));
            }
            catch (AppException ex)
            {
                _logger.LogWarning("Truy cập bị từ chối khi gọi [GET] /api/Session/{SessionId}/questions. ErrorCode: {ErrorCode}", sessionId, ex.ErrorCode);
                return StatusCode((int)ResponseStatus.Forbidden, APIResponse<string>.Fail(ex.ErrorCode, ResponseStatus.Forbidden));
            }
        }

        [HttpPost("answer")]
        public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerRequest request)
        {
            _logger.LogInformation("Nhận được request tại endpoint [POST] /api/Session/answer cho Session ID: {SessionId}", request.SessionId);
            try
            {
                var res = await _sessionService.SubmitAnswerAsync(request);
                return StatusCode((int)ResponseStatus.Success, APIResponse<AnswerReportResponse>.Success(res, "Answer submitted"));
            }
            catch (AppException ex) when (ex.ErrorCode == AppErrorCode.SessionNotFound)
            {
                _logger.LogWarning("Không tìm thấy session khi gọi [POST] /api/Session/answer. Session ID: {SessionId}", request.SessionId);
                return StatusCode((int)ResponseStatus.NotFound, APIResponse<string>.Fail(AppErrorCode.SessionNotFound, ResponseStatus.NotFound));
            }
            catch (AppException ex)
            {
                _logger.LogWarning("Truy cập bị từ chối khi gọi [POST] /api/Session/answer. Session ID: {SessionId}. ErrorCode: {ErrorCode}", request.SessionId, ex.ErrorCode);
                return StatusCode((int)ResponseStatus.Forbidden, APIResponse<string>.Fail(ex.ErrorCode, ResponseStatus.Forbidden));
            }
            catch (Exception ex)
            {
                // Ghi log lỗi 500 chi tiết
                _logger.LogError(ex, "Lỗi không xác định xảy ra trong [POST] /api/Session/answer. Session ID: {SessionId}", request.SessionId);
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        [HttpGet("{sessionId}/report")]
        public async Task<IActionResult> GetReport(int sessionId)
        {
            _logger.LogInformation("Nhận được request tại endpoint [GET] /api/Session/{SessionId}/report", sessionId);

            // 1. LẤY USER ID TỪ CLAIMS
            // Giả định: UserId được lưu dưới dạng ClaimTypes.NameIdentifier
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int currentUserId))
            {
                // Xử lý nếu UserId không hợp lệ hoặc không có trong token (không được xác thực)
                _logger.LogWarning("Không thể tìm thấy hoặc parse UserId từ token. Access Denied.");
                // Giả sử Unauthorized là 401 hoặc Forbidden 403 tùy thuộc vào logic bảo mật của bạn
                return StatusCode((int)ResponseStatus.Forbidden, APIResponse<string>.Fail(AppErrorCode.UnauthorizedAccess, ResponseStatus.Forbidden));
            }

            try
            {
                // 2. TRUYỀN CẢ SESSION ID VÀ USER ID VÀO SERVICE
                // Bạn cần thay đổi chữ ký hàm trong Service để chấp nhận currentUserId
                var res = await _sessionService.GetReportBySessionAsync(sessionId, currentUserId);

                return StatusCode((int)ResponseStatus.Success, APIResponse<ReportResponse>.Success(res, "Report fetched"));
            }
            // 3. Xử lý lỗi riêng tư/quyền sở hữu
            catch (AppException ex) when (ex.ErrorCode == AppErrorCode.UnauthorizedAccess)
            {
                _logger.LogWarning("UserId {CurrentUserId} không có quyền truy cập báo cáo Session {SessionId}.", currentUserId, sessionId);
                return StatusCode((int)ResponseStatus.Forbidden, APIResponse<string>.Fail(AppErrorCode.UnauthorizedAccess, ResponseStatus.Forbidden));
            }
            catch (AppException ex) when (ex.ErrorCode == AppErrorCode.SessionNotFound)
            {
                _logger.LogWarning("Không tìm thấy session khi gọi [GET] /api/Session/{SessionId}/report", sessionId);
                return StatusCode((int)ResponseStatus.NotFound, APIResponse<string>.Fail(AppErrorCode.SessionNotFound, ResponseStatus.NotFound));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định xảy ra trong [GET] /api/Session/{SessionId}/report", sessionId);
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus(int sessionId, string status)
        {
            try
            {
                await _sessionService.UpdateStatusAsync(sessionId, status);
                return Ok(new { message = "Update status thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi update status", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("report")]
        public async Task<IActionResult> GetUserReportsHistory()
        {
            // 1. Lấy User ID từ Claims (người dùng hiện tại)
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized(APIResponse<string>.Fail(AppErrorCode.UnauthorizedAccess, ResponseStatus.Unauthorized));
            }

            _logger.LogInformation("Nhận request lấy lịch sử báo cáo cho User ID: {UserId}", currentUserId);

            try
            {
                // 2. Gọi Service để lấy tất cả báo cáo theo User ID
                var reportsHistory = await _sessionService.GetReportsByUserIdAsync(currentUserId);

                if (reportsHistory == null || reportsHistory.Count == 0)
                {
                    return StatusCode((int)ResponseStatus.NotFound, APIResponse<string>.Fail(AppErrorCode.SessionNotFound, ResponseStatus.NotFound));
                }

                return StatusCode((int)ResponseStatus.Success,
                    APIResponse<List<ReportResponse>>.Success(reportsHistory, "Fetched user reports history successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi lấy lịch sử báo cáo.");
                return StatusCode((int)ResponseStatus.InternalServerError,
                    APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllSessions()
        {
            try
            {
                var result = await _sessionService.GetAllSessionsAsync();
                return StatusCode((int)ResponseStatus.Success,
                    APIResponse<List<SessionResponse>>.Success(result, "Get all sessions success"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all sessions");
                return StatusCode((int)ResponseStatus.InternalServerError,
                    APIResponse<string>.Fail(AppErrorCode.InternalError));
            }
        }
    }
}
