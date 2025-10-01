using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewRepositories.Implements;
using ConquerInterviewRepositories.Interfaces;
using ConquerInterviewServices.Implements;
using ConquerInterviewServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConquerInterviewAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var userResponse = _authService.Register(request);

            if (userResponse == null)
            {
                return StatusCode((int)ResponseStatus.Conflict,
                    APIResponse<string>.Fail(AppErrorCode.UserAlreadyExists, ResponseStatus.Conflict));
            }

            return StatusCode((int)ResponseStatus.Success,
                APIResponse<UserResponse>.Success(userResponse, "Register success"));
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var userResponse = _authService.Login(request);
            if (userResponse == null)
            {
                return StatusCode((int)ResponseStatus.Unauthorized,
                    APIResponse<string>.Fail(AppErrorCode.InvalidCredentials, ResponseStatus.Unauthorized));
            }
            return StatusCode((int)ResponseStatus.Created,
                APIResponse<AuthResponse>.Success(userResponse, "Login success"));
        }
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                _authService.ForgotPassword(request.Email);
                return StatusCode((int)ResponseStatus.Success,
                    APIResponse<string>.Success("Reset password token sent to your email."));
            }
            catch (AppException ex)
            {
                return StatusCode((int)ResponseStatus.NotFound,
                    APIResponse<string>.Fail(ex.ErrorCode, ResponseStatus.NotFound));
            }
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                _authService.ResetPassword(request.Token, request.NewPassword);
                return StatusCode((int)ResponseStatus.Success,
                    APIResponse<string>.Success("Password has been reset successfully."));
            }
            catch (AppException ex)
            {
                return StatusCode((int)ResponseStatus.BadRequest,
                    APIResponse<string>.Fail(ex.ErrorCode, ResponseStatus.BadRequest));
            }
        }

    }
}
