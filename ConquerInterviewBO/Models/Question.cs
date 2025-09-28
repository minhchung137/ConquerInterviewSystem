using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class Question
{
    public int question_id { get; set; }

    public string question_text { get; set; } = null!;

    public int? difficulty_level { get; set; }

    public bool? is_active { get; set; }

    public virtual ICollection<InterviewAnswer> InterviewAnswers { get; set; } = new List<InterviewAnswer>();
}
