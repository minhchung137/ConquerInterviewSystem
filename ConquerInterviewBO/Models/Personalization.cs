using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class Personalization
{
    public int PersonalizationId { get; set; }

    public string? NamePractice { get; set; }

    public string? Practice { get; set; }

    public string? Exercise { get; set; }

    public string? Objective { get; set; }

    public int CustomerId { get; set; }

    public int ReportQId { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual ReportQuestion ReportQ { get; set; } = null!;
}
