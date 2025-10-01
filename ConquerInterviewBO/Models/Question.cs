using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class Question
{
    public int QuestionId { get; set; }

    public string QuestionText { get; set; } = null!;

    public int? DifficultyLevel { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<InterviewAnswer> InterviewAnswers { get; set; } = new List<InterviewAnswer>();
}
