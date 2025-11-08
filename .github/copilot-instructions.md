# GitHub Copilot Instructions

## Project Context

This is a Blazor Server application for file uploads with email notifications.

## Key Architecture Points

- **Framework**: ASP.NET Core 8.0 Blazor Server
- **Email**: MailKit for SMTP
- **Configuration**: Options pattern with appsettings.json
- **Dependency Injection**: Scoped services for Email and FileUpload

## Code Style Preferences

- Use file-scoped namespaces
- Prefer explicit types over `var` for clarity
- Add XML documentation comments for public APIs
- Use nullable reference types
- Follow async/await patterns consistently

## Common Tasks

### Adding New Configuration
1. Add property to appropriate model in `Models/` folder
2. Update corresponding section in `appsettings.json`
3. Inject `IOptions<T>` where needed

### Adding New Services
1. Create interface in `Services/` folder
2. Implement service with logging
3. Register in `Program.cs` using DI
4. Inject where needed

### Modifying Upload Logic
- File validation happens in `Pages/Index.razor`
- File saving logic is in `Services/FileUploadService.cs`
- Update both `FileUploadSettings` model and validation if changing rules

### Email Template Changes
- HTML template: `EmailService.BuildEmailBody()`
- Text template: `EmailService.BuildTextEmailBody()`

## Security Considerations

- Never hardcode credentials
- Validate all file inputs
- Sanitize file paths
- Use HTTPS in production
- Implement authentication/authorization as needed

## Testing Checklist

- [ ] File size validation
- [ ] File extension validation
- [ ] Directory creation
- [ ] Email sending (SMTP connectivity)
- [ ] Error handling and logging
- [ ] UI responsiveness
- [ ] Configuration changes

## Deployment Notes

- Update SMTP credentials before deployment
- Ensure destination directory exists and has proper permissions
- Configure HTTPS certificates
- Set appropriate file size limits for production
- Review and update allowed file extensions
