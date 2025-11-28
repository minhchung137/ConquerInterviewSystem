namespace ConquerInterviewBO.DTOs.Responses
{
    public class PaymentLinkResponse
    {
        public int OrderId { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;
    }
}