using ConquerInterviewBO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Interfaces
{
    public interface IAuthRepository
    {
        public User GetUserByUsernameAndPass(string userName, string pass);
        public User GetUserByEmail(string email);
        public User GetUserByUsername(string username);
        public void AddUser(User user);
        public void UpdateUserToken(int userId, string refreshToken);
        public User GetUserByResetToken(string token);
        public void UpdateUser(User user);
    }
}
