using ConquerInterviewBO.DTOs.Requests; // Cần dùng ReportDTO
using ConquerInterviewBO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json; // Cần để dùng PostAsJsonAsync
using System.Text.Json; // Cần để dùng JsonSerializer
using System.Threading.Tasks;

namespace ConquerInterviewDAO
{
    public class PersonalizationDAO
    {
        private readonly ConquerInterviewDbContext _context;
        private static PersonalizationDAO instance;

        public static PersonalizationDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PersonalizationDAO();
                }
                return instance;
            }
        }

        public PersonalizationDAO()
        {
            _context = new ConquerInterviewDbContext();
        }
        public async Task<bool> CheckReportQuestionExistsAsync(int reportQId)
        {
            // Kiểm tra trong bảng ReportQuestions xem ID có tồn tại không
            // Lưu ý: Đảm bảo DbContext của bạn có DbSet<ReportQuestion> ReportQuestions
            return await _context.ReportQuestions.AnyAsync(rq => rq.ReportQId == reportQId);
        }
        // 1. Lấy lộ trình từ DB
        public async Task<List<Personalization>> GetByUserIdAsync(int customerId)
        {
            return await _context.Personalizations
                .Where(p => p.CustomerId == customerId)
                .OrderBy(p => p.PersonalizationId)
                .ToListAsync();
        }

        public async Task<List<Personalization>> GetByCustomerIdAsync(int customerId)
        {
            // Giả định tên DbSet là Personalizations
            return await _context.Personalizations

                // 1. Tải ReportQuestion Entity (Liên kết trực tiếp)
                .Include(p => p.ReportQ)
                    // 2. Tải InterviewAnswer (Answer) từ ReportQuestion
                    // GIẢ ĐỊNH: ReportQuestion có Navigation Property 'InterviewA'
                    .ThenInclude(r => r.InterviewA)
                        // 3. Tải Question từ InterviewAnswer
                        .ThenInclude(a => a.Question) // <-- Cần cho QuestionText

                // 4. Tải InterviewSession (Session) từ InterviewAnswer
                .Include(p => p.ReportQ) // Bắt đầu Include lại từ ReportQ
                    .ThenInclude(r => r.InterviewA)
                        .ThenInclude(a => a.Session) // <-- Cần cho JobPosition/StartTime

                // Lọc theo CustomerId (Đã sửa từ userId sang customerId để khớp với hàm)
                .Where(p => p.CustomerId == customerId)

                .ToListAsync();
        }

        // 2. Lưu các bước lộ trình mới vào DB
        public async Task<List<Personalization>> SavePersonalizationStepsAsync(List<Personalization> steps)
        {
            await _context.Personalizations.AddRangeAsync(steps);
            await _context.SaveChangesAsync();
            return steps;
        }

        // 3. Gọi API Python để tạo lộ trình
        public async Task<PythonPersonalizationResponse?> CallAIPersonalizationAsync(List<ReportDTO> reports)
        {
            using var client = new HttpClient();

            // Chuẩn bị payload cho Python API
            var payload = new { reports = reports };
            var response = await client.PostAsJsonAsync("https://0xs2f4db-5000.asse.devtunnels.ms/api/personalization", payload);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[ERROR] AI Personalization service returned {response.StatusCode}. Content: {errorContent}");
                throw new Exception($"AI service returned {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("📄 AI Personalization JSON: " + json);

            // Deserialize kết quả từ Python
            return JsonSerializer.Deserialize<PythonPersonalizationResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<Personalization>> CallAIPersonalizationAndSaveAsync(int customerId, List<ReportDTO> reports, int representativeReportQId)
        {
            // 1. Gọi Python AI
            var aiResponse = await CallAIPersonalizationAsync(reports);

            if (aiResponse?.PersonalizedPath == null || !aiResponse.PersonalizedPath.Any())
            {
                throw new Exception("AI did not return a valid personalization path.");
            }

            // 2. Chuyển đổi DTO từ AI sang Model của DB (Lưu tất cả bước)
            var dbStepsList = aiResponse.PersonalizedPath.Select(step => new Personalization
            {
                NamePractice = step.NamePractice,
                Practice = step.Practice,
                Exercise = step.Exercise,
                Objective = step.Objective,
                CustomerId = customerId,
                // Gán ReportQId của câu hỏi đại diện (câu đầu tiên)
                // Lưu ý: Nếu bạn muốn lưu nhiều ReportQId, cấu trúc DB cần thay đổi
                ReportQId = representativeReportQId
            }).ToList();

            // 3. Lưu danh sách các bước này vào DB
            return await PersonalizationDAO.Instance.SavePersonalizationStepsAsync(dbStepsList);
        }
    }

    // --- Lớp DTO nội bộ để nhận dữ liệu từ Python ---

    // Lớp này khớp với JSON trả về từ Python: { "personalizedPath": [...] }
    public class PythonPersonalizationResponse
    {
        public List<PersonalizationStepDTO>? PersonalizedPath { get; set; }
    }

    // Lớp này khớp với các đối tượng trong mảng "personalizedPath"
    public class PersonalizationStepDTO
    {
        public string? NamePractice { get; set; }
        public string? Practice { get; set; }
        public string? Exercise { get; set; }
        public string? Objective { get; set; }
    }
}