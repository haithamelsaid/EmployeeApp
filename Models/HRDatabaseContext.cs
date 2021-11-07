using Microsoft.EntityFrameworkCore;

namespace CRUDApp.Models
{
    public class HRDatabaseContext:DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"data source=DESKTOP-SU3Q476;initial catalog=EmployeesDB;integrated security=SSPI;");
        }
    }
}
