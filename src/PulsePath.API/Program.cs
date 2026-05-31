using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using PulsePath.Infrastructure.Data;
using PulsePath.Infrastructure.Services;
using PulsePath.Core.Interfaces;
using PulsePath.Core.Services;
using PulsePath.Infrastructure.Repositories;

namespace PulsePath_Engine_DotNet;

public class Program
{
    public static void Main(string[] args)
    {
        // 1. Initialisation du Builder
        var builder = WebApplication.CreateBuilder(args);

        // 2. Enregistrement des services nécessaires
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // Configuration de l'injection de dépendances pour votre base SQLite
        builder.Services.AddDbContext<AppDbContext>();

        // Configuration de la sécurité JWT dans le pipeline .NET
        var keyStr = builder.Configuration["Jwt:Key"] ?? "SUPER_SECRET_KEY_MIN_32_CHARS_LONG_PULSEPATH_2026";
        var key = Encoding.UTF8.GetBytes(keyStr);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
        });

        // Configuration avancée de Swagger pour supporter l'authentification JWT visuellement
        // Remplacez votre bloc builder.Services.AddSwaggerGen(...) par cette implémentation :
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
            { 
                Title = "PulsePath Engine API V2", 
                Version = "v1" 
            });

            var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Saisissez votre jeton JWT de la forme : Bearer {votre_token}",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Id = "Bearer",
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
                }
            };

            c.AddSecurityDefinition("Bearer", securityScheme);

            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            });
        });



        // Configuration CORS pour autoriser les futures requêtes Web (Angular/React)
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Enregistrement de l'infrastructure d'authentification découpée
        builder.Services.AddScoped<IAuthRepository, AuthRepository>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        // Enregistrement du service de génération de tokens JWT
        builder.Services.AddScoped<ITokenService, TokenService>();
        // 3. Construction de l'application
        var app = builder.Build();

        // 4. Activation SYSTEMATIQUE de Swagger
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "PulsePath Engine API V2");
            c.RoutePrefix = "swagger"; // L'URL d'accès sera http://localhost:5000/swagger/index.html
        });

        // 5. Configuration du Pipeline HTTP
        app.UseCors("AllowAll");

        // ATTENTION : L'authentification doit TOUJOURS être placée avant l'autorisation
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        Console.WriteLine("=========================================================");
        Console.WriteLine("🌐   SERVEUR WEB PULSEPATH ENGINE DÉMARRÉ SUR LE PORT 5000 ");
        Console.WriteLine("🌐   URL SWAGGER : http://localhost:5000/swagger/index.html");
        Console.WriteLine("=========================================================");

        // 6. Lancement du serveur
        app.Run();
    }
}
