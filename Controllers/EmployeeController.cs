using Microsoft.AspNetCore.Mvc;
using CRUDApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRUDApp.Controllers
{
    public class EmployeeController : Controller
    {   
        public enum SortDirection { Ascending,Descending}
        private readonly HRDatabaseContext dbContext = new();
       
        private List<Employee> GetEmployees()
        {

            var employees = dbContext.Employees.ToList();
            return employees;
        }
        public IActionResult Index(string SortField, string CurrentSortField, SortDirection SortDirection, string searchByName)
        {
            var employees = GetEmployees();
            if (!string.IsNullOrEmpty(searchByName))
                employees = employees.Where(e => e.EmployeeName.ToLower().Contains(searchByName.ToLower())).ToList();
            return View(this.SortedEmployees(employees, SortField, CurrentSortField, SortDirection));
        }

        [HttpGet]

        public IActionResult CreateEmployee()
        {

            return View();
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


        public IActionResult Edit(int ID)
        {
            Employee data = this.dbContext.Employees.FirstOrDefault(e => e.EmployeeID == ID); 
            return View("CreateEmployee",data);
        }

        public IActionResult Delete(int ID)
        {
            Employee data = this.dbContext.Employees.FirstOrDefault(e => e.EmployeeID == ID);
            if (data != null)
            {
                dbContext.Employees.Remove(data);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
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
