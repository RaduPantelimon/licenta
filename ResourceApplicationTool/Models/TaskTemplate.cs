using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResourceApplicationTool.Models
{
    public class TaskTemplate
    {
        public int templateTaskID { get; set; }
        public int employeeID { get; set; }
        public string startDate { get; set; }
        public int sprintID { get; set;}
        public int duration { get; set;}
        public int directDescendant { get; set; }
    }
}