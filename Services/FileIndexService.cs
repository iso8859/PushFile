using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using PushFile.Models;

namespace PushFile.Services;

/// <summary>
/// Simple in-memory token mapping for uploaded files.
/// Registers a file path and returns a short token used in download links.
/// Note: in-memory mapping is lost on restart; consider persistent store for production.
/// </summary>
public class FileIndexService : IFileIndexService
{
 private readonly Dictionary<string, string> _map = new(); // token => fullpath
 private readonly FileUploadSettings _settings;

 public FileIndexService(IOptions<FileUploadSettings> settings)
 {
 _settings = settings.Value;
 }

 public string Register(string filePath)
 {
 // create a short token based on SHA256 of path + timestamp
 using var sha = SHA256.Create();
 var input = Encoding.UTF8.GetBytes(filePath + DateTime.UtcNow.Ticks);
 var hash = sha.ComputeHash(input);
 var token = Convert.ToBase64String(hash)[..8] // take first8 chars
 .Replace('+', '-')
 .Replace('/', '_');

 _map[token] = filePath;
 return token;
 }

 public string? GetFilePath(string token)
 {
 return _map.TryGetValue(token, out var path) ? path : null;
 }

 public IEnumerable<string> ListAllFiles()
 {
 if (!Directory.Exists(_settings.DestinationDirectory))
 yield break;

 foreach (var f in Directory.EnumerateFiles(_settings.DestinationDirectory))
 {
 yield return Path.GetFileName(f);
 }
 }
}
