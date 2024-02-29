using CitiesManager.Core.Entities;
using CitiesManager.Core.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.Infrastructure.DatabaseContext
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public ApplicationDbContext()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<City> Cities { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
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
