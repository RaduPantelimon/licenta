using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Newtonsoft.Json;


namespace ResourceApplicationTool.Models
{
    [MetadataType(typeof(TaskMD))]
    public  partial class Task
    {
    }

    public class TaskMD
    {

        [JsonIgnore]
        public virtual Employee Employee { get; set; }
        [JsonIgnore]
        public virtual Role Role { get; set; }
        [JsonIgnore]
        public virtual Sprint Sprint { get; set; }
    }
}