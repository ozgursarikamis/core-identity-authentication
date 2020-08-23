using Microsoft.EntityFrameworkCore;

namespace IdentityExample.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }
    }
}