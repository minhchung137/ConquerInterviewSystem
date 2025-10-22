using System;

namespace ConquerInterviewBO.DTOs.Responses
{
    public class PersonalizationResponse
    {
        public int PersonalizationId { get; set; }
        public string? NamePractice { get; set; }
        public string? Practice { get; set; }
        public string? Exercise { get; set; }
        public string? Objective { get; set; }
        public int CustomerId { get; set; }
        public int ReportQId { get; set; }
    }
}