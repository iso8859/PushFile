# Development Guide

This guide is for developers working on the PushFile project.

## Project Overview

**Type:** ASP.NET Core 8.0 Blazor Server Application  
**Purpose:** File upload portal with email notifications  
**Language:** C# 12 with nullable reference types  
**UI Framework:** Bootstrap 5.3  

## Architecture

### Layered Architecture

```
┌─────────────────────────────────────┐
│         Presentation Layer          │
│         (Blazor Components)         │
│           Pages/Index.razor         │
└─────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────┐
│         Service Layer               │
│    FileUploadService, EmailService  │
└─────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────┐
│      Infrastructure Layer           │
│   File System, SMTP (MailKit)       │
└─────────────────────────────────────┘
```

### Key Design Patterns

1. **Dependency Injection**: All services registered in `Program.cs`
2. **Options Pattern**: Configuration via `IOptions<T>`
3. **Interface Segregation**: Separate interfaces for each service
4. **Repository Pattern**: Services abstract infrastructure concerns

## Development Environment Setup

### Required Tools

- .NET 8.0 SDK
- Visual Studio Code
- C# Dev Kit extension
- Git

### Optional Tools

- SQL Server (for future database features)
- Azure CLI (for deployment)
- Docker (for containerization)

### VS Code Configuration

The project includes complete VS Code configuration:

- **`.vscode/settings.json`**: Editor settings, formatters
- **`.vscode/launch.json`**: Debug configurations
- **`.vscode/tasks.json`**: Build, run, test tasks
- **`.vscode/extensions.json`**: Recommended extensions

## Coding Standards

### C# Style Guide

1. **Naming Conventions**
   - Classes: PascalCase
   - Methods: PascalCase
   - Private fields: _camelCase
   - Local variables: camelCase
   - Constants: PascalCase

2. **File Organization**
   - One class per file
   - File name matches class name
   - Related classes in same folder

3. **Documentation**
   - XML comments for public APIs
   - Summary and parameter descriptions
   - Example usage for complex methods

4. **Null Handling**
   - Use nullable reference types (`string?`)
   - Check for null before use
   - Use null-conditional operators (`?.`, `??`)

### Blazor Component Guidelines

1. **Code-Behind vs Inline**
   - Use `@code` block for simple logic
   - Create separate `.razor.cs` for complex components

2. **Component Structure**
   ```razor
   @page "/route"
   
   <!-- HTML markup -->
   
   @code {
       // Injected services
       // Fields
       // Properties
       // Lifecycle methods
       // Event handlers
       // Helper methods
   }
   ```

3. **State Management**
   - Use component state for UI-only state
   - Consider state containers for shared state

## Common Development Tasks

### Adding a New Configuration Setting

1. **Update Model**
   ```csharp
   // Models/FileUploadSettings.cs
   public string NewSetting { get; set; } = "default";
   ```

2. **Update appsettings.json**
   ```json
   "FileUpload": {
     "NewSetting": "value"
   }
   ```

3. **Use in Code**
   ```csharp
   [Inject] private IOptions<FileUploadSettings> Settings { get; set; }
   
   var value = Settings.Value.NewSetting;
   ```

### Adding a New Service

1. **Create Interface**
   ```csharp
   // Services/IMyService.cs
   public interface IMyService
   {
       Task DoSomethingAsync(string input);
   }
   ```

2. **Implement Service**
   ```csharp
   // Services/MyService.cs
   public class MyService : IMyService
   {
       private readonly ILogger<MyService> _logger;
       
       public MyService(ILogger<MyService> logger)
       {
           _logger = logger;
       }
       
       public async Task DoSomethingAsync(string input)
       {
           try
           {
               // Implementation
               _logger.LogInformation("Operation completed");
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "Operation failed");
               throw;
           }
       }
   }
   ```

3. **Register Service**
   ```csharp
   // Program.cs
   builder.Services.AddScoped<IMyService, MyService>();
   ```

4. **Inject and Use**
   ```razor
   @inject IMyService MyService
   
   @code {
       private async Task HandleClick()
       {
           await MyService.DoSomethingAsync("test");
       }
   }
   ```

### Adding File Type Validation

The validation is already implemented in `Index.razor`, but here's how to extend it:

```csharp
// Add to appsettings.json
"FileUpload": {
  "AllowedExtensions": [".pdf", ".docx", ".txt"],
  "AllowedMimeTypes": ["application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"]
}

// Add to FileUploadSettings
public List<string> AllowedMimeTypes { get; set; } = new();

// Add validation in Index.razor
if (uploadSettings.AllowedMimeTypes.Any() && 
    !uploadSettings.AllowedMimeTypes.Contains(selectedFile.ContentType))
{
    errorMessage = $"File type '{selectedFile.ContentType}' is not allowed.";
    return;
}
```

## Testing

### Unit Testing Setup

1. **Create Test Project**
   ```powershell
   dotnet new xunit -n PushFile.Tests
   dotnet sln add PushFile.Tests
   dotnet add PushFile.Tests reference PushFile
   ```

2. **Add Testing Packages**
   ```powershell
   cd PushFile.Tests
   dotnet add package Moq
   dotnet add package FluentAssertions
   ```

3. **Example Test**
   ```csharp
   public class FileUploadServiceTests
   {
       [Fact]
       public async Task SaveFileAsync_CreatesDirectory_WhenNotExists()
       {
           // Arrange
           var settings = Options.Create(new FileUploadSettings 
           { 
               DestinationDirectory = "C:\\Temp\\Test" 
           });
           var logger = Mock.Of<ILogger<FileUploadService>>();
           var service = new FileUploadService(settings, logger);
           
           // Act & Assert
           // ... test implementation
       }
   }
   ```

### Integration Testing

Use bUnit for Blazor component testing:

```powershell
dotnet add package bunit
```

```csharp
public class IndexPageTests : TestContext
{
    [Fact]
    public void Index_RendersCorrectly()
    {
        // Arrange
        Services.AddScoped<IFileUploadService, MockFileUploadService>();
        
        // Act
        var cut = RenderComponent<Index>();
        
        // Assert
        cut.MarkupMatches("<div>...</div>");
    }
}
```

## Debugging

### VS Code Debugging

1. **Set Breakpoints**: Click in the gutter next to line numbers
2. **Start Debugging**: Press `F5`
3. **Debug Actions**:
   - `F10`: Step over
   - `F11`: Step into
   - `Shift+F11`: Step out
   - `F5`: Continue

### Logging

Use structured logging:

```csharp
_logger.LogInformation(
    "File uploaded: {FileName} by {User} at {Time}", 
    fileName, 
    userName, 
    DateTime.UtcNow
);
```

View logs in:
- Console output (development)
- Application Insights (production)
- File logs (if configured)

### Common Issues

**Issue:** Blazor circuit disconnected  
**Solution:** Check browser console, ensure SignalR is working

**Issue:** File upload fails  
**Solution:** Check directory permissions, file size limits

**Issue:** Email not sending  
**Solution:** Verify SMTP settings, check firewall, enable logging

## Performance Optimization

### File Upload Performance

1. **Streaming Large Files**
   ```csharp
   // For files > 100MB, use streaming
   using var stream = file.OpenReadStream(maxAllowedSize: long.MaxValue);
   ```

2. **Progress Reporting**
   ```razor
   <InputFile OnChange="OnInputFileChange" />
   <progress value="@uploadProgress" max="100"></progress>
   
   @code {
       private int uploadProgress;
       
       private async Task OnInputFileChange(InputFileChangeEventArgs e)
       {
           // Implement progress tracking
       }
   }
   ```

### Email Performance

1. **Async Sending**
   - Already implemented with `async/await`

2. **Queue for Multiple Emails**
   ```csharp
   // Consider using a background service
   builder.Services.AddHostedService<EmailQueueService>();
   ```

## Security Best Practices

### Input Validation

✅ **Do:**
- Validate file size
- Validate file extension
- Validate MIME type
- Sanitize filenames

❌ **Don't:**
- Trust client-side validation only
- Allow arbitrary file uploads
- Use user-provided filenames directly

### Configuration Security

✅ **Do:**
- Use User Secrets in development
- Use environment variables in production
- Use Azure Key Vault for sensitive data

❌ **Don't:**
- Commit passwords to source control
- Use plain text passwords
- Share configuration files

### File Storage Security

✅ **Do:**
- Store files outside wwwroot
- Use unique filenames
- Scan files for malware (in production)
- Set restrictive directory permissions

❌ **Don't:**
- Store uploaded files in wwwroot
- Allow script execution in upload directory
- Use predictable filenames

## Deployment

### Local IIS Deployment

1. **Publish the Application**
   ```powershell
   dotnet publish -c Release -o ./publish
   ```

2. **Configure IIS**
   - Install ASP.NET Core Hosting Bundle
   - Create application pool (.NET CLR Version: No Managed Code)
   - Create website pointing to publish folder

3. **Update Configuration**
   - Edit `appsettings.json` in publish folder
   - Set production SMTP credentials
   - Configure production upload directory

### Azure App Service Deployment

1. **Create App Service**
   ```powershell
   az webapp create --name pushfile --resource-group myRG --plan myPlan
   ```

2. **Deploy**
   ```powershell
   dotnet publish -c Release
   cd bin/Release/net8.0/publish
   zip -r deploy.zip *
   az webapp deployment source config-zip --src deploy.zip --name pushfile --resource-group myRG
   ```

3. **Configure**
   - Set application settings in Azure Portal
   - Add SMTP credentials to app settings
   - Configure custom domain and SSL

## Continuous Integration

### GitHub Actions Example

```yaml
name: Build and Test

on: [push, pull_request]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 8.0.x
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal
```

## Maintenance

### Regular Tasks

- Update NuGet packages monthly
- Review and rotate SMTP credentials
- Monitor disk space in upload directory
- Review error logs
- Test email delivery

### Monitoring

Consider adding:
- Application Insights
- Health checks
- Uptime monitoring
- Email delivery tracking

## Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor)
- [MailKit Documentation](https://github.com/jstedfast/MailKit)
- [Bootstrap Documentation](https://getbootstrap.com/docs)

## Getting Help

1. Check the [README.md](README.md)
2. Review the [API.md](API.md)
3. Check GitHub Issues
4. Stack Overflow (tag: blazor, asp.net-core)
