using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewBO.DTOs.Responses
{
    public class PersionalAllResponse
    {
        public int SessionId { get; set; }
        public DateTime? SessionDate { get; set; }
        public List<PersonalizationHistoryItemResponse> Personalizations { get; set; } = new();
    }
}
