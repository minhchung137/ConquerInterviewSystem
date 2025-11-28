using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewBO.DTOs.Responses
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public bool? Status { get; set; }
        public DateTime? Created_at { get; set; }

        public DateTime? Updated_at { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
