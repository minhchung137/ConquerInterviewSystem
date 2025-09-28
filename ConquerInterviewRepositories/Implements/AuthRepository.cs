using ConquerInterviewBO.Models;
using ConquerInterviewDAO;
using ConquerInterviewRepositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Implements
{
    public class AuthRepository : IAuthRepository
    {
        public List<User> GetAllUsers() => AuthDAO.Instance.GetAllUsers();

        public User GetUserById(int id) => AuthDAO.Instance.GetUserById(id);

        public User GetUserByUsernameAndPass(string username, string pass)
            => AuthDAO.Instance.GetUserByUsernameAndPass(username, pass);

        public User GetUserByEmail(string email)
            => AuthDAO.Instance.GetUserByEmail(email);
        public User GetUserByUsername(string username)
            => AuthDAO.Instance.GetUserByUsername(username);
        public void AddUser(User user) => AuthDAO.Instance.AddUser(user);
        public void UpdateUser(User user) => AuthDAO.Instance.UpdateUser(user);

        public void SoftDeleteUser(int userId) => AuthDAO.Instance.SoftDeleteUser(userId);

    }
}
