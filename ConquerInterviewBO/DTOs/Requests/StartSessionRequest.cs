using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewBO.DTOs.Requests
{
    public class StartSessionRequest
    {
        public int UserId { get; set; }
        public string JobPosition { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public int? DurationMinutes { get; set; } = 30;
    }
}
