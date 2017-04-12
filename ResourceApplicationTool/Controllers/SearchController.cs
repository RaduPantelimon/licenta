using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net.Http;
using ResourceApplicationTool.Models;
using ResourceApplicationTool.Models.SecondaryModels;
using ResourceApplicationTool.Utils;
using Newtonsoft.Json;
using System.Text;
using ResourceApplicationTool.Utils;
using System.Net;
using PagedList;

namespace ResourceApplicationTool.Controllers
{
    public class SearchController : Controller
    {
        private RATV3Entities db = new RATV3Entities();

        // GET: Search
        public ActionResult Index(string query,string filter, int? page, string secondaryFilter)
        {
            MainSearchResult searchRes = new MainSearchResult();

            bool filteredData = false;
            if (!String.IsNullOrEmpty(filter) && Const.SearchModelFilters.Any(x => x.ToLower() == filter.ToLower()))
            {
                ViewBag.filteredData = true;
                filteredData = true;
                
            }
            else
            {
                ViewBag.filteredData = false;
            }

            if(!String.IsNullOrEmpty(secondaryFilter))
            {
                ViewBag.secondaryFilter = secondaryFilter;
            }

            //executing the search queries
            if(!String.IsNullOrEmpty(query) && query.Length>=2)
            {
                SearchForDepartments(searchRes, query);
                SearchForEmployees(searchRes, query);
                SearchForProjects(searchRes, query);
                SearchForRoles(searchRes, query);

                ViewBag.searchDone = true;
            }
            else if(!String.IsNullOrEmpty(query) && query.Length > 0)
            {
                ViewBag.searchDone = false;
            }

            //checking if we will omit any data
            if ((searchRes.employeeSearchResults.Count >12 ||
                searchRes.employeeSearchResults.Count> 12 ||
                searchRes.employeeSearchResults.Count > 12) && !filteredData)
            {
                ViewBag.dataIsOmitted = true;

                //limiting the number of results shown
                searchRes.employeeSearchResults = searchRes.employeeSearchResults.Take(12).ToList();
                searchRes.departmentSearchResults = searchRes.departmentSearchResults.Take(12).ToList();
                searchRes.projectSearchResults = searchRes.projectSearchResults.Take(12).ToList();
            }

            searchRes.initializeQuickSearchResults();

            ViewBag.query = query;
            ViewBag.filter = filter;

            //initialize baseUrl
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
            Request.ApplicationPath.TrimEnd('/') + "/";
            ViewBag.baseUrl = baseUrl;


            if(filteredData)
            {
                int pageSize =Const.SearchParams.pageSize;
                int pageNumber = (page ?? 1);
                if (!String.IsNullOrEmpty(secondaryFilter))
                {
                    //applying project filters
                    #region ProjectFilters
                    if (secondaryFilter == "project_name")
                    {
                        searchRes.projectSearchResults = searchRes.projectSearchResults.OrderBy(x => x.Title).ToList();
                    }
                    else if (secondaryFilter == "project_name_desc")
                    {
                        searchRes.projectSearchResults = searchRes.projectSearchResults.OrderByDescending(x => x.Title).ToList();
                    }
                    else if (secondaryFilter == "project_department")
                    {
                        searchRes.projectSearchResults = searchRes.projectSearchResults.OrderBy(x => x.Department.Title).ToList();
                    }
                    else if (secondaryFilter == "project_department_desc")
                    {
                        searchRes.projectSearchResults = searchRes.projectSearchResults.OrderByDescending(x => x.Department.Title).ToList();
                    }
                    else if (secondaryFilter == "project_start_date")
                    {
                        searchRes.projectSearchResults = searchRes.projectSearchResults.OrderBy(x => x.StartDate).ToList();

                    }
                    else if (secondaryFilter == "project_start_date_desc")
                    {
                        searchRes.projectSearchResults = searchRes.projectSearchResults.OrderByDescending(x => x.StartDate).ToList();
                    }
                    else if (secondaryFilter == "project_end_date")
                    {
                        searchRes.projectSearchResults = searchRes.projectSearchResults.OrderBy(x => x.EndDate).ToList();
                    }
                    else if (secondaryFilter == "project_end_date_desc")
                    {
                        searchRes.projectSearchResults = searchRes.projectSearchResults.OrderByDescending(x => x.EndDate).ToList();
                    }
                    #endregion
                    //applying department filters
                    #region DepartmentFilters
                    if (secondaryFilter == "department_name")
                    {
                        searchRes.departmentSearchResults = searchRes.departmentSearchResults.OrderBy(x => x.Title).ToList();
                    }
                    else if (secondaryFilter == "department_name_desc")
                    {
                        searchRes.departmentSearchResults = searchRes.departmentSearchResults.OrderByDescending(x => x.Title).ToList();
                    }
                    else if (secondaryFilter == "department_start_date")
                    {
                        searchRes.departmentSearchResults = searchRes.departmentSearchResults.OrderBy(x => x.StartDate).ToList();
                    }
                    else if (secondaryFilter == "department_start_date_desc")
                    {
                        searchRes.departmentSearchResults = searchRes.departmentSearchResults.OrderByDescending(x => x.StartDate).ToList();
                    }
                    else if (secondaryFilter == "department_employees_number")
                    {
                        searchRes.departmentSearchResults = searchRes.departmentSearchResults.Where(x => x.Employees == null).Concat(
                            searchRes.departmentSearchResults.Where(x => x.Employees != null).OrderBy(x => x.Employees.Count())).ToList();

                    }
                    else if (secondaryFilter == "department_employees_number_desc")
                    {
                        searchRes.departmentSearchResults = searchRes.departmentSearchResults.Where(x => x.Employees != null).OrderBy(x => x.Employees.Count()).Concat(
                            searchRes.departmentSearchResults.Where(x => x.Employees == null)).ToList();
                    }
                    else if (secondaryFilter == "department_description")
                    {
                        searchRes.departmentSearchResults = searchRes.departmentSearchResults.OrderBy(x => x.DeptDescription).ToList();
                    }
                    else if (secondaryFilter == "department_description_desc")
                    {
                        searchRes.departmentSearchResults = searchRes.departmentSearchResults.OrderByDescending(x => x.DeptDescription).ToList();
                    }
                    #endregion
                    //applying employee filters
                    #region EmployeeFilters
                    if (secondaryFilter == "employee_name")
                    {
                        searchRes.employeeSearchResults = searchRes.employeeSearchResults.OrderBy(x => x.FirstName + " " + x.LastName).ToList();
                    }
                    else if(secondaryFilter == "employee_name_desc")
                    {
                        searchRes.employeeSearchResults = searchRes.employeeSearchResults.OrderByDescending(x => x.FirstName + " " + x.LastName).ToList();
                    }
                    else if (secondaryFilter == "employee_department")
                    {
                        searchRes.employeeSearchResults = searchRes.employeeSearchResults.OrderBy(x => x.Department.Title).ToList();
                    }
                    else if (secondaryFilter == "employee_department_desc")
                    {
                        searchRes.employeeSearchResults = searchRes.employeeSearchResults.OrderByDescending(x => x.Department.Title).ToList();
                    }
                    else if (secondaryFilter == "employee_role")
                    {
                        searchRes.employeeSearchResults = searchRes.employeeSearchResults.Where(x => x.Role == null).Concat(
                            searchRes.employeeSearchResults.Where(x => x.Role != null).OrderBy(x => x.Role.Name)).ToList();
                        
                    }
                    else if (secondaryFilter == "employee_role_desc")
                    {
                        searchRes.employeeSearchResults = searchRes.employeeSearchResults.Where(x => x.Role != null).OrderBy(x => x.Role.Name).Concat(
                            searchRes.employeeSearchResults.Where(x => x.Role == null)).ToList();
                    }
                    else if (secondaryFilter == "employee_date")
                    {
                        searchRes.employeeSearchResults = searchRes.employeeSearchResults.OrderBy(x => x.HireDate).ToList();
                    }
                    else if (secondaryFilter == "employee_date_desc")
                    {
                        searchRes.employeeSearchResults = searchRes.employeeSearchResults.OrderByDescending(x => x.HireDate).ToList();
                    }
                    #endregion
                }

                //depending on the selected filter, we will initialize a pagedList
                if (filter.ToLower() == Const.SearchParams.EmployeeFilterName.ToLower())
                {
                    searchRes.pagedEmployees = searchRes.employeeSearchResults.ToPagedList(pageNumber, pageSize);
                }
                else if( filter.ToLower() == Const.SearchParams.DepartmentFilterName.ToLower())
                {
                    searchRes.pagedDepartments = searchRes.departmentSearchResults.ToPagedList(pageNumber, pageSize);
                }
                else if( filter.ToLower() == Const.SearchParams.ProjectFilterName.ToLower())
                {
                    searchRes.pagedProjects = searchRes.projectSearchResults.ToPagedList(pageNumber, pageSize);
                }

            }

            return View(searchRes);
        }

        // GET: Search/GetPartial
        public JsonResult GetPartial(string query)
        {
            MainSearchResult searchRes = new MainSearchResult();

            //executing the search queries
            SearchForDepartments(searchRes,query);
            SearchForEmployees(searchRes,query);
            SearchForProjects(searchRes,query);
            SearchForRoles(searchRes,query);

            searchRes.initializeQuickSearchResults();

            return this.Json(searchRes.quickSearch, JsonRequestBehavior.AllowGet);
        }


        #region Queries

        public void SearchForDepartments(MainSearchResult res, string query,bool retriveRelatedEmployees = true, bool retriveRelatedProjects = true)
        {
            query = query.ToLower();
            List<Department> resDepts = db.Departments.Where(d => d.Title.ToLower().Contains(query)).ToList();
            res.departmentSearchResults.AddRange(resDepts);
            res.departmentSearchResults = res.departmentSearchResults.Distinct().ToList();

            //we'll also get the employees associated to these departments
            if (retriveRelatedEmployees)
            {
                List<int> deptIds = resDepts.Select(d => d.DepartmentID).ToList();
                List<Employee> resEmp = db.Employees.Where(e => e.DepartmentID.HasValue && deptIds.Contains(e.DepartmentID.Value)).ToList();

                res.employeeSearchResults.AddRange(resEmp);
                res.employeeSearchResults = res.employeeSearchResults.Distinct().ToList();
            }

            //we'll also get the proejcts associated to these departments
            if (retriveRelatedProjects)
            {
                List<int> deptIds = resDepts.Select(d => d.DepartmentID).ToList();
                List<Project> resProj = db.Projects.Where(p => p.DepartmentID.HasValue && deptIds.Contains(p.DepartmentID.Value)).ToList();

                res.projectSearchResults.AddRange(resProj);
                res.projectSearchResults = res.projectSearchResults.Distinct().ToList();
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
            res.employeeSearchResults = res.employeeSearchResults.Distinct().ToList();
        }

        public void SearchForRoles (MainSearchResult res, string query)
        {
            query = query.ToLower();

            List<Role> roles = db.Roles.Where(r => (r.Name.ToLower().Contains(query))).ToList();
            List<int> rolesIds = roles.Select(r => r.RoleID).ToList();
            List<Employee> resEmp = db.Employees.Where(e => e.DepartmentID.HasValue && rolesIds.Contains(e.DepartmentID.Value)).ToList();

            res.employeeSearchResults.AddRange(resEmp);
            res.employeeSearchResults = res.employeeSearchResults.Distinct().ToList();
        }

        public void SearchForProjects(MainSearchResult res, string query)
        {
            query = query.ToLower();

            List<Project> resProj = db.Projects.Where(p => p.Title.ToLower().Contains(query)).ToList();
            res.projectSearchResults.AddRange(resProj);
            res.projectSearchResults = res.projectSearchResults.Distinct().ToList();

        }
        #endregion
    }
}