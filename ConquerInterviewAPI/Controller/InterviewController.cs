using ConquerInterviewBO.Models;
using ConquerInterviewServices.Implements;
using Microsoft.AspNetCore.Mvc;

namespace ConquerInterviewAPI.Controller
{
    //[ApiController]
    //[Route("api/interview")]
    //public class InterviewController : ControllerBase
    //{
    //    private readonly ConquerInterviewDbContext _context;
    //    private readonly AIService _aiService;

    //    public InterviewController(ConquerInterviewDbContext context, AIService aiService)
    //    {
    //        _context = context;
    //        _aiService = aiService;
    //    }

    //    // 🟢 1️⃣ Bắt đầu buổi phỏng vấn
    //    [HttpPost("start")]
    //    public async Task<IActionResult> StartInterview([FromBody] StartInterviewRequest req)
    //    {
    //        var aiResponse = await _aiService.StartInterviewAsync(req.Topic, req.Industry);
    //        if (aiResponse == null)
    //            return BadRequest("Không thể gọi AI service.");

    //        var session = new InterviewSession
    //        {
    //            UserId = req.UserId,
    //            StartTime = DateTime.UtcNow,
    //            JobPosition = req.Topic,
    //            Status = "InProgress"
    //        };
    //        _context.InterviewSessions.Add(session);
    //        await _context.SaveChangesAsync();

    //        var question = new Question
    //        {
    //            QuestionText = aiResponse.question,
    //            IsActive = true
    //        };
    //        _context.Questions.Add(question);
    //        await _context.SaveChangesAsync();

    //        return Ok(new
    //        {
    //            sessionId = session.SessionId,
    //            questionId = question.QuestionId,
    //            questionText = question.QuestionText,
    //            currentRound = aiResponse.current_round,
    //            totalRounds = aiResponse.total_rounds
    //        });
    //    }

    //    // 🟢 2️⃣ Gửi câu trả lời & nhận feedback/report từ AI
    //    [HttpPost("submit")]
    //    public async Task<IActionResult> SubmitRound([FromBody] SubmitRoundRequest req)
    //    {
    //        var aiResult = await _aiService.SubmitRoundAsync(req.Round, req.Answer);
    //        if (aiResult == null)
    //            return BadRequest("Không thể gọi AI service.");

    //        // Lưu câu trả lời
    //        var answer = new InterviewAnswer
    //        {
    //            SessionId = req.SessionId,
    //            QuestionId = req.QuestionId,
    //            TextAnswer = req.Answer,
    //            CreatedAt = DateTime.UtcNow
    //        };
    //        _context.InterviewAnswers.Add(answer);
    //        await _context.SaveChangesAsync();

    //        // Lưu báo cáo đánh giá (report)
    //        var report = new ReportQuestion
    //        {
    //            InterviewAId = answer.InterviewAId,
    //            CustomerId = req.UserId,
    //            OverallAssessment = aiResult.report,
    //            AnswerContentAnalysis = aiResult.feedback,
    //            ExpertiseExperience = aiResult.suggested_answer,
    //            Status = "Completed"
    //        };
    //        _context.ReportQuestions.Add(report);
    //        await _context.SaveChangesAsync();

    //        // Nếu AI báo kết thúc buổi phỏng vấn
    //        if (aiResult.status == "finished")
    //        {
    //            var session = await _context.InterviewSessions.FindAsync(req.SessionId);
    //            if (session != null)
    //            {
    //                session.Status = "Completed";
    //                session.EndTime = DateTime.UtcNow;
    //                await _context.SaveChangesAsync();
    //            }
    //        }

    //        return Ok(aiResult);
    //    }
    //}

    //// 🧾 DTO Request
    //public class StartInterviewRequest
    //{
    //    public int UserId { get; set; }
    //    public string Topic { get; set; } = null!;
    //    public string Industry { get; set; } = null!;
    //}

    //public class SubmitRoundRequest
    //{
    //    public int SessionId { get; set; }
    //    public int QuestionId { get; set; }
    //    public int UserId { get; set; }
    //    public int Round { get; set; }
    //    public string Answer { get; set; } = null!;
    //}
}
