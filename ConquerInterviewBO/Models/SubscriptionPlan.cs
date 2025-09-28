using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class SubscriptionPlan
{
    public int plan_id { get; set; }

    public string plan_name { get; set; } = null!;

    public decimal price { get; set; }

    public int duration_days { get; set; }

    public bool? is_active { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
