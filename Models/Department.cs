//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Models.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Department
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Department()
        {
            this.Employees = new HashSet<Employee>();
            this.Projects = new HashSet<Project>();
        }
    
        public int DepartmentID { get; set; }
        public string Title { get; set; }
        public int MaxSize { get; set; }
        public string DeptDescription { get; set; }
        public System.DateTime StartDate { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public Nullable<System.Guid> BannerImageID { get; set; }
        public Nullable<System.Guid> MainImageID { get; set; }
    
        public virtual File File { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> Employees { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Project> Projects { get; set; }
        public virtual File File1 { get; set; }
    }
}