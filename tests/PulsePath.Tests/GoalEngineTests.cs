using Xunit;
using PulsePath.Core.Dtos;
using PulsePath.Core.Models;
using PulsePath.Core.Services;

namespace PulsePath.Tests;

public class GoalEngineTests
{
    private readonly GoalEngine _goalEngine;

    public GoalEngineTests()
    {
        _goalEngine = new GoalEngine();
    }

    [Fact]
    public void Test_RM_GOAL_01_Should_Approve_Safe_Weekly_Fat_Loss_Rate()
    {
        // ARRANGE : Utilisateur pesant 80 kg. 1% de son poids = 0.8 kg/semaine maximum autorisés.
        var profile = new UserProfile { CurrentWeightKg = 80.0 };
        
        // Objectif : Perdre 4 kg en 10 semaines -> Taux planifié = 4 / 10 = 0.4 kg/semaine (Sécurisé)
        var safeRequest = new GoalRequestDto("perte", 76.0, 10);

        // ACT
        var response = _goalEngine.EvaluateGoal(profile, safeRequest);

        // ASSERT : Le statut doit être approuvé sans modification restrictive
        Assert.Equal("Approved", response.Status);
        Assert.Equal(0.4, response.SafeWeeklyRateKg, 2);
        
        // Calcul du déficit calorique quotidien attendu : (0.4 * 7700) / 7 = 440 kcal
        Assert.Equal(440, response.CaloricDeficitTarget);
    }

    [Fact]
    public void Test_RM_GOAL_01_Should_Override_And_Clamp_Dangerous_Aggressive_Deficit()
    {
        // ARRANGE : Utilisateur pesant 100 kg. 1% de son poids = 1.0 kg/semaine maximum autorisés.
        var profile = new UserProfile { CurrentWeightKg = 100.0 };
        
        // Objectif : Perdre 10 kg en seulement 5 semaines -> Taux planifié = 10 / 5 = 2.0 kg/semaine (Dangereux !)
        var aggressiveRequest = new GoalRequestDto("perte", 90.0, 5);

        // ACT
        var response = _goalEngine.EvaluateGoal(profile, aggressiveRequest);

        // ASSERT : L'algorithme doit écraser l'entrée (Override) et brider le taux à la borne de 1%
        Assert.Equal("Overridden", response.Status);
        Assert.Equal(1.0, response.SafeWeeklyRateKg, 2); // Bridé à 1.0 kg au lieu des 2.0 kg demandés
        
        // Calcul du déficit bridé de sécurité : (1.0 * 7700) / 7 = 1100 kcal
        Assert.Equal(1100, response.CaloricDeficitTarget);
        Assert.Contains("exceeds the 1% metabolic safety ceiling", response.WarningMessage);
    }
}
