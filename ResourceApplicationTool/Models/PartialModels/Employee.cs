using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResourceApplicationTool.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public partial class Employee
    {
        //used to send values to and from the model during binding and page rendering
        public IList<SkillLevel> SkillLevelsList { get; set; }
    }
}