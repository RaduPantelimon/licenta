using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace ResourceApplicationTool.Models
{

    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;


    [MetadataType(typeof(DepartmentMD))]
    public partial class Department
    {
        public int employeesNumber;
        public int projectsNumber;
    }
    public class DepartmentMD
    {

    }
}