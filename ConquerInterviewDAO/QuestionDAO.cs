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


        public async Task<List<Question>> GenerateAIQuestionsAsync(string topic, string industry, string jobPosition, int QuestionDifficult)
        {
            using var client = new HttpClient();
            var newQuestions = new List<Question>();

            // Payload gửi đi (giữ nguyên logic cũ)
            var payload = new
            {
                topic = topic ?? "General",
                industry = industry ?? "General",
                jobPosition = jobPosition ?? "General",
                difficultyLevel = 2
            };

            // Cách 1: Gọi tuần tự (An toàn nhất cho Localhost Python đơn luồng)
            for (int i = 0; i < QuestionDifficult; i++)
            {
                try
                {
                    var response = await client.PostAsJsonAsync("https://0xs2f4db-5000.asse.devtunnels.ms/api/generate_question", payload);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var aiQuestionResponse = JsonSerializer.Deserialize<QuestionResponse>(json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (aiQuestionResponse != null && !string.IsNullOrWhiteSpace(aiQuestionResponse.QuestionText))
                        {
                            newQuestions.Add(new Question
                            {
                                QuestionText = aiQuestionResponse.QuestionText,
                                DifficultyLevel = aiQuestionResponse.DifficultyLevel ,
                                // Nếu có cột topic/industry trong DB thì gán luôn tại đây
                                // Topic = topic, 
                                // Industry = industry 
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi nhưng không dừng luồng, để cố gắng lấy các câu còn lại
                    Console.WriteLine($"Lỗi khi lấy câu hỏi thứ {i + 1}: {ex.Message}");
                }
            }

            // Kiểm tra nếu không lấy được câu nào
            if (newQuestions.Count == 0)
            {
                throw new Exception("AI Service failed to generate any questions.");
            }

            // Lưu tất cả vào DB một lần (Bulk Insert) => Tối ưu hiệu năng
            await _context.Questions.AddRangeAsync(newQuestions);
            await _context.SaveChangesAsync();

            return newQuestions;
        }

    }
}
