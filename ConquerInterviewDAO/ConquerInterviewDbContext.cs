using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.Models;

public partial class ConquerInterviewDbContext : DbContext
{
    public ConquerInterviewDbContext()
    {
    }

    public ConquerInterviewDbContext(DbContextOptions<ConquerInterviewDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<InterviewAnswer> InterviewAnswers { get; set; }

    public virtual DbSet<InterviewSession> InterviewSessions { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Personalization> Personalizations { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<ReportQuestion> ReportQuestions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) // chỉ cấu hình nếu chưa được DI config
        {
            // đọc appsettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // AppDomain.CurrentDomain.BaseDirectory cũng được
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            optionsBuilder
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<InterviewAnswer>(entity =>
        {
            entity.HasKey(e => e.InterviewAId).HasName("PRIMARY");

            entity.HasIndex(e => e.QuestionId, "fk_answers_question");

            entity.HasIndex(e => e.SessionId, "fk_answers_session");

            entity.Property(e => e.InterviewAId).HasColumnName("interview_a_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.TextAnswer)
                .HasColumnType("text")
                .HasColumnName("text_answer");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(500)
                .HasColumnName("video_url");
            entity.Property(e => e.VoiceUrl)
                .HasMaxLength(500)
                .HasColumnName("voice_url");

            entity.HasOne(d => d.Question).WithMany(p => p.InterviewAnswers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_answers_question");

            entity.HasOne(d => d.Session).WithMany(p => p.InterviewAnswers)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_answers_session");
        });

        modelBuilder.Entity<InterviewSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PRIMARY");

            entity.HasIndex(e => e.UserId, "fk_interviews_user");

            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.JobPosition)
                .HasMaxLength(100)
                .HasColumnName("job_position");
            entity.Property(e => e.StartTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.InterviewSessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_interviews_user");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.HasIndex(e => e.PlanId, "fk_orders_plan");

            entity.HasIndex(e => e.UserId, "fk_orders_user");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PlanId).HasColumnName("plan_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Plan).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orders_plan");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orders_user");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PRIMARY");

            entity.HasIndex(e => e.OrderId, "fk_payments_order");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaidAt)
                .HasColumnType("datetime")
                .HasColumnName("paid_at");
            entity.Property(e => e.Provider)
                .HasMaxLength(100)
                .HasColumnName("provider");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(255)
                .HasColumnName("transaction_id");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payments_order");
        });

        modelBuilder.Entity<Personalization>(entity =>
        {
            entity.HasKey(e => e.PersonalizationId).HasName("PRIMARY");

            entity.ToTable("Personalization");

            entity.HasIndex(e => e.CustomerId, "fk_personal_customer");

            entity.HasIndex(e => e.ReportQId, "fk_personal_report");

            entity.Property(e => e.PersonalizationId).HasColumnName("personalization_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Exercise)
                .HasColumnType("text")
                .HasColumnName("exercise");
            entity.Property(e => e.NamePractice)
                .HasMaxLength(255)
                .HasColumnName("name_practice");
            entity.Property(e => e.Objective)
                .HasColumnType("text")
                .HasColumnName("objective");
            entity.Property(e => e.Practice)
                .HasColumnType("text")
                .HasColumnName("practice");
            entity.Property(e => e.ReportQId).HasColumnName("report_q_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.Personalizations)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_personal_customer");

            entity.HasOne(d => d.ReportQ).WithMany(p => p.Personalizations)
                .HasForeignKey(d => d.ReportQId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_personal_report");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PRIMARY");

            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.DifficultyLevel).HasColumnName("difficulty_level");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.QuestionText)
                .HasColumnType("text")
                .HasColumnName("question_text");
        });

        modelBuilder.Entity<ReportQuestion>(entity =>
        {
            entity.HasKey(e => e.ReportQId).HasName("PRIMARY");

            entity.HasIndex(e => e.InterviewAId, "fk_report_answer");

            entity.HasIndex(e => e.CustomerId, "fk_report_customer");

            entity.Property(e => e.ReportQId).HasColumnName("report_q_id");
            entity.Property(e => e.AnswerContentAnalysis)
                .HasColumnType("text")
                .HasColumnName("answer_content_analysis");
            entity.Property(e => e.ComparisonWithOtherCandidates)
                .HasColumnType("text")
                .HasColumnName("comparison_with_other_candidates");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.ExpertiseExperience)
                .HasColumnType("text")
                .HasColumnName("expertise_experience");
            entity.Property(e => e.FacialExpression)
                .HasColumnType("text")
                .HasColumnName("facial_expression");
            entity.Property(e => e.InterviewAId).HasColumnName("interview_a_id");
            entity.Property(e => e.OverallAssessment)
                .HasColumnType("text")
                .HasColumnName("overall_assessment");
            entity.Property(e => e.ProblemSolvingSkills)
                .HasColumnType("text")
                .HasColumnName("problem_solving_skills");
            entity.Property(e => e.ResponseDurationPerQuestion)
                .HasMaxLength(100)
                .HasColumnName("response_duration_per_question");
            entity.Property(e => e.SpeakingSpeedClarity)
                .HasColumnType("text")
                .HasColumnName("speaking_speed_clarity");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.Customer).WithMany(p => p.ReportQuestions)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_report_customer");

            entity.HasOne(d => d.InterviewA).WithMany(p => p.ReportQuestions)
                .HasForeignKey(d => d.InterviewAId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_report_answer");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PRIMARY");

            entity.HasIndex(e => e.RoleName, "role_name").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PRIMARY");

            entity.Property(e => e.PlanId).HasColumnName("plan_id");
            entity.Property(e => e.DurationDays).HasColumnName("duration_days");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.PlanName)
                .HasMaxLength(100)
                .HasColumnName("plan_name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.Username, "username").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500)
                .HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.ResetToken)
                .HasMaxLength(255)
                .HasColumnName("reset_token");
            entity.Property(e => e.ResetTokenExpiry)
                .HasColumnType("datetime")
                .HasColumnName("reset_token_expiry");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .HasColumnName("token");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.TrialCount)
                .HasDefaultValueSql("3") // Giá trị mặc định trong DB
                .HasColumnName("TrialCount");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_userroles_role"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_userroles_user"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("UserRoles");
                        j.HasIndex(new[] { "RoleId" }, "fk_userroles_role");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                    });
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PRIMARY");

            entity.HasIndex(e => e.PlanId, "fk_usersub_plan");

            entity.HasIndex(e => e.UserId, "fk_usersub_user");

            entity.Property(e => e.SubscriptionId).HasColumnName("subscription_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.PlanId).HasColumnName("plan_id");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("curdate()")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Plan).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usersub_plan");

            entity.HasOne(d => d.User).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usersub_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
