using System;

namespace ConquerInterviewBO.DTOs.Responses
{
    public class PaymentResponse
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Provider { get; set; }
        public string Status { get; set; }
        public DateTime? PaidAt { get; set; }
        public string TransactionId { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}