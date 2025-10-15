using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConquerInterviewAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartSessionRequest request)
        {
            try
            {
                var res = await _sessionService.StartSessionAsync(request);
                return StatusCode((int)ResponseStatus.Created, APIResponse<StartSessionResponse>.Success(res, "Session started", ResponseStatus.Created));
            }
            catch (AppException ex)
            {
                return StatusCode((int)ResponseStatus.BadRequest, APIResponse<string>.Fail(ex.ErrorCode, ResponseStatus.BadRequest));
            }
            catch (Exception)
            {
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        [HttpGet("{sessionId}/questions")]
        public async Task<IActionResult> GetQuestions(int sessionId)
        {
            try
            {
                var res = await _sessionService.GetQuestionsBySessionAsync(sessionId);
                return StatusCode((int)ResponseStatus.Success, APIResponse<List<QuestionResponse>>.Success(res, "Success"));
            }
            catch (AppException ex) when (ex.ErrorCode == AppErrorCode.SessionNotFound)
            {
                return StatusCode((int)ResponseStatus.NotFound, APIResponse<string>.Fail(AppErrorCode.SessionNotFound, ResponseStatus.NotFound));
            }
            catch (AppException ex)
            {
                return StatusCode((int)ResponseStatus.Forbidden, APIResponse<string>.Fail(ex.ErrorCode, ResponseStatus.Forbidden));
            }
        }

        [HttpPost("answer")]
        public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerRequest request)
        {
            try
            {
                var res = await _sessionService.SubmitAnswerAsync(request);
                return StatusCode((int)ResponseStatus.Success, APIResponse<AnswerReportResponse>.Success(res, "Answer submitted"));
            }
            catch (AppException ex) when (ex.ErrorCode == AppErrorCode.SessionNotFound)
            {
                return StatusCode((int)ResponseStatus.NotFound, APIResponse<string>.Fail(AppErrorCode.SessionNotFound, ResponseStatus.NotFound));
            }
            catch (AppException ex)
            {
                return StatusCode((int)ResponseStatus.Forbidden, APIResponse<string>.Fail(ex.ErrorCode, ResponseStatus.Forbidden));
            }
            catch (Exception)
            {
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        [HttpGet("{sessionId}/report")]
        public async Task<IActionResult> GetReport(int sessionId)
        {
            try
            {
                var res = await _sessionService.GetReportBySessionAsync(sessionId);
                return StatusCode((int)ResponseStatus.Success, APIResponse<ReportResponse>.Success(res, "Report fetched"));
            }
            catch (AppException ex) when (ex.ErrorCode == AppErrorCode.SessionNotFound)
            {
                return StatusCode((int)ResponseStatus.NotFound, APIResponse<string>.Fail(AppErrorCode.SessionNotFound, ResponseStatus.NotFound));
            }
        }
    }
}
