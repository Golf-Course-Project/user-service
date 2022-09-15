
using Microsoft.EntityFrameworkCore;

namespace UserService.Data
{
    public class IdentityDataContextForSp : DbContext
    {
        public IdentityDataContextForSp(DbContextOptions<IdentityDataContextForSp> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {         

        }

        //public DbSet<Tokens> Tokens { get; set; }       
    }
}
