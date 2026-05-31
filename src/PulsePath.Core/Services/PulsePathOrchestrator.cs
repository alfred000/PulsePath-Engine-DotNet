using PulsePath.Core.Models;
using PulsePath.Core.Interfaces;
namespace PulsePath.Core.Services;
public class PulsePathOrchestrator
{
    private readonly ILogService _logService;
    private readonly UserProfile _profile;
    private readonly DateTime _dateFixationObjectif;

    public PulsePathOrchestrator(UserProfile profile, ILogService logService, DateTime dateFixation)
    {
        _profile = profile;
        _logService = logService;
        _dateFixationObjectif = dateFixation;
    }

    public DashboardResult ProcessDailyLog(DailyLog log)
    {
        _logService.AddLog(log);
        DashboardResult result = new DashboardResult();

        // 1. Calculs Métaboliques du jour
        double bmrDuJour = MetabolicEngine.CalculateBMR(log.Weight, _profile.HeightCm, _profile.Age, _profile.IsMale);
        double facteurActivite = MetabolicEngine.GetActivityFactor(log.Steps);
        result.TdeeDuJour = MetabolicEngine.CalculateTDEE(bmrDuJour, facteurActivite);
        result.CaloriesBruleesActivite = MetabolicEngine.CalculateCaloriesBurned(result.TdeeDuJour, bmrDuJour);

        // 2. Vélocité et Échéance
        double deficitHebdoReel = _logService.GetAverageWeeklyDeficit(result.TdeeDuJour);
        result.DateEstimee = VelocityEngine.ProjectTargetDate(log.Weight, _profile.WeightCible, deficitHebdoReel);

        // 3. Règle RM-KPI-01 : Progression et Compteur de Jours
        result.JoursEcoules = (DateTime.Now - _dateFixationObjectif).Days + 1;
        if ((_profile.Objectif == "perte" || _profile.Objectif == "seche") && log.Weight < _profile.WeightDepart)
        {
            result.PerteTotaleKg = _profile.WeightDepart - log.Weight;
            result.ProgresPourcent = (result.PerteTotaleKg / (_profile.WeightDepart - _profile.WeightCible)) * 100;
            if (result.ProgresPourcent > 100) result.ProgresPourcent = 100.0;
        }

        result.TotalSeancesCetteSemaine = _logService.GetAllLogs().Sum(l => l.WorkoutsDone);
        result.ScoreIntegrite = InsightEngine.CalculateIntegrityScore(log);

        // 4. Déclenchement du Moteur Prescriptif (RM-COR-01)
        int joursRetard = 0;
        if (result.DateEstimee != DateTime.MaxValue && result.DateEstimee > DateTime.Now.AddDays(30))
        {
            joursRetard = (result.DateEstimee - DateTime.Now.AddDays(30)).Days;
        }

        if (joursRetard >= 3)
        {
            result.PlanCorrection = CourseCorrectionEngine.CalculateCourseCorrection(log.Weight, _profile.BudgetCaloriesCible, 8000, bmrDuJour, joursRetard);
        }
        else
        {
            result.MessagePrescriptif = "🔥 Recommandation : Vous êtes parfaitement sur la bonne voie ! Continuez ainsi pour sécuriser votre date d'échéance cible.";
        }

        return result;
    }
}
