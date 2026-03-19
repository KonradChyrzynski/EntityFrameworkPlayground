using Microsoft.EntityFrameworkCore;

namespace Shared.API.Models
{
    class SharedDbContext : DbContext
    {
        public SharedDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<BudgetCategory> BudgetCategories { get; set; } = null!;
    }
}
