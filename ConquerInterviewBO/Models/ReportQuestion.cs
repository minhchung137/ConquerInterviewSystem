using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class ReportQuestion
{
    public int report_q_id { get; set; }

    public string? overall_assessment { get; set; }

    public string? facial_expression { get; set; }

    public string? speaking_speed_clarity { get; set; }

    public string? expertise_experience { get; set; }

    public string? response_duration_per_question { get; set; }

    public string? answer_content_analysis { get; set; }

    public string? comparison_with_other_candidates { get; set; }

    public string? problem_solving_skills { get; set; }

    public string? status { get; set; }

    public int customer_id { get; set; }

    public int interview_a_id { get; set; }

    public virtual ICollection<Personalization> Personalizations { get; set; } = new List<Personalization>();

    public virtual User customer { get; set; } = null!;

    public virtual InterviewAnswer interview_a { get; set; } = null!;
}
