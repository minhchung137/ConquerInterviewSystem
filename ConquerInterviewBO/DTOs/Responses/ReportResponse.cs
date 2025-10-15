using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewBO.DTOs.Responses
{
    public class ReportResponse
    {
        public int SessionId { get; set; }
        public string JobPosition { get; set; } = string.Empty;
        public List<AnswerReportResponse> Reports { get; set; } = new();
    }
}
