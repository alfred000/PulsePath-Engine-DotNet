using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PulsePath_Engine_DotNet;

public class Program
{
    public static void Main(string[] args)
    {
        // 1. Initialisation du Builder (Répare l'erreur CS0103)
        var builder = WebApplication.CreateBuilder(args);

        // 2. Enregistrement des services nécessaires
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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

        // 4. Activation SYSTEMATIQUE de Swagger (Pas de condition IF pour ton portfolio)
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "PulsePath Engine API V2");
            c.RoutePrefix = "swagger"; // L'URL d'accès sera /swagger/index.html
        });

        // 5. Configuration du Pipeline HTTP
        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.MapControllers();

        Console.WriteLine("=========================================================");
        Console.WriteLine("🌐   SERVEUR WEB PULSEPATH ENGINE DÉMARRÉ SUE LE PORT 5000 ");
        Console.WriteLine("🌐   URL SWAGGER : http://localhost:5000/swagger/index.html");
        Console.WriteLine("=========================================================");

        // 6. Lancement du serveur
        app.Run();
    }
}
