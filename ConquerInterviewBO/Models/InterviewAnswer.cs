using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class InterviewAnswer
{
    public int InterviewAId { get; set; }

    public int SessionId { get; set; }

    public int QuestionId { get; set; }

    public string? TextAnswer { get; set; }

    public string? VoiceUrl { get; set; }

    public string? VideoUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual ICollection<ReportQuestion> ReportQuestions { get; set; } = new List<ReportQuestion>();

    public virtual InterviewSession Session { get; set; } = null!;
}
