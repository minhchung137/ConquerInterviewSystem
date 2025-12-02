using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Implements
{
    public class AIService
    {
        private readonly HttpClient _httpClient;

        public AIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://14.225.212.59:5000"); // Flask AI server
        }

        public async Task<AIQuestionResponse?> StartInterviewAsync(string topic, string industry)
        {
            var payload = new { topic = topic, industry_filter = industry };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/start_interview", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AIQuestionResponse>(result);
        }

        public async Task<AISubmitResponse?> SubmitRoundAsync(int round, string answer)
        {
            var payload = new { round = round, answer = answer };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/submit_round", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AISubmitResponse>(result);
        }
    }

    // DTO phản hồi từ Flask
    public class AIQuestionResponse
    {
        public int current_round { get; set; }
        public int total_rounds { get; set; }
        public string question { get; set; }
    }

    public class AISubmitResponse
    {
        public string status { get; set; }
        public string report { get; set; }
        public string feedback { get; set; }
        public string suggested_answer { get; set; }
        public string final_report { get; set; }
    }
}
