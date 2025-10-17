using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Thêm thư viện logging

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
            try
            {
                var res = await _sessionService.GetReportBySessionAsync(sessionId);
                return StatusCode((int)ResponseStatus.Success, APIResponse<ReportResponse>.Success(res, "Report fetched"));
            }
            catch (AppException ex) when (ex.ErrorCode == AppErrorCode.SessionNotFound)
            {
                _logger.LogWarning("Không tìm thấy session khi gọi [GET] /api/Session/{SessionId}/report", sessionId);
                return StatusCode((int)ResponseStatus.NotFound, APIResponse<string>.Fail(AppErrorCode.SessionNotFound, ResponseStatus.NotFound));
            }
            catch (Exception ex)
            {
                // Ghi log lỗi 500 chi tiết
                _logger.LogError(ex, "Lỗi không xác định xảy ra trong [GET] /api/Session/{SessionId}/report", sessionId);
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }
    }
}
