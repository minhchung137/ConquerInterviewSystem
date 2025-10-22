namespace ConquerInterviewBO.DTOs.Responses
{
    public class PlanResponse
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; } = null!;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public bool IsActive { get; set; }
    }
}