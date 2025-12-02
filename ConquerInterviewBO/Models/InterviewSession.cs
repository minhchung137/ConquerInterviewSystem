using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConquerInterviewBO.Models;
[Table("interviewsessions")]
public partial class InterviewSession
{
    public int SessionId { get; set; }

    public int UserId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? JobPosition { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<InterviewAnswer> InterviewAnswers { get; set; } = new List<InterviewAnswer>();

    public virtual User User { get; set; } = null!;
}
