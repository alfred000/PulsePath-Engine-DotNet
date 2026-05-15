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

    //  Calcul de l'IMC
    public static double CalculateIMC(double weight, double heightCm)
    {
        double heightMeters = heightCm / 100;
        return weight / (heightMeters * heightMeters);
    }

    //  Interprétation de l'IMC et Recommandation de Plage
    public static (string categorie, double poidsMin, double poidsMax) GetImcInterpretation(double heightCm)
    {
        double heightMeters = heightCm / 100;
        double poidsMin = 18.5 * (heightMeters * heightMeters);
        double poidsMax = 25.0 * (heightMeters * heightMeters);
        return ("Interprétation gérée dans l'UI", poidsMin, poidsMax);
    }

    //  Calcul de l'IMG (Deurenberg)
    public static double CalculateIMG(double imc, int age, bool isMale)
    {
        int genderFactor = isMale ? 1 : 0;
        return (1.20 * imc) + (0.23 * age) - (10.8 * genderFactor) - 5.4;
    }

    //  Calcul des Macromolécules (Ratio Parfait)
    public static (int pro, int glu, int lip) CalculateMacros(double targetCalories, string objective)
    {
        double pPro = 0.25, pGlu = 0.45, pLip = 0.30; // Maintien par défaut

        if (objective == "perte") { pPro = 0.30; pGlu = 0.40; pLip = 0.30; }
        else if (objective == "gain") { pPro = 0.25; pGlu = 0.50; pLip = 0.25; }

        int proG = (int)((targetCalories * pPro) / 4);
        int gluG = (int)((targetCalories * pGlu) / 4);
        int lipG = (int)((targetCalories * pLip) / 9);

        return (proG, gluG, lipG);
    }
    public static double CalculateCaloriesBurned(double tdee, double bmr)
    {
        return tdee - bmr;
    }
}
