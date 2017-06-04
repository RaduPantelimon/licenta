using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ResourceApplicationTool.Models
{
    [MetadataType(typeof(EventMD))]
    public partial class Event
    {
        public bool isCreator;
        public bool isAttendant;
    }

    public class EventMD
    {
        [Display(Name = "Creator")]
        public virtual Employee Employee { get; set; }
    }
}