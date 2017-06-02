using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace ResourceApplicationTool.Models
{

    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;


    [MetadataType(typeof(DepartmentMD))]
    public partial class Department
    {
        public int employeesNumber;
        public int projectsNumber;
    }
    public class DepartmentMD
    {
        [Display(Name = "Max Size")]
        public int MaxSize { get; set; }
        [Display(Name = "Department Description")]
        public string DeptDescription { get; set; }
        [Display(Name = "Start Date")]
        public System.DateTime StartDate { get; set; }
        [Display(Name = "Monthly Expenses")]
        public decimal MonthlyExpenses { get; set; }
    }
}