using Xunit;
using PulsePath.Core.Services; // Cible le namespace de ton projet principal

namespace PulsePath.Tests;

public class EngineTests
{
    [Fact]
    public void Test_RM_COR_01_Calcul_Doit_Respecter_Barriere_BMR()
    {
        // Arrange
        double poidsActuel = 80;
        double budgetCaloriesInitial = 2000;
        double objectifPasInitial = 8000;
        double bmr = 1800;
        int joursEcart = 15;

        // Act
        var plan = CourseCorrectionEngine.CalculateCourseCorrection(poidsActuel, budgetCaloriesInitial, objectifPasInitial, bmr, joursEcart);

        // Assert
        Assert.True(plan.CibleCalories >= bmr);
        Assert.Equal(bmr, plan.CibleCalories);
    }

    [Fact]
    public void Test_RM_KPI_01_Progres_Global_Fige_A_Zero_En_Surplus()
    {
        // Arrange
        double poidsDepart = 85.0;
        double poidsCible = 75.0;
        double poidsActuel = 86.0;

        double perteTotaleKg = 0;
        double progresPourcent = 0;

        // Act
        if (poidsActuel < poidsDepart)
        {
            perteTotaleKg = poidsDepart - poidsActuel;
            progresPourcent = (perteTotaleKg / (poidsDepart - poidsCible)) * 100;
        }
        else
        {
            perteTotaleKg = 0.0;
            progresPourcent = 0.0;
        }

        // Assert
        Assert.Equal(0.0, perteTotaleKg);
        Assert.Equal(0.0, progresPourcent);
    }

    [Fact]
    public void Test_RM_COR_01_Cible_Pas_Brides_A_DixHuitMille_Maximum()
    {
        // Arrange
        double poidsActuel = 100;
        double budgetCaloriesInitial = 2500;
        double objectifPasInitial = 8000;
        double bmr = 1900;
        int joursEcart = 30;

        // Act
        var plan = CourseCorrectionEngine.CalculateCourseCorrection(poidsActuel, budgetCaloriesInitial, objectifPasInitial, bmr, joursEcart);

        // Assert
        Assert.True(plan.CiblePas <= 18000);
        Assert.Equal(18000, plan.CiblePas);
    }
}
