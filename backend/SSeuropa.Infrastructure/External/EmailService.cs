using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SSeuropa.Application.Interfaces;

namespace SSeuropa.Infrastructure.External;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendStaffInvitationAsync(string toEmail, string recipientName, string role, string temporaryPassword, CancellationToken ct = default)
    {
        var subject = $"Welcome to SS Europa Management – Your {role} Account";
        var body = $@"Hello {recipientName},

You have been granted access to the SS Europa Management Portal (https://admin.sseuropa.com) as a {role}.

Your Login Credentials:
Email: {toEmail}
Temporary Password: {temporaryPassword}
Login URL: https://admin.sseuropa.com/login

Please sign in and change your password immediately upon your first login.

Best regards,
The SS Europa Academic & Technical Team
https://sseuropa.com";

        await SendEmailAsync(toEmail, subject, body, ct);
    }

    public async Task SendPasswordResetCodeAsync(string toEmail, string code, CancellationToken ct = default)
    {
        var subject = "SS Europa – Password Reset Verification Code";
        var body = $@"Hello,

A request was received to reset your password for your SS Europa account.

Your 6-digit verification code is: {code}

This code will expire in 15 minutes. If you did not make this request, please ignore this email.

SS Europa Security Team";

        await SendEmailAsync(toEmail, subject, body, ct);
    }

    public async Task SendVerificationOtpAsync(string toEmail, string studentName, string code, CancellationToken ct = default)
    {
        var subject = "Your Verification Code – SS Europa";
        var body = $@"Hello {studentName},

Your 6-digit verification code for SS Europa is:

{code}

This code will expire in 10 minutes. If you did not make this request, please ignore this email.

SS Europa Security Team";

        await SendEmailAsync(toEmail, subject, body, ct);
    }

    public async Task SendAssignmentFeedbackAsync(string toEmail, string studentName, string assignmentTitle, int marks, string? feedback, CancellationToken ct = default)
    {
        var subject = $"Assignment Graded: {assignmentTitle} – SS Europa";
        var body = $@"Hello {studentName},

Your assignment '{assignmentTitle}' has been reviewed by your mentor.

Marks Awarded: {marks} / 100
Mentor Feedback: {feedback ?? "Good work!"}

You can view details in your SS Europa Mobile App.

SS Europa Academic Team";

        await SendEmailAsync(toEmail, subject, body, ct);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken ct)
    {
        var smtpHost = _config["Email:SmtpHost"]
            ?? _config["SMTP_HOST"]
            ?? Environment.GetEnvironmentVariable("SMTP_HOST");
        var smtpPortStr = _config["Email:SmtpPort"]
            ?? _config["SMTP_PORT"]
            ?? Environment.GetEnvironmentVariable("SMTP_PORT");
        var smtpPort = int.TryParse(smtpPortStr, out var port) ? port : 587;
        var smtpUser = _config["Email:SmtpUser"]
            ?? _config["SMTP_USER"]
            ?? Environment.GetEnvironmentVariable("SMTP_USER");
        var smtpPass = _config["Email:SmtpPassword"]
            ?? _config["SMTP_PASSWORD"]
            ?? Environment.GetEnvironmentVariable("SMTP_PASSWORD");
        var fromAddress = _config["Email:FromAddress"]
            ?? _config["SMTP_FROM_ADDRESS"]
            ?? Environment.GetEnvironmentVariable("SMTP_FROM_ADDRESS")
            ?? "noreply@sseuropa.com";
        var fromName = _config["Email:FromName"]
            ?? _config["SMTP_FROM_NAME"]
            ?? Environment.GetEnvironmentVariable("SMTP_FROM_NAME")
            ?? "SS Europa";

        var isConfigured = !string.IsNullOrWhiteSpace(smtpHost) &&
                           !string.IsNullOrWhiteSpace(smtpUser) &&
                           !string.IsNullOrWhiteSpace(smtpPass) &&
                           !smtpPass.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase);

        if (isConfigured)
        {
            try
            {
                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var htmlBody = $@"
<div style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 0 auto; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 10px; overflow: hidden;"">
  <div style=""background: #0D5C75; padding: 24px; text-align: center;"">
    <h1 style=""color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 1px;"">SS EUROPA</h1>
    <p style=""color: #dbeafe; margin: 4px 0 0; font-size: 13px;"">Academic & Student Learning Platform</p>
  </div>
  <div style=""padding: 28px; color: #1e293b; line-height: 1.6; font-size: 15px;"">
    {body.Replace("\r\n", "<br/>").Replace("\n", "<br/>")}
  </div>
  <div style=""background: #f8fafc; padding: 16px; text-align: center; font-size: 12px; color: #64748b; border-top: 1px solid #e2e8f0;"">
    © 2026 SS Europa. All rights reserved. • <a href=""https://sseuropa.com"" style=""color: #0D5C75; text-decoration: none;"">sseuropa.com</a>
  </div>
</div>";

                using var mail = new MailMessage
                {
                    From = new MailAddress(fromAddress, fromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                mail.To.Add(toEmail);

                await client.SendMailAsync(mail, ct);
                _logger.LogInformation("Sent live email via SMTP {Host}:{Port} to {ToEmail} with subject '{Subject}'.", smtpHost, smtpPort, toEmail, subject);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email via SMTP to {ToEmail}. Falling back to log-only output.", toEmail);
            }
        }

        // Development / unconfigured SMTP fallback:
        _logger.LogInformation("==================================================\n" +
                               "[SIMULATED EMAIL DISPATCH]\n" +
                               "To: {ToEmail}\n" +
                               "Subject: {Subject}\n" +
                               "Body:\n{Body}\n" +
                               "==================================================",
                               toEmail, subject, body);
    }
}

