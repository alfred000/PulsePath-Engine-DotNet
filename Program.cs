using System;
using System.Linq;
using PulsePath_Engine_DotNet.Models;
using PulsePath_Engine_DotNet.Business;

namespace PulsePath_Engine_DotNet;

class Program
{
    static void Main(string[] args)
    {
        LogService service = new LogService();
        
        // Configuration de la date de départ (Simule que l'objectif a été fixé il y a 5 jours)
        DateTime dateFixationObjectif = DateTime.Now.AddDays(-5); 
        
        Console.WriteLine("=========================================================");
        Console.WriteLine("🚀     BIENVENUE SUR PULSEPATH ENGINE (.NET V2)        🚀");
        Console.WriteLine("=========================================================");
        
        // --- ÉTAPE 1 : ONBOARDING & DIAGNOSTIC MÉTABOLIQUE ---
        Console.WriteLine("\n[1/3] DIAGNOSTIC DE DÉPART");
        
        Console.Write("Genre (h/f) : ");
        bool isMale = Console.ReadLine()?.ToLower() == "h";

        Console.Write("Votre Âge (ans) : ");
        int age = int.Parse(Console.ReadLine()!);

        Console.Write("Votre Taille (cm) : ");
        double tailleCm = double.Parse(Console.ReadLine()!);

        Console.Write("Poids de départ (kg) : ");
        double poidsDepart = double.Parse(Console.ReadLine()!);

        Console.WriteLine("\nNiveau d'activité physique habituel :");
        Console.WriteLine("1. Mode de vie sédentaire (Bureau, peu de déplacements)");
        Console.WriteLine("2. Activité légère (Travail debout, marche légère)");
        Console.WriteLine("3. Activité modérée (Actif, marche beaucoup, entraînement léger)");
        Console.WriteLine("4. Activité élevée (Métier physique, entraînement intense)");
        Console.Write("Votre choix (1-4) : ");
        int choixActivite = int.Parse(Console.ReadLine()!);

        // Exécution des calculs biométriques initiaux
        double bmrInitial = MetabolicEngine.CalculateBMR(poidsDepart, tailleCm, age, isMale);
        double imcInitial = MetabolicEngine.CalculateIMC(poidsDepart, tailleCm);
        double imgInitial = MetabolicEngine.CalculateIMG(imcInitial, age, isMale);
        var (categorieImc, poidsMin, poidsMax) = MetabolicEngine.GetImcInterpretation(tailleCm, imcInitial);
        double facteurInitial = MetabolicEngine.GetInitialActivityFactor(choixActivite);
        double tdeeInitial = MetabolicEngine.CalculateTDEE(bmrInitial, facteurInitial);

        // Affichage du profil santé initial
        Console.WriteLine("\n------------------- DIAGNOSTIC INITIAL -------------------");
        Console.WriteLine($"📊 Catégorie IMC : {categorieImc} ({imcInitial:F1})");
        Console.WriteLine($"🧬 IMG (Masse Grasse) : {imgInitial:F1}%");
        Console.WriteLine($"⚖️ Votre plage de poids bien-être : {poidsMin:F1} kg - {poidsMax:F1} kg");
        Console.WriteLine($"🍏 Métabolisme de Base (BMR) : {bmrInitial:F0} kcal");
        Console.WriteLine($"🔥 Maintenance théorique initiale : {tdeeInitial:F0} kcal / jour");
        
        // --- ÉTAPE 2 : PLANIFICATION S.M.A.R.T ---
        Console.WriteLine("\n[2/3] PLANIFICATION DE L'OBJECTIF S.M.A.R.T");
        Console.WriteLine("Choisissez votre objectif : (perte / seche / maintien / gain)");
        Console.Write("Votre choix : ");
        string objectif = Console.ReadLine()?.ToLower() ?? "maintien";

        Console.Write("Entrez votre poids cible (kg) : ");
        double poidsCible = double.Parse(Console.ReadLine()!);

        Console.Write("Combien de séances de sport prévoyez-vous par semaine ? : ");
        int seancesPrevues = int.Parse(Console.ReadLine()!);

        // Calcul du budget énergétique cible
        double budgetCaloriesCible = tdeeInitial;
        if (objectif == "perte" || objectif == "seche") budgetCaloriesCible -= 500;
        else if (objectif == "gain") budgetCaloriesCible += 300;

        var (pro, glu, lip) = MetabolicEngine.CalculateMacros(budgetCaloriesCible, objectif);

        Console.WriteLine("\n------------------- VOTRE PLANNING VALIDÉ -------------------");
        Console.WriteLine($"🎯 Apport énergétique ciblé : {budgetCaloriesCible:F0} kcal / jour");
        Console.WriteLine($"🧬 Ratios Macros : P: {pro}g | G: {glu}g | L: {lip}g");
        Console.WriteLine($"🏋️ Fréquence d'entraînement cible : {seancesPrevues} séances / semaine");
        if (objectif == "perte" || objectif == "seche")
        {
            Console.WriteLine($"💡 Stratégie : Atteignez {budgetCaloriesCible:F0} kcal. Ajoutez 8000 pas/jour ou du cardio LISS pour maximiser la dépense.");
        }

        // --- ÉTAPE 3 : JOURNALISATION QUOTIDIENNE (DAILY LOG) ---
        Console.WriteLine("\n[3/3] DOSSIER DE SUIVI QUOTIDIEN");

        bool continuer = true;
        while (continuer)
        {
            Console.WriteLine("\n--- Saisie de la journée de suivi ---");
            DailyLog log = new DailyLog { Date = DateTime.Now };

            Console.Write("Poids du jour (kg) : ");
            log.Weight = double.Parse(Console.ReadLine()!);

            Console.Write("Calories consommées (kcal) : ");
            log.CaloriesIn = int.Parse(Console.ReadLine()!);

            Console.Write("Nombre de pas (Montre connectée) : ");
            log.Steps = int.Parse(Console.ReadLine()!);

            Console.Write("Heures de sommeil (Tracker de nuit) : ");
            log.SleepHours = double.Parse(Console.ReadLine()!);

            Console.Write("Protéines consommées (g) : ");
            log.ProteinsIn = int.Parse(Console.ReadLine()!);

            Console.Write("Séance de sport validée aujourd'hui ? (1 pour oui / 0 pour non) : ");
            log.WorkoutsDone = int.Parse(Console.ReadLine()!);

            Console.Write("Objectif de jeûne intermittent atteint ? (o/n) : ");
            log.FastingValidated = Console.ReadLine()?.ToLower() == "o";

            // Calculs dynamiques du jour (RM-MET-01)
            double bmrDuJour = MetabolicEngine.CalculateBMR(log.Weight, tailleCm, age, isMale);
            double facteurActivite = MetabolicEngine.GetActivityFactor(log.Steps);
            double tdeeDuJour = MetabolicEngine.CalculateTDEE(bmrDuJour, facteurActivite);
            double caloriesBruleesActivite = MetabolicEngine.CalculateCaloriesBurned(tdeeDuJour, bmrDuJour);

            // Enregistrement des données et calcul de vélocité (RM-VEL-01)
            service.AddLog(log);
            double deficitHebdoReel = service.GetAverageWeeklyDeficit(tdeeDuJour);
            DateTime dateEstimee = VelocityEngine.ProjectTargetDate(log.Weight, poidsCible, deficitHebdoReel);

            // --- APPLICATION DE LA RÈGLE RM-KPI-01 : PROGRESSION LINÉAIRE ---
            int joursEcoules = (DateTime.Now - dateFixationObjectif).Days + 1;
            double perteTotaleKg = 0;
            double progresPourcent = 0;

            if (objectif == "perte" || objectif == "seche")
            {
                // Si l'utilisateur est en surplus (poids du jour >= poids de départ), le progrès reste figé à 0
                if (log.Weight < poidsDepart)
                {
                    perteTotaleKg = poidsDepart - log.Weight;
                    double totalAPerdre = poidsDepart - poidsCible;
                    progresPourcent = (perteTotaleKg / totalAPerdre) * 100;
                    if (progresPourcent > 100) progresPourcent = 100.0;
                }
                else
                {
                    perteTotaleKg = 0.0;
                    progresPourcent = 0.0;
                }
            }

            int totalSeancesCetteSemaine = service.GetAllLogs().Sum(l => l.WorkoutsDone);
            int scoreIntegrite = InsightEngine.CalculateIntegrityScore(log);

            // Simulation d'un calcul de retard pour déclencher le moteur de rattrapage
            int joursRetard = 0;
            if (dateEstimee != DateTime.MaxValue && dateEstimee > DateTime.Now.AddDays(30))
            {
                joursRetard = (dateEstimee - DateTime.Now.AddDays(30)).Days;
            }

            // --- RESTITUTION DU DASHBOARD CONSOLE ---
            Console.WriteLine("\n=================== DASHBOARD DU JOUR ===================");
            Console.WriteLine($"⏱️ Jours cumulés depuis le début : {joursEcoules} jours");
            Console.WriteLine($"📉 Perte totale : {perteTotaleKg:F1} kg | 📈 Progrès global : {progresPourcent:F1}%");
            Console.WriteLine($"🔥 TDEE du jour : {tdeeDuJour:F0} kcal | 🏃 Activité (Pas/LISS) : +{caloriesBruleesActivite:F0} kcal");
            Console.WriteLine($"🏋️ Séances validées cette semaine : {totalSeancesCetteSemaine} / {seancesPrevues}");
            Console.WriteLine($"⚖️ Balance Calorique Net du jour : {log.CaloriesIn - tdeeDuJour:F0} kcal");
            
            if (dateEstimee == DateTime.MaxValue)
                Console.WriteLine("⏱️ Trajectoire : Échéance gelée (En surplus métabolique temporaire).");
            else
                Console.WriteLine($"⏱️ Date d'échéance révisée : {dateEstimee.ToShortDateString()}");

            // --- INTELLIGENCE PRESCRIPTIVE : MOTEUR DE RATTRAPAGE (RM-COR-01) ---
            if (joursRetard >= 3)
            {
                // Objectif de pas initial fixé à 8000 pour le calcul de déviation
                var planRattrapage = CourseCorrectionEngine.CalculateCourseCorrection(log.Weight, budgetCaloriesCible, 8000, bmrDuJour, joursRetard);
                
                Console.WriteLine("\n🚨 --- RECOMMANDATION PRESCRIPTIVE (REMETTRE SUR LA BONNE VOIE) ---");
                Console.WriteLine($"{planRattrapage.Message}");
                Console.WriteLine($"Cible Calories temporaire (7 jours) : {planRattrapage.CibleCalories:F0} kcal (Seuil de sécurité BMR respecté)");
                Console.WriteLine($"Cible Pas temporaire (7 jours)      : {planRattrapage.CiblePas:F0} pas");
                Console.WriteLine($"Cardio LISS suggéré                 : {planRattrapage.CardioLISSSuggere}");
            }
            else
            {
                Console.WriteLine("\n--- COACHING & INSIGHTS STANDARD ---");
                Console.WriteLine(InsightEngine.GetSleepInsight(log.SleepHours));
                Console.WriteLine(InsightEngine.GetProteinInsight(log.ProteinsIn, log.Weight));
                Console.WriteLine($"📊 Score d'Intégrité de la donnée : {scoreIntegrite}%");
                if (scoreIntegrite == 100) Console.WriteLine("🏆 Badge 'Intégrité' débloqué ! Vos données de santé sont parfaitement fiables.");
            }

            Console.Write("\nAjouter une autre journée ? (o/n) : ");
            continuer = Console.ReadLine()?.ToLower() == "o";
        }
        
        Console.WriteLine("\nApplication fermée. Toutes les métriques ont été calculées.");
    }
}
