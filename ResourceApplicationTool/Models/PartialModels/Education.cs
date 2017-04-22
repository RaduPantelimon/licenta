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
    }
}