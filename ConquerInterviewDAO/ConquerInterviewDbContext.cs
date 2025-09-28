using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<InterviewAnswer>(entity =>
        {
            entity.HasKey(e => e.interview_a_id).HasName("PRIMARY");

            entity.HasIndex(e => e.question_id, "fk_answers_question");

            entity.HasIndex(e => e.session_id, "fk_answers_session");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.text_answer).HasColumnType("text");
            entity.Property(e => e.video_url).HasMaxLength(500);
            entity.Property(e => e.voice_url).HasMaxLength(500);

            entity.HasOne(d => d.question).WithMany(p => p.InterviewAnswers)
                .HasForeignKey(d => d.question_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_answers_question");

            entity.HasOne(d => d.session).WithMany(p => p.InterviewAnswers)
                .HasForeignKey(d => d.session_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_answers_session");
        });

        modelBuilder.Entity<InterviewSession>(entity =>
        {
            entity.HasKey(e => e.session_id).HasName("PRIMARY");

            entity.HasIndex(e => e.user_id, "fk_interviews_user");

            entity.Property(e => e.end_time).HasColumnType("datetime");
            entity.Property(e => e.job_position).HasMaxLength(100);
            entity.Property(e => e.start_time)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.status).HasMaxLength(50);

            entity.HasOne(d => d.user).WithMany(p => p.InterviewSessions)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_interviews_user");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.order_id).HasName("PRIMARY");

            entity.HasIndex(e => e.plan_id, "fk_orders_plan");

            entity.HasIndex(e => e.user_id, "fk_orders_user");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.total_amount).HasPrecision(10, 2);

            entity.HasOne(d => d.plan).WithMany(p => p.Orders)
                .HasForeignKey(d => d.plan_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orders_plan");

            entity.HasOne(d => d.user).WithMany(p => p.Orders)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orders_user");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.payment_id).HasName("PRIMARY");

            entity.HasIndex(e => e.order_id, "fk_payments_order");

            entity.Property(e => e.amount).HasPrecision(10, 2);
            entity.Property(e => e.paid_at).HasColumnType("datetime");
            entity.Property(e => e.provider).HasMaxLength(100);
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.transaction_id).HasMaxLength(255);

            entity.HasOne(d => d.order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.order_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payments_order");
        });

        modelBuilder.Entity<Personalization>(entity =>
        {
            entity.HasKey(e => e.personalization_id).HasName("PRIMARY");

            entity.ToTable("Personalization");

            entity.HasIndex(e => e.customer_id, "fk_personal_customer");

            entity.HasIndex(e => e.report_q_id, "fk_personal_report");

            entity.Property(e => e.exercise).HasColumnType("text");
            entity.Property(e => e.name_practice).HasMaxLength(255);
            entity.Property(e => e.objective).HasColumnType("text");
            entity.Property(e => e.practice).HasColumnType("text");

            entity.HasOne(d => d.customer).WithMany(p => p.Personalizations)
                .HasForeignKey(d => d.customer_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_personal_customer");

            entity.HasOne(d => d.report_q).WithMany(p => p.Personalizations)
                .HasForeignKey(d => d.report_q_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_personal_report");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.question_id).HasName("PRIMARY");

            entity.Property(e => e.is_active).HasDefaultValueSql("'1'");
            entity.Property(e => e.question_text).HasColumnType("text");
        });

        modelBuilder.Entity<ReportQuestion>(entity =>
        {
            entity.HasKey(e => e.report_q_id).HasName("PRIMARY");

            entity.HasIndex(e => e.interview_a_id, "fk_report_answer");

            entity.HasIndex(e => e.customer_id, "fk_report_customer");

            entity.Property(e => e.answer_content_analysis).HasColumnType("text");
            entity.Property(e => e.comparison_with_other_candidates).HasColumnType("text");
            entity.Property(e => e.expertise_experience).HasColumnType("text");
            entity.Property(e => e.facial_expression).HasColumnType("text");
            entity.Property(e => e.overall_assessment).HasColumnType("text");
            entity.Property(e => e.problem_solving_skills).HasColumnType("text");
            entity.Property(e => e.response_duration_per_question).HasMaxLength(100);
            entity.Property(e => e.speaking_speed_clarity).HasColumnType("text");
            entity.Property(e => e.status).HasMaxLength(50);

            entity.HasOne(d => d.customer).WithMany(p => p.ReportQuestions)
                .HasForeignKey(d => d.customer_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_report_customer");

            entity.HasOne(d => d.interview_a).WithMany(p => p.ReportQuestions)
                .HasForeignKey(d => d.interview_a_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_report_answer");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.role_id).HasName("PRIMARY");

            entity.HasIndex(e => e.role_name, "role_name").IsUnique();

            entity.Property(e => e.role_name).HasMaxLength(100);
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.plan_id).HasName("PRIMARY");

            entity.Property(e => e.is_active).HasDefaultValueSql("'1'");
            entity.Property(e => e.plan_name).HasMaxLength(100);
            entity.Property(e => e.price).HasPrecision(10, 2);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("PRIMARY");

            entity.HasIndex(e => e.email, "email").IsUnique();

            entity.HasIndex(e => e.username, "username").IsUnique();

            entity.Property(e => e.avatar_url).HasMaxLength(500);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.full_name).HasMaxLength(255);
            entity.Property(e => e.gender).HasMaxLength(10);
            entity.Property(e => e.password_hash).HasMaxLength(255);
            entity.Property(e => e.phone_number).HasMaxLength(20);
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.username).HasMaxLength(100);

            entity.HasMany(d => d.roles).WithMany(p => p.users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("role_id")
                        .HasConstraintName("fk_userroles_role"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("user_id")
                        .HasConstraintName("fk_userroles_user"),
                    j =>
                    {
                        j.HasKey("user_id", "role_id")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("UserRoles");
                        j.HasIndex(new[] { "role_id" }, "fk_userroles_role");
                    });
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(e => e.subscription_id).HasName("PRIMARY");

            entity.HasIndex(e => e.plan_id, "fk_usersub_plan");

            entity.HasIndex(e => e.user_id, "fk_usersub_user");

            entity.Property(e => e.start_date).HasDefaultValueSql("curdate()");
            entity.Property(e => e.status).HasMaxLength(50);

            entity.HasOne(d => d.plan).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.plan_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usersub_plan");

            entity.HasOne(d => d.user).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usersub_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
