using Microsoft.AspNetCore.Mvc;
using PulsePath_Engine_DotNet.Models;
using PulsePath_Engine_DotNet.Business;
using PulsePath_Engine_DotNet.DTOs;
//Un alias strict pour cibler votre classe de calcul métabolique
using UserProfile = PulsePath_Engine_DotNet.Business.UserProfile;

namespace PulsePath_Engine_DotNet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PulsePathController : ControllerBase
{
    private readonly LogService _logService;

    public PulsePathController()
    {
        // En production, on utiliserait l'injection de dépendances, 
        // mais pour notre portfolio, on instancie directement notre service SQLite
        _logService = new LogService();
    }

    // Endpoint 1 : Récupérer tout l'historique des logs (GET /api/pulsepath/history)
    [HttpGet("history")]
    public IActionResult GetHistory()
    {
        var logs = _logService.GetAllLogs();
        return Ok(logs);
    }

    // Endpoint 2 : Soumettre une journée et obtenir le Dashboard immédiat (POST /api/pulsepath/log)
    [HttpPost("log")]
    public IActionResult SubmitDailyLog([FromBody] DailyLogDto dto)
    {
        // Simulation d'un profil de test (Homme, 30 ans, 180cm, objectif Perte)
        UserProfile mockProfile = new UserProfile
        {
            IsMale = true,
            Age = 30,
            HeightCm = 180,
            WeightDepart = 85.0,
            WeightCible = 75.0,
            Objectif = "perte",
            BudgetCaloriesCible = 1636
        };

        // Extraction du DTO vers notre Modèle de BDD
        DailyLog logEntity = new DailyLog
        {
            Date = DateTime.Now,
            Weight = dto.Weight,
            CaloriesIn = dto.CaloriesIn,
            Steps = dto.Steps,
            SleepHours = dto.SleepHours,
            ProteinsIn = dto.ProteinsIn,
            FastingValidated = dto.FastingValidated,
            WorkoutsDone = dto.WorkoutsDone
        };

        // Utilisation de l'Orchestrateur propre que nous avons codé à l'étape précédente
        PulsePathOrchestrator orchestrator = new PulsePathOrchestrator(mockProfile, _logService, DateTime.Now.AddDays(-5));
        DashboardResult results = orchestrator.ProcessDailyLog(logEntity);

        // On renvoie un JSON propre contenant l'analyse complète (Bloc 1, 2 et 3)
        return Ok(results);
    }
}
