using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Api.Config.MapsterConfig;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;

using TaskFlow.Infrastructure.Services;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services.Token;
using TaskFlow.Infrastructure.Services.Token;
using TaskFlow.Core.Validations;
using TaskFlow.Api.Middlewares;

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
builder.Services.AddScoped<IHistorialService, HistorialService>();
builder.Services.AddScoped<IProyectoUsuarioRepository, ProyectoUsuarioRepository>();
builder.Services.AddScoped<IProyectoUsuarioService, ProyectoUsuarioService>();
builder.Services.AddScoped<ITareaEtiquetaRepository, TareaEtiquetaRepository>();
builder.Services.AddScoped<ITareaEtiquetaService, TareaEtiquetaService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Scopes de permissions
builder.Services.AddScoped<IProyectoPermissionService, ProyectoPermissionService>();
builder.Services.AddScoped<IComentarioPermissionService, ComentarioPermissionService>();
builder.Services.AddScoped<ITareaPermissionService, TareaPermissionService>();
builder.Services.AddScoped<IPassPermissionService, PassPermissionService>();

// Inyección del context
builder.Services.AddDbContext<TaskFlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Mapster
builder.Services.AddMapsterMappings();

//JWT
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT configuration missing: Jwt:Key");
}
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false; // False para las pruebas, debería ser true en la version final.
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Validations
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioValidator>();

// Builders del Middleware
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
