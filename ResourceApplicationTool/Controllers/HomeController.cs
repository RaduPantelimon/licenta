using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ResourceApplicationTool.Models;
using ResourceApplicationTool.Models.ControllerModels;
using ResourceApplicationTool.Models.SecondaryModels;

using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

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

            //latest employees
            List<Employee> latestEmployees = employees.OrderByDescending(x => x.HireDate).Take(12).ToList();

            //biggest projects
            foreach (Project p in projects)
            {
                //selecting the correct tasks for each project
                List<Task> projectTasks = db.Tasks.Where(x => x.Sprint != null && x.Sprint.ProjectID == p.ProjectID).ToList();
                List<EmployeeEffort> participants = new List<EmployeeEffort>();

                foreach (Task t in projectTasks)
                {
                    if (t.Estimation.HasValue)
                    {
                        EmployeeEffort participant = participants.Where(x => t.EmployeeID == x.EmployeeID).FirstOrDefault();
                        if (participant == null)
                        {
                            participant = new EmployeeEffort();
                            try
                            {
                                if (t.EmployeeID.HasValue)  participant.EmployeeID = t.EmployeeID.Value;
                                participant.EffortInHours = t.Estimation.Value;
                                participant.EmployeeName = t.Employee.FirstName + " " + t.Employee.LastName;
                                participants.Add(participant);
                            }
                            catch (Exception ex)
                            {
                                //handle error
                            }
                        }
                        else
                        {
                            participant.EffortInHours += t.Estimation.Value;
                        }

                        p.ManHoursEffort += t.Estimation.Value;
                    }
                }
                p.participants = participants;
            }

            List<Project> biggestProjects = projects.OrderByDescending(x => x.ManHoursEffort).Take(3).ToList();
            string projectsStatistics = JsonConvert.SerializeObject(biggestProjects,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });



            //initialize baseUrl
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
            Request.ApplicationPath.TrimEnd('/') + "/";
            ViewBag.baseUrl = baseUrl;
            ViewBag.employeeOmonth = employeeOmonth;
            ViewBag.employees = latestEmployees;
            ViewBag.biggestProjects = biggestProjects;
            ViewBag.projectStatistics = projectsStatistics;
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