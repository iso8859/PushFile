# API Documentation

## Services

### IFileUploadService

Interface for file upload operations.

#### Methods

##### SaveFileAsync
```csharp
Task<string> SaveFileAsync(IBrowserFile file)
```

Saves the uploaded file to the configured destination directory.

**Parameters:**
- `file`: The file to save (from Blazor InputFile component)

**Returns:**
- `Task<string>`: The full path where the file was saved

**Throws:**
- `Exception`: If file cannot be saved (directory issues, permissions, etc.)

**Implementation Details:**
- Creates destination directory if it doesn't exist
- Generates unique filename using timestamp prefix: `yyyyMMdd_HHmmss_originalname.ext`
- Respects `MaxFileSizeMB` setting from configuration
- Logs operations and errors

---

### IEmailService

Interface for email notification operations.

#### Methods

##### SendFileUploadNotificationAsync
```csharp
Task SendFileUploadNotificationAsync(string fileName, long fileSize, string filePath)
```

Sends a notification email when a file is uploaded.

**Parameters:**
- `fileName`: Name of the uploaded file
- `fileSize`: Size of the uploaded file in bytes
- `filePath`: Full path where the file was saved

**Returns:**
- `Task`: Async operation

**Throws:**
- `Exception`: If email cannot be sent (SMTP connection issues, authentication, etc.)

**Email Format:**
- Subject: `New File Upload: {fileName}`
- Content: HTML and plain text versions
- Includes: file name, formatted size, upload timestamp, file location

---

## Configuration Models

### FileUploadSettings

Configuration for file upload behavior.

```csharp
public class FileUploadSettings
{
    public string DestinationDirectory { get; set; }  // Required
    public int MaxFileSizeMB { get; set; }            // Default: 100
    public List<string> AllowedExtensions { get; set; } // Default: empty (all allowed)
}
```

**Properties:**

- `DestinationDirectory`: Absolute path where uploaded files are saved
  - Example: `"C:\\Uploads"`
  - Directory will be created if it doesn't exist

- `MaxFileSizeMB`: Maximum file size in megabytes
  - Used for validation in UI
  - Also enforced in FileUploadService

- `AllowedExtensions`: List of allowed file extensions
  - Include the dot: `[".pdf", ".docx"]`
  - Empty list allows all extensions
  - Case-insensitive matching

---

### EmailSettings

Configuration for SMTP email notifications.

```csharp
public class EmailSettings
{
    public string SmtpServer { get; set; }       // Required
    public int SmtpPort { get; set; }            // Default: 587
    public string SmtpUsername { get; set; }     // Required for auth
    public string SmtpPassword { get; set; }     // Required for auth
    public bool EnableSsl { get; set; }          // Default: true
    public string FromEmail { get; set; }        // Required
    public string FromName { get; set; }         // Required
    public string NotificationEmail { get; set; } // Required
}
```

**Properties:**

- `SmtpServer`: SMTP server hostname
  - Example: `"smtp.gmail.com"`

- `SmtpPort`: SMTP server port
  - Common ports: 587 (TLS), 465 (SSL), 25 (unsecured)

- `SmtpUsername`: Username for SMTP authentication
  - Can be empty if server doesn't require auth

- `SmtpPassword`: Password for SMTP authentication
  - Use app-specific passwords for Gmail
  - Store securely (user secrets, environment variables)

- `EnableSsl`: Whether to use SSL/TLS
  - Recommended: `true`

- `FromEmail`: Sender email address
  - Must be valid email format

- `FromName`: Sender display name
  - Example: `"File Upload System"`

- `NotificationEmail`: Recipient email address
  - Where notifications are sent

---

### PageSettings

Configuration for page customization.

```csharp
public class PageSettings
{
    public string Title { get; set; }           // Default: "File Upload Portal"
    public string WelcomeMessage { get; set; }  // Default: "Upload your files securely"
}
```

**Properties:**

- `Title`: Page title shown in browser tab and header
- `WelcomeMessage`: Welcome message shown on upload page

---

## Blazor Components

### Index.razor

Main file upload page.

**Route:** `/` or `/upload`

**Injected Services:**
- `IFileUploadService`: For saving files
- `IEmailService`: For sending notifications
- `IOptions<FileUploadSettings>`: Configuration
- `IOptions<PageSettings>`: Page customization

**Component State:**

```csharp
private IBrowserFile? selectedFile;      // Currently selected file
private string? errorMessage;            // Error message to display
private string? successMessage;          // Success message to display
private bool isUploading;               // Upload in progress flag
```

**Methods:**

- `HandleFileSelected(InputFileChangeEventArgs e)`: Called when user selects a file
- `UploadFile()`: Validates, uploads file, and sends email
- `FormatFileSize(long bytes)`: Formats bytes to human-readable size

**Validation:**
1. File size check against `MaxFileSizeMB`
2. Extension check against `AllowedExtensions`

**User Feedback:**
- Bootstrap alerts for errors/success
- Spinner during upload
- File details display
- Disabled button states

---

## Dependency Injection

Services are registered in `Program.cs`:

```csharp
// Configuration
builder.Services.Configure<FileUploadSettings>(
    builder.Configuration.GetSection("FileUpload"));
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Email"));
builder.Services.Configure<PageSettings>(
    builder.Configuration.GetSection("PageSettings"));

// Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
```

**Service Lifetimes:**
- `Scoped`: New instance per Blazor circuit (user connection)

---

## Error Handling

### FileUploadService
- Logs errors with `ILogger<FileUploadService>`
- Throws exceptions up to component
- Creates directories automatically

### EmailService
- Logs errors with `ILogger<EmailService>`
- Throws exceptions up to component
- Handles SMTP authentication failures

### Index.razor Component
- Try-catch around upload operations
- Displays user-friendly error messages
- Resets upload state on error

---

## Security Considerations

### Input Validation
- File size limits prevent DoS
- Extension whitelist prevents malicious files
- Filename sanitization via timestamp prefix

### Configuration
- Passwords in appsettings.json (development only)
- Use User Secrets for development
- Use environment variables for production

### File Storage
- Files saved outside wwwroot (not web-accessible)
- Unique filenames prevent overwrites
- Directory permissions should be restricted

### Email
- SSL/TLS encryption recommended
- No file attachments (security by design)
- Only metadata sent in email

---

## Logging

All services use `ILogger<T>` for structured logging.

**Log Levels:**
- `Information`: Successful operations
- `Error`: Exceptions and failures

**Logged Events:**
- Directory creation
- File saves with paths
- Email sends
- All exceptions

**Example:**
```csharp
_logger.LogInformation("File saved successfully: {FilePath}", filePath);
_logger.LogError(ex, "Error sending email notification for file: {FileName}", fileName);
```

---

## Extension Points

### Custom Validators
Create a validator service to add business logic:

```csharp
public interface IFileValidator
{
    Task<ValidationResult> ValidateAsync(IBrowserFile file);
}
```

### Custom Email Templates
Override `BuildEmailBody()` in EmailService or create a template service:

```csharp
public interface IEmailTemplateService
{
    string BuildTemplate(EmailData data);
}
```

### File Processing
Add a processing pipeline after upload:

```csharp
public interface IFileProcessor
{
    Task ProcessAsync(string filePath);
}
```

### Audit Logging
Create an audit service to track all uploads:

```csharp
public interface IAuditService
{
    Task LogUploadAsync(string fileName, string user, DateTime timestamp);
}
```
