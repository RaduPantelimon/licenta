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

        [Required]
        [DataType(DataType.Date)]
        public System.DateTime StartTime { get; set; }

        [DataType(DataType.Date)]
        public Nullable<System.DateTime> EndTime { get; set; }

        [Display(Name = "Event Type")]
        public string EventType { get; set; }

        public string Location { get; set; }

        [Display(Name = "Creator")]
        [Required]
        public int CreatorID { get; set; }

        [Required]
        public string Title { get; set; }
    }
}