using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class SubscriptionPlan
{
    public int PlanId { get; set; }

    public string PlanName { get; set; } = null!;

    public decimal Price { get; set; }

    public int DurationDays { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
