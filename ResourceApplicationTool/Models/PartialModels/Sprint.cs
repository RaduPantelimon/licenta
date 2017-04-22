using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Newtonsoft.Json;


namespace ResourceApplicationTool.Models
{

    [MetadataType(typeof(SprintMD))]
    public partial class Sprint
    {
    }

    public class SprintMD
    {

        [JsonIgnore]
        public virtual Project Project { get; set; }
        [JsonIgnore]
        public virtual ICollection<Task> Tasks { get; set; }
    }
}