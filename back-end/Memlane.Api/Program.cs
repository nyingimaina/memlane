using Memlane.Api.Hubs;
using Memlane.Api.Infrastructure;
using Memlane.Api.Models;
using Memlane.Api.Providers;
using Memlane.Api.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Ensure the application runs on port 5237
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5237);
});

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=memlane.db";
builder.Services.AddSingleton<IDbConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));
builder.Services.AddScoped<IJobRepository, SqliteJobRepository>();
builder.Services.AddSingleton<ISyncEngine, FileHashSyncEngine>();
builder.Services.AddSingleton<IFilenameGenerator, SortableFilenameGenerator>();
builder.Services.AddSingleton<IBackupProvider, SqlServerBackupProvider>();
builder.Services.AddSingleton<IBackupProvider, MariaDbBackupProvider>();
builder.Services.AddSingleton<IStorageProvider, LocalStorageProvider>();
builder.Services.AddSingleton<IStorageProvider, FolderStorageProvider>();
builder.Services.AddSingleton<IStorageProvider, S3StorageProvider>();
builder.Services.AddSingleton<IStorageProviderFactory, StorageProviderFactory>();
builder.Services.AddScoped<IJobOrchestrator, BackupJobOrchestrator>();
builder.Services.AddHostedService<BackgroundJobService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { Error = "An unexpected error occurred. Please check the logs." });
        });
    });
}

app.UseHttpsRedirection();
app.UseCors();

app.MapHub<JobHub>("/hubs/jobs");

// Job API Endpoints
app.MapGet("/api/jobs", async (IJobRepository repo) =>
{
    return Results.Ok(await repo.GetAllJobsAsync());
});

app.MapGet("/api/jobs/{id}", async (int id, IJobRepository repo) =>
{
    var job = await repo.GetByIdAsync(id);
    return job != null ? Results.Ok(job) : Results.NotFound();
});

app.MapPost("/api/jobs", async ([FromBody] JobMetadata job, IJobRepository repo) =>
{
    job.CreatedAt = DateTime.UtcNow;
    job.Status = JobStatus.Pending;
    var id = await repo.AddJobAsync(job);
    job.Id = id;
    return Results.Created($"/api/jobs/{id}", job);
});

app.MapPut("/api/jobs/{id}", async (int id, [FromBody] JobMetadata job, IJobRepository repo) =>
{
    job.Id = id;
    await repo.UpdateAsync(job);
    return Results.Ok(job);
});

app.MapDelete("/api/jobs/{id}", async (int id, IJobRepository repo) =>
{
    await repo.DeleteAsync(id);
    return Results.NoContent();
});

app.MapPost("/api/jobs/{id}/trigger", async (int id, IJobRepository repo) =>
{
    await repo.UpdateJobStatusAsync(id, JobStatus.Pending);
    return Results.Accepted();
});

app.Run();
