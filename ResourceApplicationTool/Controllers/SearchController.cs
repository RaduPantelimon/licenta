using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net.Http;
using ResourceApplicationTool.Models;
using ResourceApplicationTool.Models.SecondaryModels;
using Newtonsoft.Json;
using System.Text;
using ResourceApplicationTool.Utils;
using System.Net;

namespace ResourceApplicationTool.Controllers
{
    public class SearchController : Controller
    {
        private RATV3Entities db = new RATV3Entities();

        // GET: Search
        public ActionResult Index(string query)
        {

            ViewBag.query = query;
            return View();
        }

        // GET: Search/GetPartial
        public JsonResult GetPartial(string query)
        {
            List<SearchResult> dummyResults = null;

            if (!String.IsNullOrEmpty(query) && query.Length >= 2 )
            {
                dummyResults = new List<SearchResult>{ new SearchResult("Albert Camus", "/Employees/Details/14", " Employee"),
                                                   new SearchResult("Albanezu Michi", "/Employees/Details/14", " Employee")};
            }
            else
            {
                dummyResults = new List<SearchResult>{ new SearchResult("Albert Camus", "/Employees/Details/14", " Employee"),
                                                   new SearchResult("Jean-Paul Sartre", "/Employees/Details/15", "Employee") };
            }

            return this.Json(dummyResults, JsonRequestBehavior.AllowGet);
        }


        #region Departments

        public void SearchForDepartments(MainSearchResult res, string query,bool retriveRelatedEmployees = true, bool retriveRelatedProjects = true)
        {
            query = query.ToLower();
            List<Department> resDepts = db.Departments.Where(d => d.Title.ToLower().Contains(query) || 
                                                            d.DeptDescription.ToLower().Contains(query)).ToList();
            res.departmentSearchResults.AddRange(resDepts);

            //we'll also get the employees associated to these departments
            if(retriveRelatedEmployees)
            {
                List<int> deptIds = resDepts.Select(d => d.DepartmentID).ToList();
                List<Employee> resEmp = db.Employees.Where(e => e.DepartmentID.HasValue && deptIds.Contains(e.DepartmentID.Value)).ToList();

                res.employeeSearchResults.AddRange(resEmp);
            }

            //we'll also get the proejcts associated to these departments
            if (retriveRelatedProjects)
            {
                List<int> deptIds = resDepts.Select(d => d.DepartmentID).ToList();
                List<Project> resProj = db.Projects.Where(p => p.DepartmentID.HasValue && deptIds.Contains(p.DepartmentID.Value)).ToList();

                res.projectSearchResults.AddRange(resProj);
            }
        }
        public void SearchForEmployees(MainSearchResult res, string query)
        {
            query = query.ToLower();

            List<Employee> resEmp = db.Employees.Where(e => (e.FirstName.ToLower().Contains(query) || 
            e.LastName.ToLower().Contains(query)) ||
            (e.FirstName + " " + e.LastName).ToLower().Contains(query) ||
            (e.LastName + " " + e.FirstName).ToLower().Contains(query) || 
            e.Email.ToLower().Contains(query) ||
            e.Account.ToLower().Contains(query) ||
            query.Contains(e.Account.ToLower()) ||
            query.Contains(e.Email.ToLower()) ||
            query.Contains(e.FirstName.ToLower()) ||
            query.Contains(e.LastName.ToLower()) 
            ).ToList();
            res.employeeSearchResults.AddRange(resEmp);
        }
        #endregion
    }
}