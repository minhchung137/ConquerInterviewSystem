using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Interfaces
{
    public interface IUserService
    {
        public UserResponse GetUserById(int userId);
        public List<UserResponse> GetAllUsers();
        UserResponse UpdateUser(int userId, UpdateUserRequest request);
        void SoftDeleteUser(int userId);
    }
}
