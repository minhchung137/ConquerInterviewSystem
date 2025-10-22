using System.ComponentModel.DataAnnotations;

namespace ConquerInterviewBO.DTOs.Requests
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "User ID là bắt buộc")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Plan ID là bắt buộc")]
        public int PlanId { get; set; }
    }
}