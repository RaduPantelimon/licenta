using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ResourceApplicationTool.Models;
using ResourceApplicationTool.Utils;
namespace ResourceApplicationTool.Models.SecondaryModels
{
    public class MainSearchResult
    {
        public List<Employee> employeeSearchResults;
        public List<Department> departmentSearchResults;
        public List<Role> roleSearchResults;
        public List<Project> projectSearchResults;
    }
}