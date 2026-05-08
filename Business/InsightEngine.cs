namespace PulsePath_Engine_DotNet.Business;

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
}
