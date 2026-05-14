using PulsePath_Engine_DotNet.Models;
using PulsePath_Engine_DotNet.Business;

namespace PulsePath_Engine_DotNet;

class Program
{
    static void Main(string[] args)
    {
        LogService service = new LogService();

        Console.WriteLine("=== BIENVENUE SUR PULSEPATH ENGINE (.NET) ===");
        Console.WriteLine("Paramétrage de l'objectif :");

        Console.Write("Poids cible (kg) : ");
        double poidsCible = double.Parse(Console.ReadLine()!);

        bool continuer = true;
        while (continuer)
        {
            Console.WriteLine("\n--- Nouvelle saisie quotidienne ---");

            // Création d'un nouveau log
            DailyLog log = new DailyLog { Date = DateTime.Now };

            // Saisie avec validation simple (SF-01)
            Console.Write("Poids actuel (kg) : ");
            log.Weight = double.Parse(Console.ReadLine()!);

            Console.Write("Calories consommées (kcal) : ");
            log.CaloriesIn = int.Parse(Console.ReadLine()!);

            Console.Write("Nombre de pas effectués : ");
            log.Steps = int.Parse(Console.ReadLine()!);

            Console.Write("Heures de sommeil : ");
            log.SleepHours = double.Parse(Console.ReadLine()!);

            Console.Write("Protéines consommées (g) [0 si non renseigné] : ");
            log.ProteinsIn = int.Parse(Console.ReadLine()!);

            Console.Write("Objectif de jeûne atteint ? (o/n) : ");
            log.FastingValidated = Console.ReadLine()?.ToLower() == "o";


            // 1. Calcul immédiat du TDEE (RM-MET-01)
            double bmr = MetabolicEngine.CalculateBMR(log.Weight, 180, 30, true); // Taille/Âge fixés pour le test
            double facteur = MetabolicEngine.GetActivityFactor(log.Steps);
            double tdee = MetabolicEngine.CalculateTDEE(bmr, facteur);

            // 2. Enregistrement et Calcul de Vélocité (RM-VEL-01)
            service.AddLog(log);
            double deficitHebdo = service.GetAverageWeeklyDeficit(tdee);
            DateTime dateEstimee = VelocityEngine.ProjectTargetDate(log.Weight, poidsCible, deficitHebdo);

            // 3. Restitution des résultats (Dashboard Console)
            Console.WriteLine("\n--- RÉSULTATS DU MOTEUR ---");
            Console.WriteLine($"Votre dépense totale (TDEE) : {tdee:F0} kcal");
            Console.WriteLine($"Bilan Net : {log.CaloriesIn - tdee:F0} kcal");

            // 4. Génération des Insights (Coaching automatisé)
            string conseilSommeil = InsightEngine.GetSleepInsight(log.SleepHours);
            // Calcul du score d'intégrité (RM-GAM-01)
            int scoreIntegrite = InsightEngine.CalculateIntegrityScore(log);
            Console.WriteLine("\n--- COACHING & INSIGHTS ---");
            Console.WriteLine(conseilSommeil);
            Console.WriteLine($"📊 SCORE D'INTÉGRITÉ DE LA DONNÉE : {scoreIntegrite}%");

            if (scoreIntegrite == 100)
            {
                Console.WriteLine("🏆 FÉLICITATIONS : Badge 'Intégrité' débloqué ! Vos prédictions sont fiables à 95%.");
            }
            else
            {
                Console.WriteLine("ℹ️ INFO : Log incomplet. La fiabilité de l'estimation de la trajectoire est réduite.");
            }
            
            if (dateEstimee == DateTime.MaxValue)
            {
                Console.WriteLine("Trajectoire : En surplus (Pas de date d'échéance possible)");
            }
            else
            {
                Console.WriteLine($"Date d'échéance estimée : {dateEstimee.ToShortDateString()}");
            }

            Console.Write("\nAjouter une autre journée ? (o/n) : ");
            continuer = Console.ReadLine()?.ToLower() == "o";
        }

        Console.WriteLine("\nMerci d'avoir utilisé PulsePath Engine. Vos données sont stockées en mémoire.");
    }
}

