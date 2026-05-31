namespace PuslsePath.Core.Models;
public class PlanRattrapage
{
    public int DureeJours { get; set; } = 7;
    public double CibleCalories { get; set; }
    public double CiblePas { get; set; }
    public string Message { get; set; } = string.Empty;
    public string CardioLISSSuggere { get; set; } = string.Empty;
}