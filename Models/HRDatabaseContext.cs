using Microsoft.EntityFrameworkCore;

namespace CRUDApp.Models
{
    public class HRDatabaseContext:DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"data source=(localdb)\mssqllocaldb;initial catalog=EmployeesDB;integrated security=SSPI;");
        }
    }
}
