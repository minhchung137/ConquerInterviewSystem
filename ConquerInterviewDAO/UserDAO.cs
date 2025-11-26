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
                .FirstOrDefault(u => u.UserId == id );
        }
        
        public void UpdateUser(User updatedUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);
            if (user == null)
                throw new AppException(AppErrorCode.UserNotFound);

            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.FullName = updatedUser.FullName;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.DateOfBirth = updatedUser.DateOfBirth;
            user.Gender = updatedUser.Gender;
            user.AvatarUrl = updatedUser.AvatarUrl;
            user.UpdatedAt = DateTime.UtcNow;


            _context.SaveChanges();
        }

        
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
        // --- Update user role ---
        public void UpdateUserRole(int userId, string roleName)
        {
            var user = _context.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                throw new AppException(AppErrorCode.UserNotFound);

            var role = _context.Roles.FirstOrDefault(r => r.RoleName == roleName);
            if (role == null)
                throw new AppException(AppErrorCode.RoleNotFound);

            // Xóa role cũ và gán role mới (tuỳ quan hệ n-n hoặc 1-n)
            user.Roles.Clear();
            user.Roles.Add(role);
            user.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();
        }
        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        // Đặt hàm này bên trong class UserDAO
        public void UpdateUserStatus(int userId, bool newStatus)
        {
            // 1. Tìm kiếm người dùng, sử dụng AsTracking để theo dõi thay đổi
            var user = _context.Users
                .AsTracking()
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                throw new AppException(AppErrorCode.UserNotFound);

            // 2. Kiểm tra xem trạng thái có cần cập nhật không
            if (user.Status == newStatus)
            {
                // Tùy chọn: Log thông tin và không cần làm gì thêm
                return;
            }

            // 3. Cập nhật trạng thái và thời gian cập nhật
            user.Status = newStatus;
            user.UpdatedAt = DateTime.UtcNow;

            // 4. Lưu thay đổi vào database
            _context.SaveChanges();
        }

        public void UpdateTrialCount(int userId, int newTrialCount)
        {
            // 1. Tìm kiếm người dùng, sử dụng AsTracking để theo dõi thay đổi
            var user = _context.Users
                .AsTracking()
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                throw new AppException(AppErrorCode.UserNotFound); // Giả định AppException/AppErrorCode tồn tại

            // 2. Cập nhật TrialCount và thời gian cập nhật
            user.TrialCount = newTrialCount;
            user.UpdatedAt = DateTime.UtcNow; // Cập nhật thời gian thay đổi

            // 3. Lưu thay đổi vào database
            _context.SaveChanges();
        }
    }
}
