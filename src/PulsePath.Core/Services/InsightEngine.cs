using PulsePath.Core.Models;

namespace PulsePath.Core.Services;

public static class InsightEngine
{
    // Règle RM-FAST-01 : Analyse croisée Sommeil / Énergie
    public static string GetSleepInsight(double sleepHours)
    {
        if (sleepHours < 7)
        {
            return "⚠️ INSIGHT : Votre sommeil est inférieur à 7h. Attention, cela peut dérégler vos hormones de faim demain.";
        }
        return "✅ INSIGHT : Sommeil optimal. Votre récupération favorise la stabilité métabolique.";
    }

    // Règle RM-PRO-01 : Alerte Protéines (si vous avez déjà les protéines dans votre modèle)
    public static string GetProteinInsight(double proteins, double weight)
    {
        double seuil = weight * 1.5;
        if (proteins < seuil)
        {
            return "⚠️ ALERTE : Apport protéique trop bas. Risque de perte musculaire pendant le déficit.";
        }
        return "✅ INFO : Apport protéique suffisant pour protéger vos muscles.";
    }

    // Règle RM-GAM-01 : Calcul du Score d'Intégrité
    public static int CalculateIntegrityScore(DailyLog log)
    {
        int score = 0;

        // Poids et Calories (Critique : 50%)
        if (log.Weight > 0 && log.CaloriesIn > 0) score += 50;

        // Pas et Sommeil (30%)
        if (log.Steps > 0 && log.SleepHours > 0) score += 30;

        // Protéines et Jeûne (20% - 10% chacun)
        if (log.ProteinsIn > 0) score += 10;
        if (log.FastingValidated) score += 10;

        return score;
    }
}
