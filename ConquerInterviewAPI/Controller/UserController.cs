using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewServices.Implements;
using ConquerInterviewServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConquerInterviewAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize(Roles = "ADMIN,STAFF,CUSTOMER")]
        [HttpGet("{userId}")]
        public IActionResult GetUserById(int userId)
        {
            var userResponse = _userService.GetUserById(userId);
            if (userResponse == null)
            {
                return StatusCode((int)ResponseStatus.NotFound,
                    APIResponse<string>.Fail(AppErrorCode.UserNotFound, ResponseStatus.NotFound));
            }
            return StatusCode((int)ResponseStatus.Success,
                APIResponse<UserResponse>.Success(userResponse, "Get user success"));
        }


        [Authorize(Roles = "ADMIN")]
        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            if (users == null || users.Count == 0)
            {
                return StatusCode((int)ResponseStatus.NotFound,
                    APIResponse<string>.Fail(AppErrorCode.ListIsEmpty, ResponseStatus.NotFound));
            }
            return StatusCode((int)ResponseStatus.Success,
                APIResponse<List<UserResponse>>.Success(users, "Get all users success"));
        }


        [Authorize(Roles = "ADMIN,STAFF,CUSTOMER")]
        [HttpPut("{userId}")]
        public IActionResult UpdateUser(int userId, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var userResponse = _userService.UpdateUser(userId, request);
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


        [Authorize(Roles = "ADMIN,STAFF,CUSTOMER")]
        [HttpDelete("{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            try
            {
                _userService.SoftDeleteUser(userId);
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
