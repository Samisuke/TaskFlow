
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TaskFlow.Api.Config.JwtConfig
{
    public static class JwtConfig
    {
        public static IServiceCollection AddTaskFlowAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtKey = configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException(
                    "JWT configuration missing: Jwt:Key");
            }

            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Solo desarrollo
                options.SaveToken = true;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(keyBytes),

                        ValidateIssuer = true,
                        ValidIssuer = configuration["Jwt:Issuer"],

                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
            });

            services.AddAuthorization();

            return services;
        }
    }
}