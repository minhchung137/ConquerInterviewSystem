using System;

namespace ConquerInterviewBO.DTOs.Responses
{
    public class UserSubscriptionResponse
    {
        public int SubscriptionId { get; set; }
        public int UserId { get; set; }
        public string PlanName { get; set; } 
        public decimal Price { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string Status { get; set; }
    }
}