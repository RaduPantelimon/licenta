using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ResourceApplicationTool.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [MetadataType(typeof(ProjectMD))]
    public partial class Project
    {
        //used to send values to and from the model during binding and page rendering
        public int ManHoursEffort { get; set; }
        public int ManDaysEffort { get
            {
                return ManHoursEffort / 8;
            }
        }
    }

    public class ProjectMD
    {
        [Display(Name = "Start Date")]
        public System.DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public System.DateTime EndDate { get; set; }
        [Display(Name = "Contract Number")]
        public string ContractNumber { get; set; }
        [Display(Name = "Project Description")]
        public string PJDescription { get; set; }
        [Display(Name = "Department")]
        public Nullable<int> DepartmentID { get; set; }
    }
}