using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewDAO
{
    public class AuthDAO
    {
        private readonly ConquerInterviewDbContext _context;
        private static AuthDAO instance;
        public static AuthDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AuthDAO();
                }
                return instance;
            }
        }
        public AuthDAO()
        {
            _context = new ConquerInterviewDbContext();
        }
        public List<User> GetAllUsers()
        {
            return _context.Users
                .Include(u => u.roles)
                .ToList();
        }
        public User GetUserByUsernameAndPass(string userName, string pass)
        {
            return _context.Users
                .Include(u => u.roles)
                .FirstOrDefault(u => u.username.Equals(userName) && u.password_hash.Equals(pass) && u.status == true);
        }
        public User GetUserByEmail(string email)
        {
            return _context.Users
                .Include(u => u.roles)
                .FirstOrDefault(u => u.email.Equals(email));
        }
        public User GetUserByUsername(string username)
        {
            return _context.Users
                .Include(u => u.roles)
                .FirstOrDefault(u => u.username.Equals(username));
        }
        // --- Get user by id ---
        public User GetUserById(int id)
        {
            return _context.Users
                .Include(u => u.roles)
                .FirstOrDefault(u => u.user_id == id && u.status == true);
        }
        // --- Add new user ---
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges(); 

            int roleId = (user.user_id == 1) ? 1 : 3;

            var role = _context.Roles.FirstOrDefault(r => r.role_id == roleId);
            if (role == null)
            {
                throw new AppException(AppErrorCode.InternalError);
            }

            user.roles.Add(role);

            _context.SaveChanges();
        }
        // --- Soft delete ---
        public void SoftDeleteUser(int userId)
        {
            var user = _context.Users
                .AsTracking()
                .FirstOrDefault(u => u.user_id == userId);

            if (user == null)
                throw new AppException(AppErrorCode.UserNotFound);

            if (user.status == false)
                throw new AppException(AppErrorCode.UserAlreadyDeleted);

            user.status = false;
            user.updated_at = DateTime.UtcNow;

            _context.Users.Update(user); 
            _context.SaveChanges();
        }


        // --- Update user fields (only allowed fields) ---
        public void UpdateUser(User updatedUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.user_id == updatedUser.user_id);
            if (user == null)
                throw new AppException(AppErrorCode.UserNotFound);

            // cập nhật các trường cho phép (không thay password/email/username ở đây)
            user.full_name = updatedUser.full_name;
            user.phone_number = updatedUser.phone_number;
            user.date_of_birth = updatedUser.date_of_birth;
            user.gender = updatedUser.gender;
            user.avatar_url = updatedUser.avatar_url;
            user.updated_at = DateTime.UtcNow;

            _context.SaveChanges();
        }

    }
}
