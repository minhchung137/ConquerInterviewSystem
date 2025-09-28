using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class User
{
    public int user_id { get; set; }

    public string username { get; set; } = null!;

    public string email { get; set; } = null!;

    public string password_hash { get; set; } = null!;

    public string? full_name { get; set; }

    public string? phone_number { get; set; }

    public DateOnly? date_of_birth { get; set; }

    public string? gender { get; set; }

    public string? avatar_url { get; set; }

    public bool? status { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<InterviewSession> InterviewSessions { get; set; } = new List<InterviewSession>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Personalization> Personalizations { get; set; } = new List<Personalization>();

    public virtual ICollection<ReportQuestion> ReportQuestions { get; set; } = new List<ReportQuestion>();

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();

    public virtual ICollection<Role> roles { get; set; } = new List<Role>();
}
