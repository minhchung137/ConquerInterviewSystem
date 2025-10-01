using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Interfaces
{
    public interface IAuthService
    {
        public UserResponse Register(RegisterRequest request);
        public AuthResponse Login(LoginRequest request);
        public void ForgotPassword(string email);
        public void ResetPassword(string token, string newPassword);
    }
}
