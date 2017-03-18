using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResourceApplicationTool.Models.ControllerModels
{
    public class HomeModel
    {
        public virtual List<Project> Projects { get; set; }
        public virtual List<Department> Departments { get; set; }
        public virtual List<Employee> Employees { get; set; }
    }
}