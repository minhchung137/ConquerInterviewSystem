using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class Payment
{
    public int payment_id { get; set; }

    public int order_id { get; set; }

    public string? provider { get; set; }

    public string? transaction_id { get; set; }

    public decimal amount { get; set; }

    public string? status { get; set; }

    public DateTime? paid_at { get; set; }

    public virtual Order order { get; set; } = null!;
}
