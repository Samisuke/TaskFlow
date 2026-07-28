using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Services;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Core.Repositories;
using TaskFlow.Api.Config.MapsterConfig;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Scopes
builder.Services.AddScoped<IUsuarioRepository, UsuarioReposity>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Inyección del context
builder.Services.AddDbContext<TaskFlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Mapster
builder.Services.AddMapsterMappings();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
