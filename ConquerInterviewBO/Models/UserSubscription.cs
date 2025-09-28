using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class UserSubscription
{
    public int subscription_id { get; set; }

    public int user_id { get; set; }

    public int plan_id { get; set; }

    public DateOnly? start_date { get; set; }

    public DateOnly? end_date { get; set; }

    public string? status { get; set; }

    public virtual SubscriptionPlan plan { get; set; } = null!;

    public virtual User user { get; set; } = null!;
}
