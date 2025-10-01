using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewBO.Models;
using ConquerInterviewRepositories.Implements;
using ConquerInterviewRepositories.Interfaces;
using ConquerInterviewServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Implements
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public List<UserResponse> GetAllUsers()
        {
            var users = _userRepository.GetAllUsers();
            return users.Select(user => MapToUserResponse(user)).ToList();
        }

        public UserResponse GetUserById(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                return null;
            }

            return MapToUserResponse(user);
        }
        public UserResponse UpdateUser(int userId, UpdateUserRequest request)
        {
            var existing = _userRepository.GetUserById(userId);
            if (existing == null)
                throw new AppException(AppErrorCode.UserNotFound);

            existing.FullName = request.FullName;
            existing.PhoneNumber = request.PhoneNumber;
            existing.DateOfBirth = request.DateOfBirth.HasValue ? DateOnly.FromDateTime(request.DateOfBirth.Value) : null;
            existing.Gender = request.Gender;
            existing.AvatarUrl = request.AvatarUrl;
            existing.UpdatedAt = DateTime.UtcNow;

            // gọi repository -> DAO cập nhật
            _userRepository.UpdateUser(existing);

            // reload hoặc dùng existing (roles có thể chưa loaded; GetUserById bao gồm roles)
            var user = _userRepository.GetUserById(userId);
            return MapToUserResponse(user);
        }

        public void SoftDeleteUser(int userId)
        {
            _userRepository.SoftDeleteUser(userId);
        }

        // Helper method to map User to UserResponse
        private UserResponse MapToUserResponse(User user)
        {
            return new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth?.ToDateTime(TimeOnly.MinValue),
                Gender = user.Gender,
                AvatarUrl = user.AvatarUrl,
                Created_at = user.CreatedAt,
                Updated_at = user.UpdatedAt,
                Roles = user.Roles?.Select(r => r.RoleName).ToList() ?? new List<string>()
            };
        }
    }
}
