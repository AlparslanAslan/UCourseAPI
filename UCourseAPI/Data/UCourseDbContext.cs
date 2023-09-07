using Microsoft.EntityFrameworkCore;
namespace UCourseAPI.Data
{
    public class UCourseDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer();
        }
    }
}
