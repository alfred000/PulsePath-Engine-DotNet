namespace PulsePath.Core.Dtos;

public class DailyLogDto
{
    public double Weight { get; set; }
    public int CaloriesIn { get; set; }
    public int Steps { get; set; }
    public double SleepHours { get; set; }
    public int ProteinsIn { get; set; }
    public bool FastingValidated { get; set; }
    public int WorkoutsDone { get; set; }
}
