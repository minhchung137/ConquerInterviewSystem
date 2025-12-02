using ConquerInterviewServices.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace ConquerInterviewServices.Implements
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly string _templatePath;

        public EmailService(IConfiguration config)
        {
            _config = config;
            _templatePath = Path.Combine(AppContext.BaseDirectory, "EmailTemplates");
        }

        public string LoadTemplate(string templateName)
        {
            var filePath = Path.Combine(_templatePath, templateName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Email template not found: {filePath}");

            return File.ReadAllText(filePath);
        }

        public void SendEmail(string to, string subject, string body, bool isHtml = false)
        {
            try
            {
                var mail = new MailMessage
                {
                    From = new MailAddress(_config["Email:From"], "Conquer Interview"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mail.To.Add(to);

                var smtp = new SmtpClient(_config["Email:Smtp"], int.Parse(_config["Email:Port"]))
                {
                    Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"]),
                    EnableSsl = true
                };

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] {ex.Message}");
                throw;
            }
        }
    }
}
