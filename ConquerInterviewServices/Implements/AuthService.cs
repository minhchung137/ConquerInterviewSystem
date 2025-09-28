using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewBO.Models;
using ConquerInterviewRepositories.Interfaces;
using ConquerInterviewServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public UserResponse Register(RegisterRequest request)
        {
            // Check trùng
            if (_authRepository.GetUserByEmail(request.Email) != null || _authRepository.GetUserByUsername(request.Username) != null)
            {
                return null;
            }

            var user = new User
            {
                username = request.Username,
                email = request.Email,
                full_name = request.FullName,
                phone_number = request.PhoneNumber,
                status = true,
                date_of_birth = request.DateOfBirth.HasValue ? DateOnly.FromDateTime(request.DateOfBirth.Value) : null,
                gender = request.Gender,
                avatar_url = request.AvatarUrl,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,
                password_hash = request.Password
            };

            _authRepository.AddUser(user);

            return MapToUserResponse(user);
        }
        public UserResponse Login(LoginRequest request)
        {
            var user = _authRepository.GetUserByUsernameAndPass(request.Username, request.Password);
            if (user == null)
            {
                return null;
            }
            return MapToUserResponse(user);
        }

        public List<UserResponse> GetAllUsers()
        {
            var users = _authRepository.GetAllUsers();
            return users.Select(user => MapToUserResponse(user)).ToList();
        }

        public UserResponse GetUserById(int id)
        {
            var user = _authRepository.GetUserById(id);
            if (user == null)
            {
                return null;
            }
            
            return MapToUserResponse(user);
        }
        public UserResponse UpdateUser(int userId, UpdateUserRequest request)
        {
            var existing = _authRepository.GetUserById(userId);
            if (existing == null)
                throw new AppException(AppErrorCode.UserNotFound);

            existing.full_name = request.FullName;
            existing.phone_number = request.PhoneNumber;
            existing.date_of_birth = request.DateOfBirth.HasValue ? DateOnly.FromDateTime(request.DateOfBirth.Value) : null;
            existing.gender = request.Gender;
            existing.avatar_url = request.AvatarUrl;
            existing.updated_at = DateTime.UtcNow;

            // gọi repository -> DAO cập nhật
            _authRepository.UpdateUser(existing);

            // reload hoặc dùng existing (roles có thể chưa loaded; GetUserById bao gồm roles)
            var user = _authRepository.GetUserById(userId);
            return MapToUserResponse(user);
        }

        public void SoftDeleteUser(int userId)
        {
            _authRepository.SoftDeleteUser(userId);
        }

        // Helper method to map User to UserResponse
        private UserResponse MapToUserResponse(User user)
        {
            return new UserResponse
            {
                UserId = user.user_id,
                Username = user.username,
                Email = user.email,
                FullName = user.full_name,
                PhoneNumber = user.phone_number,
                DateOfBirth = user.date_of_birth?.ToDateTime(TimeOnly.MinValue),
                Gender = user.gender,
                AvatarUrl = user.avatar_url,
                Created_at = user.created_at,
                Updated_at = user.updated_at,
                Roles = user.roles?.Select(r => r.role_name).ToList() ?? new List<string>()
            };
        }
    }
}
