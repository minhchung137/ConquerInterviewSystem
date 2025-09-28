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
        List<User> GetAllUsers();
        User GetUserByUsernameAndPass(string userName, string pass);
        User GetUserByEmail(string email);
        User GetUserByUsername(string username);
        User GetUserById(int id);
        void AddUser(User user);
        void UpdateUser(User user);
        void SoftDeleteUser(int userId);
    }
}
