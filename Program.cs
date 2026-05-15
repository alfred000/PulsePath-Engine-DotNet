using PulsePath_Engine_DotNet.Models;
using PulsePath_Engine_DotNet.Business;

namespace PulsePath_Engine_DotNet;

class Program
{
    static void Main(string[] args)
    {
        LogService service = new LogService();
        
        Console.WriteLine("=================================================");
        Console.WriteLine("🚀   BIENVENUE SUR PULSEPATH ENGINE (.NET)     🚀");
        Console.WriteLine("=================================================");
        
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

        // Exécution des règles du profil
        double bmrInitial = MetabolicEngine.CalculateBMR(poidsDepart, tailleCm, age, isMale);
        double imcInitial = MetabolicEngine.CalculateIMC(poidsDepart, tailleCm);
        double imgInitial = MetabolicEngine.CalculateIMG(imcInitial, age, isMale);
        var (categorieImc, poidsMin, poidsMax) = MetabolicEngine.GetImcInterpretation(tailleCm);

        // Affichage du bilan de santé de départ
        Console.WriteLine("\n------------------- VOS MÉTRIQUES -------------------");
        Console.WriteLine($"📊 IMC : {imcInitial:F1}");
        Console.WriteLine($"🧬 IMG (Masse Grasse) : {imgInitial:F1}%");
        Console.WriteLine($"⚖️ Plage de poids bien-être conseillée : {poidsMin:F1} kg - {poidsMax:F1} kg");
        Console.WriteLine($"🍏 Métabolisme de Base (BMR) : {bmrInitial:F0} kcal (Maintenance au repos)");
        
        // --- ÉTAPE 2 : FIXATION DE L'OBJECTIF S.M.A.R.T ---
        Console.WriteLine("\n[2/3] PLANIFICATION DE L'OBJECTIF S.M.A.R.T");
        Console.WriteLine("Choisissez votre objectif : (perte / maintien / gain)");
        Console.Write("Votre choix : ");
        string objectif = Console.ReadLine()?.ToLower() ?? "maintien";

        Console.Write("Entrez votre poids cible (kg) : ");
        double poidsCible = double.Parse(Console.ReadLine()!);

        // Sécurité SMART basée sur la plage bien-être
        if (objectif == "perte" && poidsCible < poidsMin)
        {
            Console.WriteLine($"⚠️ ATTENTION : Votre cible est sous votre poids bien-être ({poidsMin:F1} kg).");
        }

        // Calcul du budget calorique théorique de départ (Hypothèse activité moyenne pour calibrer)
        double tdeeInitial = bmrInitial * 1.4; 
        double budgetCaloriesCible = tdeeInitial;
        if (objectif == "perte") budgetCaloriesCible -= 500;
        else if (objectif == "gain") budgetCaloriesCible += 300;

        var (pro, glu, lip) = MetabolicEngine.CalculateMacros(budgetCaloriesCible, objectif);

        Console.WriteLine("\n------------------- VOTRE PLANNING -------------------");
        Console.WriteLine($"🎯 Budget Énergétique Ciblé : {budgetCaloriesCible:F0} Calories / jour");
        Console.WriteLine($"🧬 Ratio Macros Parfait : P: {pro}g | G: {glu}g | L: {lip}g");

        // --- ÉTAPE 3 : JOURNALISATION QUOTIDIENNE (DAILY LOG) ---
        Console.WriteLine("\n[3/3] DOSSIER DE SUIVI QUOTIDIEN (SOURCE UNIQUE DE VÉRITÉ)");
        Console.WriteLine("Prévu pour être connecté par API (Apple Health/Garmin) prochainement.");

        bool continuer = true;
        while (continuer)
        {
            Console.WriteLine("\n--- Nouvelle saisie journalière ---");
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

            Console.Write("Objectif de jeûne intermittent atteint ? (o/n) : ");
            log.FastingValidated = Console.ReadLine()?.ToLower() == "o";

            // Calculs dynamiques basés sur le mouvement réel du jour
            double bmrDuJour = MetabolicEngine.CalculateBMR(log.Weight, tailleCm, age, isMale);
            double facteurActivite = MetabolicEngine.GetActivityFactor(log.Steps);
            double tdeeDuJour = MetabolicEngine.CalculateTDEE(bmrDuJour, facteurActivite);
            double caloriesBruleesActivite = MetabolicEngine.CalculateCaloriesBurned(tdeeDuJour, bmrDuJour);

            // Enregistrement et Vélocité
            service.AddLog(log);
            double deficitHebdoReel = service.GetAverageWeeklyDeficit(tdeeDuJour);
            DateTime dateEstimee = VelocityEngine.ProjectTargetDate(log.Weight, poidsCible, deficitHebdoReel);

            // Évaluation de la qualité de la donnée (Gamification)
            int scoreIntegrite = InsightEngine.CalculateIntegrityScore(log);

            // Restitution complète du Dashboard Console
            Console.WriteLine("\n=================== DASHBOARD DU JOUR ===================");
            Console.WriteLine($"🔥 Dépense Totale (TDEE) : {tdeeDuJour:F0} kcal");
            Console.WriteLine($"🏃 Calories Brûlées (Activité) : {caloriesBruleesActivite:F0} kcal");
            Console.WriteLine($"⚖️ Bilan Net : {log.CaloriesIn - tdeeDuJour:F0} kcal");
            
            if (dateEstimee == DateTime.MaxValue)
                Console.WriteLine("⏱️ Trajectoire : Échéance impossible à projeter (Pas de déficit).");
            else
                Console.WriteLine($"⏱️ Date d'échéance estimée : {dateEstimee.ToShortDateString()}");

            Console.WriteLine("\n--- COACHING & INSIGHTS ---");
            Console.WriteLine(InsightEngine.GetSleepInsight(log.SleepHours));
            Console.WriteLine(InsightEngine.GetProteinInsight(log.ProteinsIn, log.Weight));
            Console.WriteLine($"📊 Score de Qualité des Données (Intégrité) : {scoreIntegrite}%");
            if (scoreIntegrite == 100) Console.WriteLine("🏆 Badge 'Intégrité' débloqué ! Vos données de santé sont parfaitement fiables.");

            Console.Write("\nAjouter une autre journée ? (o/n) : ");
            continuer = Console.ReadLine()?.ToLower() == "o";
        }
    }
}
