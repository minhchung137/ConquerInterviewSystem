using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewBO.DTOs.Responses
{
    public class ReportQuestionResponse
    {
        public string? QuestionText { get; set; }
        public string? OverallAssessment { get; set; }
        public string? FacialExpression { get; set; }
        public string? SpeakingSpeedClarity { get; set; }
        public string? ExpertiseExperience { get; set; }
        public string? ResponseDurationPerQuestion { get; set; }
        public string? AnswerContentAnalysis { get; set; }
        public string? ComparisonWithOtherCandidates { get; set; }
        public string? ProblemSolvingSkills { get; set; }
        public string? Status { get; set; }
    }
}
