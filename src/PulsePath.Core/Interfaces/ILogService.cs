using System;
using PulsePath.Core.Models;

namespace PulsePath.Core.Interfaces
{
    public interface ILogService
    {
        // Enregistre les données du journal quotidien pour un utilisateur donné
        void AddLog(DailyLog log);
        // Récupère tous les logs journaliers enregistrés pour un utilisateur donné
        public List<DailyLog> GetAllLogs();
        // RM-VEL-01 : Calcule la moyenne glissante du déficit calorique sur les 7 derniers jours
        double GetAverageWeeklyDeficit(double currentTdee);

    }
}
