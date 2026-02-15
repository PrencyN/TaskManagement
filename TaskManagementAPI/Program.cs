using TaskManagementAPI.Middleware;
using TaskManagementAPI.Repositories;
using TaskManagementAPI.Services;
using System.Text.Json.Serialization;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This ensures enums are serialized as strings in JSON responses
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Task Management API", 
        Version = "v1",
        Description = "API for managing tasks with status transitions"
    });
    
    // This makes enums show as strings with dropdowns in Swagger UI
    c.UseInlineDefinitionsForEnums();
});

// Register dependencies
builder.Services.AddSingleton<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API V1");
        // This ensures enums show as dropdowns
        c.DisplayOperationId();
        c.DisplayRequestDuration();
    });
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();