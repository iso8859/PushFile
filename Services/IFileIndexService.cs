namespace PushFile.Services;

public interface IFileIndexService
{
 /// <summary>
 /// Registers a file path and returns a short token to be used in download links.
 /// </summary>
 string Register(string filePath);

 /// <summary>
 /// Returns the full file path for the given token, or null if not found.
 /// </summary>
 string? GetFilePath(string token);

 /// <summary>
 /// Returns all files in the upload directory (file names).
 /// </summary>
 IEnumerable<string> ListAllFiles();
}
