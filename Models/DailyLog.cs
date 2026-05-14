namespace PulsePath_Engine_DotNet.Models
{
    public class DailyLog
    {
        public DateTime Date { get; set; }
        public double Weight { get; set; }
        public int CaloriesIn { get; set; }
        public int Steps { get; set; }
        public double SleepHours { get; set; }
        public int ProteinsIn { get; set; } // Ajout pour RM-GAM-01
        public bool FastingValidated { get; set; } // Ajout pour RM-GAM-01

    }
}
