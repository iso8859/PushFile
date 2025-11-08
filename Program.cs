using PushFile.Models;
using PushFile.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configure settings from appsettings.json
builder.Services.Configure<FileUploadSettings>(
    builder.Configuration.GetSection("FileUpload"));
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Email"));
builder.Services.Configure<PageSettings>(
    builder.Configuration.GetSection("PageSettings"));

// Register services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddSingleton<IFileIndexService, FileIndexService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Minimal API endpoints for file download — avoid Razor rendering to allow header manipulation
app.MapGet("/download-file/{token}", (string token, IFileIndexService index) =>
{
    var path = index.GetFilePath(token);
    if (path == null || !System.IO.File.Exists(path))
    {
        return Results.NotFound();
    }

    var stream = System.IO.File.OpenRead(path);
    var fileName = System.IO.Path.GetFileName(path);
    return Results.File(stream, "application/octet-stream", fileName, enableRangeProcessing: true);
});

app.MapGet("/download-file-byname/{fileName}", (string fileName, IConfiguration config, IFileIndexService index) =>
{
    var uploadDir = config.GetSection("FileUpload")["DestinationDirectory"] ?? string.Empty;
    var path = System.IO.Path.Combine(uploadDir, fileName);
    if (!System.IO.File.Exists(path))
    {
        return Results.NotFound();
    }

    var stream = System.IO.File.OpenRead(path);
    return Results.File(stream, "application/octet-stream", fileName, enableRangeProcessing: true);
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
