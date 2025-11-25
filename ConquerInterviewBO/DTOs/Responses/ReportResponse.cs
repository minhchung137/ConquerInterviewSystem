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
        public DateTime? SessionDate {  get; set; } 
        public List<ReportQuestionResponse> Reports { get; set; } = new();
    }
}
