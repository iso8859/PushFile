# PushFile - Blazor File Upload System

A Blazor Server application that provides a secure file upload portal with email notifications.

## Features

- 📤 **File Upload**: Simple, intuitive interface for uploading files
- 📧 **Email Notifications**: Automatic SMTP email notifications when files are uploaded
- ⚙️ **Configurable**: All settings managed through `appsettings.json`
- 🔒 **File Validation**: File size and extension restrictions
- 🎨 **Modern UI**: Clean, responsive Bootstrap-based interface
- 📝 **Detailed Logging**: Comprehensive logging for debugging and monitoring

## Configuration

All configuration is done in `appsettings.json`. Here's what you need to configure:

### File Upload Settings

```json
"FileUpload": {
  "DestinationDirectory": "C:\\Uploads",           // Where files are saved
  "MaxFileSizeMB": 100,                            // Maximum file size in MB
  "AllowedExtensions": [".pdf", ".doc", ".docx"]   // Allowed file types
}
```

### SMTP Email Settings

```json
"Email": {
  "SmtpServer": "smtp.example.com",               // SMTP server address
  "SmtpPort": 587,                                 // SMTP port
  "SmtpUsername": "your-email@example.com",        // SMTP username
  "SmtpPassword": "your-password",                 // SMTP password
  "EnableSsl": true,                               // Use SSL/TLS
  "FromEmail": "noreply@example.com",              // Sender email
  "FromName": "File Upload System",                // Sender name
  "NotificationEmail": "owner@example.com"         // Where notifications are sent
}
```

### Page Customization

```json
"PageSettings": {
  "Title": "File Upload Portal",                   // Page title
  "WelcomeMessage": "Upload your files securely"   // Welcome message
}
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio Code (recommended) or Visual Studio

### Installation

1. **Clone or download this project**

2. **Restore dependencies**
   ```powershell
   dotnet restore
   ```

3. **Configure settings**
   - Edit `appsettings.json` with your SMTP server details
   - Set the destination directory for uploads
   - Customize page title and message

4. **Create the upload directory**
   ```powershell
   New-Item -ItemType Directory -Path "C:\Uploads" -Force
   ```

5. **Run the application**
   ```powershell
   dotnet run
   ```

6. **Open your browser**
   - Navigate to `https://localhost:5001` (or the URL shown in the console)

## Development

### Building

```powershell
dotnet build
```

### Running in Development Mode

```powershell
dotnet run --environment Development
```

### Publishing

```powershell
dotnet publish -c Release -o ./publish
```

## Project Structure

```
PushFile/
├── Models/                      # Configuration models
│   ├── EmailSettings.cs
│   ├── FileUploadSettings.cs
│   └── PageSettings.cs
├── Pages/                       # Blazor pages
│   ├── Index.razor             # Main upload page
│   └── _Host.cshtml            # Host page
├── Services/                    # Business logic
│   ├── EmailService.cs         # Email sending
│   ├── FileUploadService.cs    # File handling
│   └── Interfaces/
├── wwwroot/                     # Static files
│   └── css/
│       └── site.css
├── .vscode/                     # VS Code configuration
│   ├── launch.json
│   ├── tasks.json
│   ├── settings.json
│   └── extensions.json
├── appsettings.json            # Configuration
├── appsettings.Development.json
├── Program.cs                  # Application entry point
└── PushFile.csproj            # Project file
```

## VS Code Integration

This project includes comprehensive VS Code configuration:

- **IntelliSense**: Full C# and Razor support
- **Debugging**: F5 to launch and debug
- **Tasks**: Build, run, clean, restore, watch
- **Extensions**: Recommended extensions for best experience

### Recommended Extensions

The project will prompt you to install:
- C# Dev Kit
- C# for Visual Studio Code
- .NET Runtime Install Tool

## Security Considerations

⚠️ **Important**: Before deploying to production:

1. **Never commit passwords** - Use user secrets or environment variables
2. **Use HTTPS** - Ensure SSL/TLS is properly configured
3. **Validate uploads** - The application includes basic validation, but consider additional security measures
4. **Restrict access** - Add authentication/authorization as needed
5. **Sanitize filenames** - The app adds timestamps, but consider additional sanitization
6. **Set proper permissions** - Ensure the upload directory has appropriate access controls

## Troubleshooting

### Email not sending

- Verify SMTP settings in `appsettings.json`
- Check firewall/antivirus blocking SMTP ports
- Enable "Less secure app access" if using Gmail (or use App Passwords)
- Check application logs for detailed error messages

### Files not uploading

- Ensure destination directory exists and has write permissions
- Check file size limits
- Verify allowed file extensions
- Review application logs

### Application won't start

- Ensure .NET 8.0 SDK is installed: `dotnet --version`
- Restore NuGet packages: `dotnet restore`
- Check for compilation errors: `dotnet build`

## License

This project is provided as-is for educational and commercial use.

## Support

For issues, questions, or contributions, please use the GitHub repository.
