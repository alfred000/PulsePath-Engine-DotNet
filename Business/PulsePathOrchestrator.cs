using PulsePath_Engine_DotNet.Models;

namespace PulsePath_Engine_DotNet.Business;

public class UserProfile
{
    public bool IsMale { get; set; }
    public int Age { get; set; }
    public double HeightCm { get; set; }
    public double WeightDepart { get; set; }
    public double WeightCible { get; set; }
    public string Objectif { get; set; } = "maintien";
    public int SeancesPrevues { get; set; }
    public double BudgetCaloriesCible { get; set; }
}

public class DashboardResult
{
    public int JoursEcoules { get; set; }
    public double PerteTotaleKg { get; set; }
    public double ProgresPourcent { get; set; }
    public double TdeeDuJour { get; set; }
    public double CaloriesBruleesActivite { get; set; }
    public int TotalSeancesCetteSemaine { get; set; }
    public DateTime DateEstimee { get; set; }
    public int ScoreIntegrite { get; set; }
    public PlanRattrapage? PlanCorrection { get; set; }
    public string MessagePrescriptif { get; set; } = string.Empty;
}

public class PulsePathOrchestrator
{
    private readonly LogService _logService;
    private readonly UserProfile _profile;
    private readonly DateTime _dateFixationObjectif;

    public PulsePathOrchestrator(UserProfile profile, LogService logService, DateTime dateFixation)
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
