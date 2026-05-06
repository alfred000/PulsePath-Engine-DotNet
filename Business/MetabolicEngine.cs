namespace PulsePath_Engine_DotNet.Business;
public static class MetabolicEngine
{
    // Règle RM-MET-01 : Équation de Mifflin-St Jeor
    public static double CalculateBMR(double weight, double height, int age, bool isMale)
    {
        if (isMale)
            return (10 * weight) + (6.25 * height) - (5 * age) + 5;
        
        return (10 * weight) + (6.25 * height) - (5 * age) - 161;
    }

    // Règle RM-MET-01 : Facteur d'Activité Dynamique
    public static double GetActivityFactor(int steps)
    {
        if (steps < 5000) return 1.2;      // Sédentaire
        if (steps < 10000) return 1.4;     // Actif
        return 1.6;                        // Très Actif
    }

    public static double CalculateTDEE(double bmr, double activityFactor)
    {
        return bmr * activityFactor;
    }
}
