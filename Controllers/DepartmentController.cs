using Microsoft.AspNetCore.Mvc;
using CRUDApp.Models;
using Microsoft.EntityFrameworkCore;


namespace CRUDApp.Controllers
{
    public class DepartmentController : Controller
    {
        public enum SortDirection
        {
            Ascending,
            Descending
        }
        private readonly HRDatabaseContext dbContext = new();

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Employees = this.dbContext.Employees.Include(e=>e.Department).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Index(string SortField, string CurrentSortField, SortDirection SortDirection,string searchByName)
        {
            var departments = GetDepartments();
            if (!string.IsNullOrEmpty(searchByName))
                departments = departments.Where(e => e.DepartmentName.ToLower().Contains(searchByName.ToLower()))
                    .ToList();
            return View(this.SortedDepartment(departments, SortField, CurrentSortField, SortDirection));
        }

        [HttpGet]
        public IActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateDepartment(Department model)
        {
            ModelState.Remove(nameof(model.DepartmentId));
            if (ModelState.IsValid)
            {
                dbContext.Departments.Add(model);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Departments = dbContext.Departments.ToList();
            return View();
        }


        

        public IActionResult Edit(int ID)
        {
            Department data = this.dbContext.Departments.FirstOrDefault(e => e.DepartmentId == ID);
            ViewBag.Departments = this.dbContext.Departments.ToList();
            return View("Index", data);
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
        private List<Department> GetDepartments()
        {
            var departments = dbContext.Departments.ToList();
            return departments;
        }

        private List<Department> SortedDepartment(List<Department> departments, string sortField, string currentSortField, SortDirection sortDirection)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                ViewBag.SortField = "DepartmentId";
                ViewBag.SortDirection = SortDirection.Ascending;
            }
            else
            {
                if (currentSortField == sortField)
                {
                    ViewBag.SortDirection = sortDirection == SortDirection.Ascending
                        ? SortDirection.Descending
                        : SortDirection.Ascending;
                }
                else
                {
                    ViewBag.SortDirection = SortDirection.Ascending;
                }

                ViewBag.SortField = sortField;

            }

            var propertyInfo = typeof(Department).GetProperty(ViewBag.sortField);

            if (ViewBag.SortDirection == SortDirection.Ascending)
                departments = departments.OrderBy(e => propertyInfo.GetValue(e, null)).ToList();
            else
                departments = departments.OrderByDescending(e => propertyInfo.GetValue(e, null)).ToList();
            return departments;
        }

        

    }

}
