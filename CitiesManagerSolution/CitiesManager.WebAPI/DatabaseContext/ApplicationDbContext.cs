using CitiesManager.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.WebAPI.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public ApplicationDbContext()
        {
            
        }

        public virtual DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<City>().HasData(new City() 
            { 
                CityID = Guid.Parse("B4942D79-CBF1-48A2-B7E1-6EC23F2F2ECF"),
                CityName = "Belgrade"
            });

            modelBuilder.Entity<City>().HasData(new City()
            {
                CityID = Guid.Parse("779E0634-0604-466B-B540-3B92D19D970B"),
                CityName = "Kragujevac"
            });

        }
    }
}
