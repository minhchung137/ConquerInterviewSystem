using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewBO.DTOs.Requests
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(255, ErrorMessage = "Full name must be less than 255 characters")]
        public string? FullName { get; set; }
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20, ErrorMessage = "Phone number must be less than 20 characters")]
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(RegisterRequest), nameof(ValidateDateOfBirth))]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        [StringLength(10, ErrorMessage = "Gender must be less than 10 characters")]
        public string? Gender { get; set; }
        [Url(ErrorMessage = "Invalid avatar URL format")]
        [StringLength(500, ErrorMessage = "Avatar URL must be less than 500 characters")]
        public string? AvatarUrl { get; set; }
        public static ValidationResult? ValidateDateOfBirth(DateTime? date, ValidationContext context)
        {
            if (date.HasValue && date.Value >= DateTime.Today)
            {
                return new ValidationResult("Date of birth must be in the past");
            }
            return ValidationResult.Success;
        }
    }
}
