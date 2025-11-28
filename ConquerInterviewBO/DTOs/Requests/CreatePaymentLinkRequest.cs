namespace ConquerInterviewBO.DTOs.Requests
{
    public class CreatePaymentLinkRequest
    {
        public int OrderId { get; set; }
        public string ReturnUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
    }
}