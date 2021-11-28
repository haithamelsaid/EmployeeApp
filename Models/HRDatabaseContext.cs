using Microsoft.EntityFrameworkCore;

namespace CRUDApp.Models
{
    public class HrDatabaseContext : DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=initMigration;Trusted_Connection=True");
        }

    }
}
