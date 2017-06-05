using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ResourceApplicationTool.Models;
using ResourceApplicationTool.Models.ControllerModels;

using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;

namespace ResourceApplicationTool.Controllers
{
    public class HomeController : Controller
    {
        public ResourceApplicationTool.Models.RATV3Entities db = new RATV3Entities();
        public ActionResult Index()
        {
            HomeModel model = new HomeModel();

            List<Department> depts = db.Departments.AsQueryable().ToList();

            List<Project> projects = db.Projects.ToList();
            List<Employee> employees = db.Employees.ToList();

            foreach (Department dept in depts)
            {
                dept.projectsNumber = projects.Where(x => x.DepartmentID == dept.DepartmentID).Count();
                dept.employeesNumber = employees.Where(x => x.DepartmentID == dept.DepartmentID).Count();
            }

            DateTime currentDate = DateTime.Now;
            Employee employeeOmonth = employees.OrderByDescending(x => x.Tasks.Where(z =>z.StartDate.HasValue && z.StartDate.Value.Month == currentDate.Month)
            .Sum(y => (y.Estimation > 0) ? y.Estimation : 0)).FirstOrDefault();


            employees.Remove(employeeOmonth);

            //initialize baseUrl
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
            Request.ApplicationPath.TrimEnd('/') + "/";
            ViewBag.baseUrl = baseUrl;
            ViewBag.employeeOmonth = employeeOmonth;
            ViewBag.employees = employees;
            model.Departments = depts;
            return View(model);
        }

        public ActionResult NotFound()
        {
            ViewBag.Message = "Page or resource not found.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}