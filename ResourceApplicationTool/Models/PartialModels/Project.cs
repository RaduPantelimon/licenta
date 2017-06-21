using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ResourceApplicationTool.Models.SecondaryModels;

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

        public List<EmployeeEffort> participants;
    }

    public class ProjectMD
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public System.DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public System.DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Contract Number")]
        public string ContractNumber { get; set; }

        [Required]
        [Display(Name = "Project Description")]
        public string PJDescription { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Budget { get; set; }

        [Display(Name = "Department")]
        public Nullable<int> DepartmentID { get; set; }

        [JsonIgnore]
        public virtual Department Department { get; set; }
        [JsonIgnore]
        public virtual ICollection<Sprint> Sprints { get; set; }
        [JsonIgnore]
        public virtual Contact Contact1 { get; set; }
    }
}