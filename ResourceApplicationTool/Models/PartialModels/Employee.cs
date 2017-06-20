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

        //used when validating the password
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class EmployeeMD
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Please enter only letters")]
        [StringLength(50,ErrorMessage ="Please enter only 50 characters")]
        public string Account { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20, ErrorMessage = "Please enter only 20 characters")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [JsonIgnore]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [JsonIgnore]
        [Required]
        [Display(Name = "Manager")]
        public Nullable<int> ManagerID { get; set; }

        [JsonIgnore]
        [Required]
        [Display(Name = "Department")]
        public Nullable<int> DepartmentID { get; set; }

        [JsonIgnore]
        [Required]
        [StringLength(13, MinimumLength = 13)]
        [RegularExpression(@"^\d{1,13}$", ErrorMessage = "Please enter 13 digits")]
        public string CNP { get; set; }
       
        [JsonIgnore]
        [Required]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }

        [JsonIgnore]
        public decimal PriorSalary { get; set; }

        [JsonIgnore]
        public Nullable<decimal> LastRaise { get; set; }

        [Display(Name = "Hire Date")]
        [Required]
        [DataType(DataType.Date)]
        public System.DateTime HireDate { get; set; }

        [JsonIgnore]
        [Display(Name = "Termination Date")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> TerminationDate { get; set; }

        [Display(Name = "Name Initial")]
        [StringLength(1, ErrorMessage = "Please enter only one letter")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Please enter only one letter")]
        public string MiddleInitial { get; set; }

        [Display(Name = "First Name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Please enter only letters")]
        [StringLength(50, ErrorMessage = "Please enter only 50 characters")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Please enter only letters")]
        [StringLength(50, ErrorMessage = "Please enter only 50 characters")]
        public string LastName { get; set; }

        [Display(Name = "Permissions")]
        [Required]
        public string Administrator { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required]
        [StringLength(50, ErrorMessage = "Please enter only 50 characters")]
        public string Email { get; set; }

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