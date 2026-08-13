using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Api.Config.MapsterConfig;
using TaskFlow.Api.Config.JwtConfig;
using FluentValidation;

using TaskFlow.Api.Config.DependencyInjection;
using TaskFlow.Core.Validations;
using TaskFlow.Api.Middlewares;
using TaskFlow.Api.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger
builder.Services.AddTaskFlowSwagger();

// Scopes
builder.Services.AddTaskFlowServices();

// Inyección del context
builder.Services.AddDbContext<TaskFlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Mapster
builder.Services.AddMapsterMappings();

// JWT
builder.Services.AddTaskFlowAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

// Validations
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioValidator>();

// Builders del Middleware
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
