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

    [MetadataType(typeof(EmployeeMD))]
    public partial class Employee
    {
        //used to send values to and from the model during binding and page rendering
        public IList<SkillLevel> SkillLevelsList { get; set; }
    }

    public class EmployeeMD
    {
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public Nullable<int> ManagerID { get; set; }
        [JsonIgnore]
        public Nullable<int> DepartmentID { get; set; }
        [JsonIgnore]
        public string CNP { get; set; }

        [JsonIgnore]
        public decimal Salary { get; set; }
        [JsonIgnore]
        public decimal PriorSalary { get; set; }
        [JsonIgnore]
        public Nullable<decimal> LastRaise { get; set; }
        [JsonIgnore]
        public Nullable<System.DateTime> TerminationDate { get; set; }
        public string Administrator { get; set; }
        public Nullable<System.Guid> ProfileImageID { get; set; }
        [JsonIgnore]
        public virtual Department Department { get; set; }
        [JsonIgnore]
        public virtual ICollection<Education> Educations { get; set; }
        [JsonIgnore]
        public virtual ICollection<Task> Tasks { get; set; }
        [JsonIgnore]
        public virtual ICollection<Employee> Employees1 { get; set; }
        [JsonIgnore]
        public virtual Employee Employee1 { get; set; }
        [JsonIgnore]
        public virtual Role Role { get; set; }
        [JsonIgnore]
        public virtual File File { get; set; }
        [JsonIgnore]
        public virtual ICollection<SkillLevel> SkillLevels { get; set; }
        [JsonIgnore]
        public virtual ICollection<Event> Events { get; set; }
        [JsonIgnore]
        public virtual ICollection<Attendant> Attendants { get; set; }
    }
}