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

        public AuthController()
        {
            IAuthRepository authRepo = new AuthRepository();
            _authService = new AuthService(authRepo);
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
                APIResponse<UserResponse>.Success(userResponse, "Login success"));
        }
        [HttpGet("{userId}")]
        public IActionResult GetUserById(int userId)
        {
            var userResponse = _authService.GetUserById(userId);
            if (userResponse == null)
            {
                return StatusCode((int)ResponseStatus.NotFound,
                    APIResponse<string>.Fail(AppErrorCode.UserNotFound, ResponseStatus.NotFound));
            }
            return StatusCode((int)ResponseStatus.Success,
                APIResponse<UserResponse>.Success(userResponse, "Get user success"));
        }

        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            var users = _authService.GetAllUsers();
            if (users == null || users.Count == 0)
            {
                return StatusCode((int)ResponseStatus.NotFound,
                    APIResponse<string>.Fail(AppErrorCode.ListIsEmpty, ResponseStatus.NotFound));
            }
            return StatusCode((int)ResponseStatus.Success,
                APIResponse<List<UserResponse>>.Success(users, "Get all users success"));
        }
        [HttpPut("{userId}")]
        public IActionResult UpdateUser(int userId, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var userResponse = _authService.UpdateUser(userId, request);
                if (userResponse == null)
                {
                    return StatusCode((int)ResponseStatus.NotFound,
                        APIResponse<string>.Fail(AppErrorCode.UserNotFound, ResponseStatus.NotFound));
                }
                return StatusCode((int)ResponseStatus.Success,
                    APIResponse<UserResponse>.Success(userResponse, "User updated successfully"));
            }
            catch (AppException ex) when (ex.ErrorCode == AppErrorCode.UserNotFound)
            {
                return StatusCode((int)ResponseStatus.NotFound,
                    APIResponse<string>.Fail(AppErrorCode.UserNotFound, ResponseStatus.NotFound));
            }
            catch (Exception)
            {
                return StatusCode((int)ResponseStatus.InternalServerError,
                    APIResponse<string>.Fail(AppErrorCode.UserUpdateFailed, ResponseStatus.InternalServerError));
            }
        }

        [HttpDelete("{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            try
            {
                _authService.SoftDeleteUser(userId);
                return StatusCode((int)ResponseStatus.Success,
                    APIResponse<string>.Success("User deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode((int)ResponseStatus.NotFound,
                    APIResponse<string>.Fail(AppErrorCode.UserNotFound, ResponseStatus.NotFound));
            }
        }
    }
}
