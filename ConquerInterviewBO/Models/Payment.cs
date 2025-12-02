using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConquerInterviewBO.Models;
[Table("payments")]
public partial class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public string? Provider { get; set; }

    public string? TransactionId { get; set; }

    public decimal Amount { get; set; }

    public string? Status { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}
