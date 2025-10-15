using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConquerInterviewBO.DTOs.Responses
{
    public class QuestionResponse
    {
        [JsonPropertyName("question_id")]
        public int QuestionId { get; set; }

        [JsonPropertyName("question_text")]
        public string QuestionText { get; set; }

        [JsonPropertyName("difficulty_level")]
        public int DifficultyLevel { get; set; }
    }
}
