using System;
using System.Collections.Generic;
using System.Linq;
using PulsePath_Engine_DotNet.Data;
using PulsePath_Engine_DotNet.Models;

namespace PulsePath_Engine_DotNet.Business;

public class LogService
{
    // Initialise et assure la création automatique de la base de données
    public LogService()
    {
        using var context = new PulsePathContext();
        context.Database.EnsureCreated(); // Crée la base de données et la table si elles n'existent pas
    }

    // Ajouter une entrée en base de données SQLite
    public void AddLog(DailyLog log)
    {
        using var context = new PulsePathContext();
        context.DailyLogs.Add(log);
        context.SaveChanges(); // Persiste la donnée sur le disque dur
    }

    // Récupérer l'intégralité des logs depuis la base de données
    public List<DailyLog> GetAllLogs()
    {
        using var context = new PulsePathContext();
        return context.DailyLogs.ToList();
    }

    // Calcul de la moyenne glissante sur les 7 derniers logs persistés (RM-VEL-01)
    public double GetAverageWeeklyDeficit(double currentTdee)
    {
        using var context = new PulsePathContext();
        
        var lastSevenDays = context.DailyLogs
            .OrderByDescending(l => l.Date)
            .Take(7)
            .ToList();
        
        if (lastSevenDays.Count == 0) return 0;

        double totalDeficit = 0;
        foreach (var log in lastSevenDays)
        {
            totalDeficit += (log.CaloriesIn - currentTdee);
        }

        return (totalDeficit / lastSevenDays.Count) * 7;
    }
}
