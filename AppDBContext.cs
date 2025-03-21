using LoginJwt.models;
using Microsoft.EntityFrameworkCore;

namespace LoginJwt
{
    public class AppDBContext :DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext>options):base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
}
