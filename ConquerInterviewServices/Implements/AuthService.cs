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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        public AuthService(IAuthRepository authRepository, IJwtService jwtService, IEmailService emailService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
            _emailService = emailService;
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
                Username = request.Username,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Status = true,
                DateOfBirth = request.DateOfBirth.HasValue ? DateOnly.FromDateTime(request.DateOfBirth.Value) : null,
                Gender = request.Gender,
                AvatarUrl = request.AvatarUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PasswordHash = request.Password,
            };

            _authRepository.AddUser(user);
            _emailService.SendEmail(user.Email, "Welcome",
               $"Wellcome to Conquer Interview");

            return MapToUserResponse(user);
        }
        public AuthResponse Login(LoginRequest request)
        {
            var user = _authRepository.GetUserByUsername(request.Username);
            if (user == null)
                throw new AppException(AppErrorCode.InvalidUsername);

            if (user.PasswordHash != request.Password)
                throw new AppException(AppErrorCode.InvalidPassword);

            if (user.Status == false)
                throw new AppException(AppErrorCode.UserDisabled);


            // Gọi JwtService
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Lưu refresh token vào DB
            _authRepository.UpdateUserToken(user.UserId, refreshToken);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserResponse
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    AvatarUrl = user.AvatarUrl,
                    DateOfBirth = user.DateOfBirth?.ToDateTime(new TimeOnly(0, 0)),
                    Created_at = user.CreatedAt,
                    Updated_at = user.UpdatedAt,
                    Roles = user.Roles.Select(r => r.RoleName).ToList()
                }
            };
        }
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

        public void ForgotPassword(string email)
        {
            var user = _authRepository.GetUserByEmail(email);
            if (user == null)
                throw new AppException(AppErrorCode.UserNotFound);

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);

            _authRepository.UpdateUser(user);

            // gửi email
            _emailService.SendEmail(user.Email, "Password Reset",
                $"Click this link to reset your password: https://yourdomain.com/reset-password?token={token}");
        }

        public void ResetPassword(string token, string newPassword)
        {
            var user = _authRepository.GetUserByResetToken(token);
            if (user == null)
                throw new AppException(AppErrorCode.InvalidToken);

            if (user.ResetTokenExpiry < DateTime.UtcNow)
                throw new AppException(AppErrorCode.TokenExpired);

            user.PasswordHash = newPassword; // Hash nếu cần
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            _authRepository.UpdateUser(user);
        }

    }
}
