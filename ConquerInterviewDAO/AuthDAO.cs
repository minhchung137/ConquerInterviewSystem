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
        
        public User GetUserByUsernameAndPass(string userName, string pass)
        {
            return _context.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Username.Equals(userName) && u.PasswordHash.Equals(pass) && u.Status == true);
        }
        public User GetUserByEmail(string email)
        {
            return _context.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Email.Equals(email));
        }
        public User GetUserByUsername(string username)
        {
            return _context.Users
                .AsNoTracking()
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Username.Equals(username));
        }

        public void AddUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();

                int roleId = (user.UserId == 1) ? 1 : 3;
                var role = _context.Roles.FirstOrDefault(r => r.RoleId == roleId);
                if (role == null)
                {
                    throw new AppException(AppErrorCode.InternalError);
                }
                user.Roles.Add(role);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Duplicate entry") == true
                    || ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
                {
                    throw new AppException(AppErrorCode.UserAlreadyExists);
                }
                throw;
            }
        }


        public void UpdateUserToken(int userId, string refreshToken)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
                throw new AppException(AppErrorCode.UserNotFound);

            user.Token = refreshToken;
            user.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }


        public User GetUserByResetToken(string token)
        {
            var user = _context.Users.FirstOrDefault(u => u.ResetToken == token);

            if (user == null)
                throw new AppException(AppErrorCode.InvalidToken);

            if (!user.ResetTokenExpiry.HasValue || user.ResetTokenExpiry <= DateTime.UtcNow)
                throw new AppException(AppErrorCode.TokenExpired);

            return user;
        }

        public void UpdateUser(User updatedUser)
        {
            var existing = _context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);
            if (existing == null)
                throw new AppException(AppErrorCode.UserNotFound);

            existing.FullName = updatedUser.FullName;
            existing.PhoneNumber = updatedUser.PhoneNumber;
            existing.DateOfBirth = updatedUser.DateOfBirth;
            existing.Gender = updatedUser.Gender;
            existing.AvatarUrl = updatedUser.AvatarUrl;
            existing.PasswordHash = updatedUser.PasswordHash;
            existing.ResetToken = updatedUser.ResetToken;
            existing.ResetTokenExpiry = updatedUser.ResetTokenExpiry;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();
        }
    }
}
