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
    
    public partial class Education
    {
        public int EducationID { get; set; }
        public string Title { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string Degree { get; set; }
        public int EmployeeID { get; set; }
        public Nullable<int> Duration { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}