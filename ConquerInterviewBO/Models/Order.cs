using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int PlanId { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual SubscriptionPlan Plan { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
