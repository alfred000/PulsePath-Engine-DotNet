using PulsePath_Engine_DotNet.Business;

double poidsActuel = 85.0;
double poidsCible = 80.0;
double tdee = 2500.0;
int caloriesConsommees = 2000; // Déficit de 500 kcal/jour

double deficitHebdo = (caloriesConsommees - tdee) * 7; // -3500 kcal/semaine
DateTime dateEstimee = VelocityEngine.ProjectTargetDate(poidsActuel, poidsCible, deficitHebdo);

Console.WriteLine("--- PulsePath Engine .NET ---");
Console.WriteLine($"Date cible estimée : {dateEstimee.ToShortDateString()}");

