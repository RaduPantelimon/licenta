using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Newtonsoft.Json;


namespace ResourceApplicationTool.Models
{

    [MetadataType(typeof(EducationMD))]
    public partial class Education
    {
    }

    public class EducationMD
    {
        [JsonIgnore]
        public virtual Employee Employee { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public System.DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public System.DateTime EndDate { get; set; }

        public string Degree { get; set; }

        [Display(Name = "Employee")]
        public int EmployeeID { get; set; }
    }
}