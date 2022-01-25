using CRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUDApp.Controllers
{
    public class EmployeeController : Controller
    {
        public enum SortDirection { Ascending, Descending }
        private readonly HrDatabaseContext dbContext = new HrDatabaseContext();

        [HttpGet]
        public IActionResult Index()
        {
            List<Employee> employees = dbContext.Employees.Include(e => e.Department).ToList();
            return View(employees);
        }
        [HttpPost]
        public IActionResult Index(string SortField, string CurrentSortField, SortDirection SortDirection, string searchByName)
        {
            var employees = GetEmployees();
            if (!string.IsNullOrEmpty(searchByName))
                employees = employees.Where(e => e.EmployeeName.ToLower().Contains(searchByName.ToLower())).ToList();
            return View(this.SortedEmployees(employees, SortField, CurrentSortField, SortDirection));
        }

        private List<Employee> GetEmployees()
        {
            ViewBag.employees = this.dbContext.Employees.Include(e => e.Department).ToList();
            return View(ViewBag.employees);
        }

        [HttpPost]
        public IActionResult CreateEmployee(Employee model)
        {
            ModelState.Remove("EmployeeID");
            ModelState.Remove("DepartmentName");
            if (ModelState.IsValid)
            {
                dbContext.Employees.Add(model);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Departments = this.dbContext.Departments.ToList();
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            Employee data = this.dbContext.Employees.Include(e => e.Department).FirstOrDefault(e => e.EmployeeId == Id);
            if (data == null)
                return View("Error");
            ViewBag.Departments = this.dbContext.Departments.ToList();
            return View("CreateEmployee", data);
        }


        [HttpPost]
        public IActionResult Edit(Employee employee)
        {
            ModelState.Remove("EmployeeId");
            ModelState.Remove("Department");
            ModelState.Remove("DepartmentName");
            if (ModelState.IsValid)
            {
                dbContext.Employees.Update(employee);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Departments = this.dbContext.Departments.ToList();
            return View("CreateEmployee", employee);
        }


        public IActionResult Delete(int ID)
        {
            Employee data = this.dbContext.Employees.FirstOrDefault(e => Equals(e.EmployeeId, ID));
            if (data != null)
            {
                dbContext.Employees.Remove(data);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult CreateEmployee()
        {
            ViewBag.Departments = this.dbContext.Departments.ToList();
            return View();
        }

        private List<Employee> SortedEmployees(List<Employee> employees, string sortField, string currentSortField, SortDirection sortDirection)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                ViewBag.SortField = "EmployeeNumber";
                ViewBag.SortDirection = SortDirection.Ascending;
            }
            else
            {
                if (currentSortField == sortField)
                {
                    ViewBag.SortDirection = sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
                }
                else
                {
                    ViewBag.SortDirection = SortDirection.Ascending;
                }
                ViewBag.SortField = sortField;

            }

            var propertyInfo = typeof(Employee).GetProperty(ViewBag.sortField);

            if (ViewBag.SortDirection == SortDirection.Ascending)
                employees = employees.OrderBy(e => propertyInfo.GetValue(e, null)).ToList();
            else
                employees = employees.OrderByDescending(e => propertyInfo.GetValue(e, null)).ToList();
            return employees;
        }

    }
}
