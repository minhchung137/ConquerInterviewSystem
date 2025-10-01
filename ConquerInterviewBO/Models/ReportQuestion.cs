using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class ReportQuestion
{
    public int ReportQId { get; set; }

    public string? OverallAssessment { get; set; }

    public string? FacialExpression { get; set; }

    public string? SpeakingSpeedClarity { get; set; }

    public string? ExpertiseExperience { get; set; }

    public string? ResponseDurationPerQuestion { get; set; }

    public string? AnswerContentAnalysis { get; set; }

    public string? ComparisonWithOtherCandidates { get; set; }

    public string? ProblemSolvingSkills { get; set; }

    public string? Status { get; set; }

    public int CustomerId { get; set; }

    public int InterviewAId { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual InterviewAnswer InterviewA { get; set; } = null!;

    public virtual ICollection<Personalization> Personalizations { get; set; } = new List<Personalization>();
}
