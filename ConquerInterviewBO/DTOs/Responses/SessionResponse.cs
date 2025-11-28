using System;

namespace ConquerInterviewBO.DTOs.Responses
{
    public class SessionResponse
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public string? JobPosition { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Status { get; set; }

        public double DurationMinutes { get; set; }
    }
}