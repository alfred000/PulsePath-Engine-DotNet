using System;
using PulsePath_Engine_DotNet.Models;
using PulsePath_Engine_DotNet.Business;

namespace PulsePath_Engine_DotNet;

class Program
{
    static void Main(string[] args)
    {
        LogService logService = new LogService();
        DateTime dateFixation = DateTime.Now.AddDays(-5); // Simulation
        UserProfile profile = new UserProfile();
        
        Console.WriteLine("=========================================================");
        Console.WriteLine("🚀     BIENVENUE SUR PULSEPATH ENGINE (.NET V2)        🚀");
        Console.WriteLine("=========================================================");
        
        // --- ÉTAPE 1 & 2 : ONBOARDING & PLANIFICATION S.M.A.R.T ---
        Console.WriteLine("\n[1/2] CONFIGURATION DU PROFIL & OBJECTIF");
        Console.Write("Genre (h/f) : "); profile.IsMale = Console.ReadLine()?.ToLower() == "h";
        Console.Write("Votre Âge (ans) : "); profile.Age = int.Parse(Console.ReadLine()!);
        Console.Write("Votre Taille (cm) : "); profile.HeightCm = double.Parse(Console.ReadLine()!);
        Console.Write("Poids de départ (kg) : "); profile.WeightDepart = double.Parse(Console.ReadLine()!);
        
        Console.Write("Objectif (perte / seche / maintien / gain) : "); profile.Objectif = Console.ReadLine()?.ToLower() ?? "maintien";
        Console.Write("Poids cible (kg) : "); profile.WeightCible = double.Parse(Console.ReadLine()!);
        Console.Write("Nombre de Séances d'entrainement prévues / semaine : "); profile.SeancesPrevues = int.Parse(Console.ReadLine()!);

        // Calibrage initial des calories et macros
        double bmrInitial = MetabolicEngine.CalculateBMR(profile.WeightDepart, profile.HeightCm, profile.Age, profile.IsMale);
        profile.BudgetCaloriesCible = bmrInitial * 1.2; // Base sédentaire par défaut pour l'onboarding
        if (profile.Objectif == "perte" || profile.Objectif == "seche") profile.BudgetCaloriesCible -= 500;
        else if (profile.Objectif == "gain") profile.BudgetCaloriesCible += 300;

        var (pro, glu, lip) = MetabolicEngine.CalculateMacros(profile.BudgetCaloriesCible, profile.Objectif);
        Console.WriteLine($"🎯 Apport cible : {profile.BudgetCaloriesCible:F0} kcal | Macros : P: {pro}g, G: {glu}g, L: {lip}g");

        // Initialisation de l'orchestrateur service
        PulsePathOrchestrator orchestrator = new PulsePathOrchestrator(profile, logService, dateFixation);

        // --- ÉTAPE 3 : JOURNALISATION QUOTIDIENNE ---
        bool continuer = true;
        while (continuer)
        {
            Console.WriteLine("\n--- Saisie de la journée de suivi ---");
            DailyLog log = new DailyLog { Date = DateTime.Now };

            Console.Write("Poids du jour (kg) : "); log.Weight = double.Parse(Console.ReadLine()!);
            Console.Write("Calories consommées (kcal) : "); log.CaloriesIn = int.Parse(Console.ReadLine()!);
            Console.Write("Nombre de pas : "); log.Steps = int.Parse(Console.ReadLine()!);
            Console.Write("Heures de sommeil : "); log.SleepHours = double.Parse(Console.ReadLine()!);
            Console.Write("Protéines (g) : "); log.ProteinsIn = int.Parse(Console.ReadLine()!);
            Console.Write("Séance de sport validée ? (1 pour oui/0 pour non) : "); log.WorkoutsDone = int.Parse(Console.ReadLine()!);
            Console.Write("Objectif de jeûne atteint ? (o pour oui/n pour non) : "); log.FastingValidated = Console.ReadLine()?.ToLower() == "o";

            // Envoi au service d'orchestration
            DashboardResult result = orchestrator.ProcessDailyLog(log);

            // === RESTITUTION STRUCTURÉE DU DASHBOARD CONSOLE ===
            
            // BLOC 1 : DASHBOARD DU JOUR (KPIs)
            Console.WriteLine("\n=================== [BLOC 1] DASHBOARD DU JOUR ===================");
            Console.WriteLine($"⏱️ Jours cumulés : {result.JoursEcoules} jours | 🏋️ Sport : {result.TotalSeancesCetteSemaine} / {profile.SeancesPrevues} séances");
            Console.WriteLine($"📉 Perte totale : {result.PerteTotaleKg:F1} kg | 📈 Progrès global : {result.ProgresPourcent:F1}%");
            Console.WriteLine($"🔥 TDEE : {result.TdeeDuJour:F0} kcal | 🏃 Activité : +{result.CaloriesBruleesActivite:F0} kcal");
            Console.WriteLine($"⚖️ Balance Calorique Net : {log.CaloriesIn - result.TdeeDuJour:F0} kcal");
            Console.WriteLine(result.DateEstimee == DateTime.MaxValue ? "⏱️ Trajectoire : Échéance gelée." : $"⏱️ Date d'échéance révisée : {result.DateEstimee.ToShortDateString()}");

            // BLOC 2 : RECOMMANDATION PRESCRIPTIVE (AIDE À LA DÉCISION)
            Console.WriteLine("\n=================== [BLOC 2] RECOMMANDATION PRESCRIPTIVE ===================");
            if (result.PlanCorrection != null)
            {
                Console.WriteLine($"{result.PlanCorrection.Message}");
                Console.WriteLine($"🎯 Nouvelle cible Calories (7 jours) : {result.PlanCorrection.CibleCalories:F0} kcal");
                Console.WriteLine($"🏃 Nouvelle cible Pas (7 jours)      : {result.PlanCorrection.CiblePas:F0} pas");
                Console.WriteLine($"🏋️ Cardio LISS suggéré             : {result.PlanCorrection.CardioLISSSuggere}");
            }
            else
            {
                Console.WriteLine(result.MessagePrescriptif);
            }

            // BLOC 3 : COACHING ET INSIGHTS COMPORTEMENTAUX
            Console.WriteLine("\n=================== [BLOC 3] COACHING & INSIGHTS ===================");
            Console.WriteLine(InsightEngine.GetSleepInsight(log.SleepHours));
            Console.WriteLine(InsightEngine.GetProteinInsight(log.ProteinsIn, log.Weight));
            Console.WriteLine($"📊 Score d'Intégrité des données : {result.ScoreIntegrite}%");

            Console.Write("\nAjouter une autre journée ? (o/n) : ");
            continuer = Console.ReadLine()?.ToLower() == "o";
        }
    }
}
