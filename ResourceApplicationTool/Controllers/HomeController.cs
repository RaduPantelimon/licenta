﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ResourceApplicationTool.Models;
using ResourceApplicationTool.Models.ControllerModels;
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

            model.Departments = depts;
            return View(model);
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