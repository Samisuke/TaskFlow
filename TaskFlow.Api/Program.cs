using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Api.Config.MapsterConfig;

using TaskFlow.Infrastructure.Services;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Core.Repositories;
using Taskflow.Core.Repositories;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Scopes
builder.Services.AddScoped<IUsuarioRepository, UsuarioReposity>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProyectoRepository, ProyectoRepository>();
builder.Services.AddScoped<IProyectoService, ProyectoService>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();
builder.Services.AddScoped<ITareaRepository, TareaRepository>();
builder.Services.AddScoped<ITareaService, TareaService>();
builder.Services.AddScoped<IEtiquetaRepository, EtiquetaRepository>();
builder.Services.AddScoped<IEtiquetaService, EtiquetaService>();
builder.Services.AddScoped<IHistorialRepository, HistorialRepository>();
builder.Services.AddScoped<IHIstorialService, HistorialService>();
builder.Services.AddScoped<IProyectoUsuarioRepository, ProyectoUsuarioRepository>();
builder.Services.AddScoped<IProyectoUsuarioService, ProyectoUsuarioService>();

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
