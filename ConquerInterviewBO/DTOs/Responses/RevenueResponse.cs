using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.DTOs.Responses
{
    public class RevenueResponse
    {
        public decimal TotalRevenue { get; set; } 
        public int TotalSubscriptions { get; set; }
        public List<SubscriptionDetailResponse> Details { get; set; } = new List<SubscriptionDetailResponse>();
    }

    public class SubscriptionDetailResponse
    {
        public int SubscriptionId { get; set; }
        public string UserEmail { get; set; }
        public string PlanName { get; set; }
        public decimal Price { get; set; }
        public DateOnly? StartDate { get; set; }
        public string Status { get; set; }
    }
}