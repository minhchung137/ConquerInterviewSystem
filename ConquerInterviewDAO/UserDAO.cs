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
    public class UserDAO
    {
        private readonly ConquerInterviewDbContext _context;
        private static UserDAO instance;
        public static UserDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserDAO();
                }
                return instance;
            }
        }
        public UserDAO()
        {
            _context = new ConquerInterviewDbContext();
        }
        // -- Get all users ---
        public List<User> GetAllUsers()
        {
            return _context.Users
                .Include(u => u.Roles)
                .ToList();
        }
        // --- Get user by id ---
        public User GetUserById(int id)
        {
            return _context.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.UserId == id && u.Status == true);
        }
        // --- Update user ---
        public void UpdateUser(User updatedUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);
            if (user == null)
                throw new AppException(AppErrorCode.UserNotFound);

            // cập nhật các trường cho phép (không thay password/email/username ở đây)
            user.FullName = updatedUser.FullName;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.DateOfBirth = updatedUser.DateOfBirth;
            user.Gender = updatedUser.Gender;
            user.AvatarUrl = updatedUser.AvatarUrl;
            user.UpdatedAt = DateTime.UtcNow;


            _context.SaveChanges();
        }

        // --- Soft delete user ---
        public void SoftDeleteUser(int userId)
        {
            var user = _context.Users
                .AsTracking()
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                throw new AppException(AppErrorCode.UserNotFound);

            if (user.Status == false)
                throw new AppException(AppErrorCode.UserAlreadyDeleted);

            user.Status = false;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}
