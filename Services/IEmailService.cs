namespace PushFile.Services;

/// <summary>
/// Interface for email operations
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a notification email when a file is uploaded
    /// </summary>
    /// <param name="fileName">Name of the uploaded file</param>
    /// <param name="fileSize">Size of the uploaded file in bytes</param>
    /// <param name="filePath">Full path where the file was saved</param>
    Task SendFileUploadNotificationAsync(string fileName, long fileSize, string filePath);

    /// <summary>
    /// Sends an email containing a direct download link for the uploaded file.
    /// </summary>
    Task SendFileDownloadLinkAsync(string fileName, string downloadUrl);
}
