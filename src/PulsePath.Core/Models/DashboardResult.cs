using PuslsePath.Core.Models;

namespace PulsePath.Core.Models
{
    public class DashboardResult
{
    public int JoursEcoules { get; set; }
    public double PerteTotaleKg { get; set; }
    public double ProgresPourcent { get; set; }
    public double TdeeDuJour { get; set; }
    public double CaloriesBruleesActivite { get; set; }
    public int TotalSeancesCetteSemaine { get; set; }
    public DateTime DateEstimee { get; set; }
    public int ScoreIntegrite { get; set; }
    public PlanRattrapage? PlanCorrection { get; set; }
    public string MessagePrescriptif { get; set; } = string.Empty;
}
}