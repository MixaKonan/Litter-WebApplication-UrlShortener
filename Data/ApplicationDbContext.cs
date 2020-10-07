using Microsoft.EntityFrameworkCore;
using Litter.Models;

namespace Litter.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<LitterModel> Litters { get; set; }
    }
}
