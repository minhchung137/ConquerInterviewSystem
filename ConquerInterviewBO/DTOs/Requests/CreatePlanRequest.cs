using System.ComponentModel.DataAnnotations;

namespace ConquerInterviewBO.DTOs.Requests
{
    public class CreatePlanRequest
    {
        [Required(ErrorMessage = "Tên gói là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên gói không được vượt quá 100 ký tự")]
        public string PlanName { get; set; } = null!;

        [Required(ErrorMessage = "Giá tiền là bắt buộc")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Giá tiền phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Thời hạn gói là bắt buộc")]
        [Range(1, 3650, ErrorMessage = "Thời hạn phải từ 1 đến 3650 ngày")]
        public int DurationDays { get; set; }
    }
}