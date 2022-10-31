
using Microsoft.EntityFrameworkCore;

using UserService.Entities.Identity;

namespace UserService.Data
{
    public class IdentityDataContext: DbContext
    {
        public IdentityDataContext(DbContextOptions<IdentityDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(builder);
            modelBuilder.Entity<User>().ToTable("Users");           
        }

        public DbSet<User> Users { get; set; }       
    }
}
