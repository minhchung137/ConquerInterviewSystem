using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class Order
{
    public int order_id { get; set; }

    public int user_id { get; set; }

    public int plan_id { get; set; }

    public decimal total_amount { get; set; }

    public string? status { get; set; }

    public DateTime? created_at { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual SubscriptionPlan plan { get; set; } = null!;

    public virtual User user { get; set; } = null!;
}
