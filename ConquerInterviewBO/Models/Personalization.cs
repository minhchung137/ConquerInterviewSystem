using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class Personalization
{
    public int personalization_id { get; set; }

    public string? name_practice { get; set; }

    public string? practice { get; set; }

    public string? exercise { get; set; }

    public string? objective { get; set; }

    public int customer_id { get; set; }

    public int report_q_id { get; set; }

    public virtual User customer { get; set; } = null!;

    public virtual ReportQuestion report_q { get; set; } = null!;
}
