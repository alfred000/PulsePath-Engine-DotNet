using Microsoft.EntityFrameworkCore;
using PulsePath_Engine_DotNet.Models;

namespace PulsePath_Engine_DotNet.Data;

public class PulsePathContext : DbContext
{
    public DbSet<DailyLog> DailyLogs { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Spécifie l'utilisation d'une base de données SQLite locale nommée pulsepath.db
        optionsBuilder.UseSqlite("Data Source=pulsepath.db");
    }
}
