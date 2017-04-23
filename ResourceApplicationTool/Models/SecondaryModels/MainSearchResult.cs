using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ResourceApplicationTool.Models;
using ResourceApplicationTool.Utils;
using PagedList;
namespace ResourceApplicationTool.Models.SecondaryModels
{
    public class MainSearchResult
    {
        public List<Employee> employeeSearchResults;
        public List<Department> departmentSearchResults;
        public List<Role> roleSearchResults;
        public List<Project> projectSearchResults;
        public List<SearchResult> quickSearch;

        public IPagedList<Employee> pagedEmployees;
        public IPagedList<Department> pagedDepartments;
        public IPagedList<Project> pagedProjects;

        public MainSearchResult() {
            employeeSearchResults = new List<Employee>();
            departmentSearchResults = new List<Department>();
            projectSearchResults = new List<Project>();

            quickSearch = new List<SearchResult>();
        }

        



        public void initializeQuickSearchResults()
        {
            List<SearchResult> employeesQuickSearch = employeeSearchResults.Select(x => new SearchResult(x.FirstName + " " + x.LastName,
                "/Employees/Details/" + x.EmployeeID,"Employee")).ToList();
            List<SearchResult> departmentsQuickSearch = departmentSearchResults.Select(x => new SearchResult(x.Title,
                    "/Departments/Details/" + x.DepartmentID, "Department")).ToList();
            List<SearchResult> projectsQuickSearch = projectSearchResults.Select(x => new SearchResult(x.Title,
                "/Projects/Details/" + x.ProjectID, "Project")).ToList();

            quickSearch.AddRange(employeesQuickSearch);
            quickSearch.AddRange(departmentsQuickSearch);
            quickSearch.AddRange(projectsQuickSearch);


        }

        public List<SearchResult> getEmployeeResults()
        {
            List<SearchResult> employeesQuickSearch = employeeSearchResults.Select(x => new SearchResult(x.FirstName + " " + x.LastName,
               "/Employees/Details/" + x.EmployeeID, "Employee")).ToList();

            return employeesQuickSearch;
        } 
    }
}