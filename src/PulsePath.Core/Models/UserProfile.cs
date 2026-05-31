namespace PulsePath.Core.Models
{
    public class UserProfile
    {
        public bool IsMale { get; set; }
        public int Age { get; set; }
        public double HeightCm { get; set; }
        public double WeightDepart { get; set; }
        public double WeightCible { get; set; }
        public string Objectif { get; set; } = "maintien";
        public int SeancesPrevues { get; set; }
        public double BudgetCaloriesCible { get; set; }
    }
}