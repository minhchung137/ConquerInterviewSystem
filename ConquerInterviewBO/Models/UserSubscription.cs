using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConquerInterviewBO.Models;

[Table("usersubscriptions")]
public partial class UserSubscription
{
    public int SubscriptionId { get; set; }

    public int UserId { get; set; }

    public int PlanId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Status { get; set; }

    public virtual SubscriptionPlan Plan { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
