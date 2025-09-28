using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class InterviewSession
{
    public int session_id { get; set; }

    public int user_id { get; set; }

    public DateTime? start_time { get; set; }

    public DateTime? end_time { get; set; }

    public string? job_position { get; set; }

    public string? status { get; set; }

    public virtual ICollection<InterviewAnswer> InterviewAnswers { get; set; } = new List<InterviewAnswer>();

    public virtual User user { get; set; } = null!;
}
