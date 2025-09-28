﻿using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class Role
{
    public int role_id { get; set; }

    public string role_name { get; set; } = null!;

    public virtual ICollection<User> users { get; set; } = new List<User>();
}
