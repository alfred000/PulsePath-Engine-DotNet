using PulsePath_Engine_DotNet.Models;

namespace PulsePath_Engine_DotNet.Business;

public class LogService
{
    private List<DailyLog> _logs = new List<DailyLog>();

    // Ajouter une entrée
    public void AddLog(DailyLog log)
    {
        _logs.Add(log);
    }

    // Récupérer tous les logs
    public List<DailyLog> GetAllLogs() => _logs;

    // Règle RM-VEL-01 : Calculer la moyenne du déficit sur les 7 derniers jours
    public double GetAverageWeeklyDeficit(double currentTdee)
    {
        // On prend les 7 derniers éléments
        var lastSevenDays = _logs.OrderByDescending(l => l.Date).Take(7).ToList();
        
        if (lastSevenDays.Count == 0) return 0;

        double totalDeficit = 0;
        foreach (var log in lastSevenDays)
        {
            totalDeficit += (log.CaloriesIn - currentTdee);
        }

        return (totalDeficit / lastSevenDays.Count) * 7;
    }
}
