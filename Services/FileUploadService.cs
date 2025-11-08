using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using PushFile.Models;

namespace PushFile.Services;

/// <summary>
/// Service for handling file upload operations
/// </summary>
public class FileUploadService : IFileUploadService
{
    private readonly FileUploadSettings _settings;
    private readonly ILogger<FileUploadService> _logger;

    public FileUploadService(
        IOptions<FileUploadSettings> settings,
        ILogger<FileUploadService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(IBrowserFile file)
    {
        try
        {
            // Ensure destination directory exists
            if (!Directory.Exists(_settings.DestinationDirectory))
            {
                Directory.CreateDirectory(_settings.DestinationDirectory);
                _logger.LogInformation("Created destination directory: {Directory}", _settings.DestinationDirectory);
            }

            // Generate a unique filename to avoid conflicts
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"{timestamp}_{file.Name}";
            var filePath = Path.Combine(_settings.DestinationDirectory, fileName);

            // Save the file
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.OpenReadStream(maxAllowedSize: _settings.MaxFileSizeMB * 1024 * 1024)
                .CopyToAsync(fileStream);

            _logger.LogInformation("File saved successfully: {FilePath}", filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file: {FileName}", file.Name);
            throw;
        }
    }
}
