namespace PushFile.Models;

/// <summary>
/// Configuration settings for file uploads
/// </summary>
public class FileUploadSettings
{
    public string DestinationDirectory { get; set; } = string.Empty;
    public int MaxFileSizeMB { get; set; } = 100;
    public List<string> AllowedExtensions { get; set; } = new();
}
