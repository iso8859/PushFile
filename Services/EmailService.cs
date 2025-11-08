using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PushFile.Models;

namespace PushFile.Services;

/// <summary>
/// Service for sending email notifications
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> settings,
        ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendFileUploadNotificationAsync(string fileName, long fileSize, string filePath)
    {
        // This method remains but will not include the direct link — use SendFileDownloadLinkAsync to send links.
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress("", _settings.NotificationEmail));
            message.Subject = $"New File Upload: {fileName}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = BuildEmailBody(fileName, fileSize, filePath),
                TextBody = BuildTextEmailBody(fileName, fileSize, filePath)
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            // Connect to SMTP server
            await client.ConnectAsync(
                _settings.SmtpServer,
                _settings.SmtpPort,
                _settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None
            );

            // Authenticate
            if (!string.IsNullOrEmpty(_settings.SmtpUsername))
            {
                if (_settings.SmtpPassword.StartsWith("%") && _settings.SmtpPassword.EndsWith("%"))
                {
                    var envVarName = _settings.SmtpPassword.Trim('%');
                    var envVarValue = Environment.GetEnvironmentVariable(envVarName);
                    if (string.IsNullOrEmpty(envVarValue))
                    {
                        throw new InvalidOperationException($"Environment variable '{envVarName}' is not set.");
                    }
                    await client.AuthenticateAsync(_settings.SmtpUsername, envVarValue);
                }
                else
                {
                    await client.AuthenticateAsync(_settings.SmtpUsername, _settings.SmtpPassword);
                }
            }

            // Send the email
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email notification sent successfully for file: {FileName}", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email notification for file: {FileName}", fileName);
            throw;
        }
    }

    public async Task SendFileDownloadLinkAsync(string fileName, string downloadUrl)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, Environment.ExpandEnvironmentVariables(_settings.FromEmail)));
            message.To.Add(new MailboxAddress("", Environment.ExpandEnvironmentVariables(_settings.NotificationEmail)));
            message.Subject = $"Download link for: {fileName}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p>A file has been uploaded. <a href=\"{downloadUrl}\">Download it here</a>.</p>",
                TextBody = $"A file has been uploaded. Download it here: {downloadUrl}"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            // Connect to SMTP server
            await client.ConnectAsync(
                Environment.ExpandEnvironmentVariables(_settings.SmtpServer),
                _settings.SmtpPort,
                _settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None
            );

            await client.AuthenticateAsync(Environment.ExpandEnvironmentVariables(_settings.SmtpUsername), Environment.ExpandEnvironmentVariables(_settings.SmtpPassword));

            // Send the email
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Download link email sent for file: {FileName}", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending download link email for file: {FileName}", fileName);
            throw;
        }
    }

    private string BuildEmailBody(string fileName, long fileSize, string filePath)
    {
        var fileSizeFormatted = FormatFileSize(fileSize);
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #0d6efd; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f8f9fa; padding: 20px; margin-top: 20px; }}
        .info-row {{ padding: 10px 0; border-bottom: 1px solid #dee2e6; }}
        .label {{ font-weight: bold; color: #495057; }}
        .value {{ color: #212529; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>New File Upload Notification</h2>
        </div>
        <div class='content'>
            <p>A new file has been uploaded to the system.</p>
            <div class='info-row'>
                <span class='label'>File Name:</span>
                <span class='value'>{fileName}</span>
            </div>
            <div class='info-row'>
                <span class='label'>File Size:</span>
                <span class='value'>{fileSizeFormatted}</span>
            </div>
            <div class='info-row'>
                <span class='label'>Upload Time:</span>
                <span class='value'>{timestamp}</span>
            </div>
            <div class='info-row'>
                <span class='label'>File Location:</span>
                <span class='value'>{filePath}</span>
            </div>
        </div>
    </div>
</body>
</html>";
    }

    private string BuildTextEmailBody(string fileName, long fileSize, string filePath)
    {
        var fileSizeFormatted = FormatFileSize(fileSize);
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        return $@"
New File Upload Notification
=============================

A new file has been uploaded to the system.

File Name: {fileName}
File Size: {fileSizeFormatted}
Upload Time: {timestamp}
File Location: {filePath}
";
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
