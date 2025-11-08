using Microsoft.AspNetCore.Components.Forms;

namespace PushFile.Services;

/// <summary>
/// Interface for file upload operations
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// Saves the uploaded file to the configured destination directory
    /// </summary>
    /// <param name="file">The file to save</param>
    /// <returns>The full path where the file was saved</returns>
    Task<string> SaveFileAsync(IBrowserFile file);
}
