using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PulsePath_Engine_DotNet.Data;
using Microsoft.OpenApi;

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
        builder.Services.AddDbContext<PulsePathContext>();

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
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PulsePath Engine API V2",
                Version = "v1"
            });

            // 1. Definition for JWT Bearer Tokens
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter your JWT token in this format: Bearer {votre_token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            // 2. Programmatic injection to completely bypass the CS1950 dictionary initializer error
            c.AddSecurityRequirement(document =>
            {
                var requirement = new OpenApiSecurityRequirement();
                var schemeReference = new OpenApiSecuritySchemeReference("Bearer", document);

                requirement.Add(schemeReference, new List<string>());
                return requirement;
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
