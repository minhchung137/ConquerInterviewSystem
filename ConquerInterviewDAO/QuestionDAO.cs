using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewBO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConquerInterviewDAO
{
    public class QuestionDAO
    {
        private readonly ConquerInterviewDbContext _context;
        private static QuestionDAO instance;
        public static QuestionDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new QuestionDAO();
                }
                return instance;
            }
        }
        public QuestionDAO()
        {
            _context = new ConquerInterviewDbContext();
        }

        public async Task<List<Question>> GetRandomQuestionsAsync(int count, int level)
        {
            var allQuestions = await _context.Questions
                .Where(q => q.DifficultyLevel == level && q.IsActive == true)
                .ToListAsync(); // Lấy toàn bộ ra (danh sách nhỏ nên ổn)

            return allQuestions
                .OrderBy(q => Guid.NewGuid()) // Random ở RAM
                .Take(count)
                .ToList();
        }


        public async Task<Question> GenerateAIQuestionAsync(string topic, string industry, string jobPosition)
        {
            using var client = new HttpClient();

            var payload = new
            {
                topic = topic ?? "General",
                industry = industry ?? "General",
                jobPosition = jobPosition ?? "General",
                difficultyLevel = 2
            };

            var response = await client.PostAsJsonAsync("http://localhost:5000/api/generate_question", payload);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"AI Service error: {response.StatusCode} - {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var aiQuestion = JsonSerializer.Deserialize<QuestionResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (aiQuestion == null || string.IsNullOrWhiteSpace(aiQuestion.QuestionText))
                throw new Exception("AI question generation failed: invalid response from Flask API.");

            var question = new Question
            {
                QuestionText = aiQuestion.QuestionText,
                DifficultyLevel = aiQuestion.DifficultyLevel
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return question;
        }

    }
}
