using System.ComponentModel.DataAnnotations;

namespace ConquerInterviewBO.DTOs.Requests
{
    public class UpdateUserRoleRequest
    {
        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "RoleName is required")]
        public string RoleName { get; set; }
    }
}
