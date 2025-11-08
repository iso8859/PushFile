# Quick Start Guide

## First Time Setup

1. **Install Prerequisites**
   - [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
   - [Visual Studio Code](https://code.visualstudio.com/)

2. **Open in VS Code**
   ```powershell
   cd r:\dev\GithubPublic\PushFile
   code .
   ```

3. **Install Recommended Extensions**
   - VS Code will prompt you to install recommended extensions
   - Click "Install All" when prompted

4. **Configure SMTP Settings**
   - Open `appsettings.json`
   - Update the `Email` section with your SMTP server details:
   
   ```json
   "Email": {
     "SmtpServer": "smtp.gmail.com",
     "SmtpPort": 587,
     "SmtpUsername": "your-email@gmail.com",
     "SmtpPassword": "your-app-password",
     "EnableSsl": true,
     "FromEmail": "your-email@gmail.com",
     "FromName": "File Upload System",
     "NotificationEmail": "recipient@example.com"
   }
   ```

5. **Configure Upload Directory**
   - In `appsettings.json`, set the destination directory:
   
   ```json
   "FileUpload": {
     "DestinationDirectory": "C:\\Uploads"
   }
   ```
   
   - Create the directory:
   ```powershell
   New-Item -ItemType Directory -Path "C:\Uploads" -Force
   ```

6. **Run the Application**
   - Press `F5` in VS Code (or use the Run menu)
   - The browser will open automatically to the upload page

## Using Gmail SMTP

If you're using Gmail:

1. Enable 2-factor authentication on your Google account
2. Generate an App Password:
   - Go to https://myaccount.google.com/security
   - Click "2-Step Verification"
   - Scroll to "App passwords"
   - Generate a new app password for "Mail"
3. Use the generated password in `SmtpPassword`

## Testing the Application

1. **Navigate to the upload page** (opens automatically)
2. **Select a file** using the file picker
3. **Click "Upload File"**
4. **Check:**
   - File appears in your destination directory
   - Email notification is received

## Common Configuration Examples

### File Size and Type Restrictions

```json
"FileUpload": {
  "MaxFileSizeMB": 50,
  "AllowedExtensions": [".pdf", ".docx", ".xlsx", ".jpg", ".png", ".zip"]
}
```

### Customizing the Page

```json
"PageSettings": {
  "Title": "Document Upload Center",
  "WelcomeMessage": "Upload your documents for processing"
}
```

## VS Code Features

### Debugging
- Set breakpoints by clicking in the gutter
- Press `F5` to start debugging
- Use the Debug Console to inspect variables

### Tasks
- `Ctrl+Shift+B` - Build the project
- `Ctrl+Shift+P` then type "Run Task" to see all available tasks

### Terminal
- `` Ctrl+` `` - Open integrated terminal
- Run `dotnet watch run` for auto-reload during development

## Troubleshooting

### Build Errors
```powershell
dotnet clean
dotnet restore
dotnet build
```

### SMTP Issues
- Check your firewall allows port 587 (or 465 for SSL)
- Verify SMTP credentials
- Check application logs in the console

### File Upload Issues
- Ensure the destination directory exists
- Check directory permissions
- Verify file size and extension settings

## Next Steps

- Add authentication (see Microsoft's Blazor docs)
- Implement file type validation
- Add file preview functionality
- Set up logging to files
- Deploy to Azure or IIS

## Support

See the main [README.md](README.md) for full documentation.
