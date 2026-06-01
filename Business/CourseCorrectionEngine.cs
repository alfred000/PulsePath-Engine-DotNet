namespace PulsePath_Engine_DotNet.Business;

public class PlanRattrapage
{
    public int DureeJours { get; set; } = 7;
    public double CibleCalories { get; set; }
    public double CiblePas { get; set; }
    public string Message { get; set; } = string.Empty;
    public string CardioLISSSuggere { get; set; } = string.Empty;
}

public static class CourseCorrectionEngine
{
    /// <summary>
    /// RM-COR-01 : Calcule un plan de rattrapage sur 7 jours en cas de déviation de la date cible.
    /// </summary>
    public static PlanRattrapage CalculateCourseCorrection(
        double poidsActuel, 
        double budgetCaloriesInitial, 
        double objectifPasInitial, 
        double bmr, 
        int joursEcart)
    {
        PlanRattrapage plan = new PlanRattrapage();

        // Étape A : Calcul de l'écart énergétique (Hypothèse de base : 500 kcal de déficit manqué par jour d'écart)
        double surplusARattraper = joursEcart * 500;
        double effortQuotidienRequis = surplusARattraper / plan.DureeJours;

        // Étape B : Répartition des leviers sous contraintes (Règle 40/60)
        double ajustementCalorique = effortQuotidienRequis * 0.40;
        double cibleTemporaireCalories = budgetCaloriesInitial - ajustementCalorique;
        double effortResiduel;

        // Application de la contrainte métabolique (Hard Guardrail sur le BMR)
        if (cibleTemporaireCalories < bmr)
        {
            plan.CibleCalories = bmr; // Bloqué au niveau de survie
            effortResiduel = effortQuotidienRequis - (budgetCaloriesInitial - bmr); // Le reste bascule sur l'activité
        }
        else
        {
            plan.CibleCalories = cibleTemporaireCalories;
            effortResiduel = effortQuotidienRequis * 0.60;
        }

        // Étape C : Conversion de l'effort résiduel en activité (Pas / LISS)
        // Modèle prédictif : 1000 pas brûlent environ (Poids * 0.5) kcal
        double caloriesPar1000Pas = poidsActuel * 0.5;
        double pasSupplementaires = (effortResiduel / caloriesPar1000Pas) * 1000;
        double nouvelleCiblePas = objectifPasInitial + pasSupplementaires;

        // Application de la contrainte d'épuisement (Hard Guardrail à 18 000 pas)
        if (nouvelleCiblePas > 18000)
        {
            plan.CiblePas = 18000;
            plan.Message = "⚠️ Écart trop important pour être rattrapé sainement en 7 jours. Cible d'activité bridée pour éviter l'épuisement.";
            plan.CardioLISSSuggere = "Ajouter 45 min de marche rapide ou vélo basse intensité (LISS).";
        }
        else
        {
            plan.CiblePas = Math.Round(nouvelleCiblePas);
            plan.Message = "📉 Plan de correction dynamique actif pour vous remettre sur la bonne voie.";
            plan.CardioLISSSuggere = effortResiduel > 200 ? "Ajouter 20-30 min de marche active quotidienne." : "Aucun cardio additionnel requis.";
        }

        return plan;
    }
}
