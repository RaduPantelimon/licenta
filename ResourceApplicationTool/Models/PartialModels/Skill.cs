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

    [MetadataType(typeof(SkillMD))]
    public partial class Skill
    {
      
    }

    public class SkillMD
    {
        [Required]
        [StringLength(50, ErrorMessage = "Please enter only 50 characters")]
        public string Title { get; set; }


        public string Description { get; set; }

        [Required]
        [Display(Name = "Skill Category")]
        public Nullable<int> CategoryID { get; set; }
    }
}