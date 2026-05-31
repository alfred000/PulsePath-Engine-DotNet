namespace PulsePath.Core.Services;

public static class VelocityEngine
{
    // Règle RM-VEL-01 : 7700 kcal de déficit = 1kg de gras
    private const int KcalPerKg = 7700; 

    public static double CalculateDailyNetBalance(int caloriesIn, double tdee)
    {
        return caloriesIn - tdee;
    }

    public static DateTime ProjectTargetDate(double currentWeight, double targetWeight, double averageWeeklyDeficit)
    {
        // Sécurité : Si l'utilisateur est en surplus, on ne peut pas projeter de date de fin
        if (averageWeeklyDeficit >= 0) return DateTime.MaxValue; 

        double kgALoser = currentWeight - targetWeight;
        double totalKcalABruler = kgALoser * KcalPerKg;
        
        // On calcule le déficit journalier moyen (Hebdo / 7)
        double averageDailyDeficit = Math.Abs(averageWeeklyDeficit / 7);
        int joursRestants = (int)Math.Ceiling(totalKcalABruler / averageDailyDeficit);

        return DateTime.Now.AddDays(joursRestants);
    }
}
