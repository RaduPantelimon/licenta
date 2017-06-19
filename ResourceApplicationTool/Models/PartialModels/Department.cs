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
        [Required]
        public string Title { get; set; }

        [Display(Name = "Max Size")]
        [Required]
        public int MaxSize { get; set; }

        [Display(Name = "Department Description")]
        public string DeptDescription { get; set; }

        [Display(Name = "Start Date")]
        [Required]
        [DataType(DataType.Date)]
        public System.DateTime StartDate { get; set; }

        [Display(Name = "Monthly Expenses")]
        [Required]
        [DataType(DataType.Currency)]
        public decimal MonthlyExpenses { get; set; }
    }
}