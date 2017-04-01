//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ResourceApplicationTool.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Skill
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Skill()
        {
            this.SkillLevels = new HashSet<SkillLevel>();
        }
    
        public int SkillID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<int> CategoryID { get; set; }
    
        public virtual SkillCategory SkillCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SkillLevel> SkillLevels { get; set; }
    }
}