using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class InterviewAnswer
{
    public int interview_a_id { get; set; }

    public int session_id { get; set; }

    public int question_id { get; set; }

    public string? text_answer { get; set; }

    public string? voice_url { get; set; }

    public string? video_url { get; set; }

    public DateTime? created_at { get; set; }

    public virtual ICollection<ReportQuestion> ReportQuestions { get; set; } = new List<ReportQuestion>();

    public virtual Question question { get; set; } = null!;

    public virtual InterviewSession session { get; set; } = null!;
}
