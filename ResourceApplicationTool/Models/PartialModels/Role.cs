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

    [MetadataType(typeof(RoleMD))]
    public partial class Role
    {
    }


    public class RoleMD
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Job Description")]
        public string JobDescription { get; set; }

        [Required]
        [Display(Name = "Average Salary")]
        [DataType(DataType.Currency)]
        public decimal AverageSalary { get; set; }
    }
}