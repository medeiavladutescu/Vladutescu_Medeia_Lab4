using Microsoft.EntityFrameworkCore;
using Vladutescu_Medeia_Lab4.Models;

namespace Vladutescu_Medeia_Lab4.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<PredictionHistory> PredictionHistories { get; set; }
    }

}
