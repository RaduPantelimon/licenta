using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResourceApplicationTool.Models
{
    public class SkillLevelCollectionPostModel
    {
        public List<SkillLevel> SkillLevelsList { get; set; }
        public SkillLevelCollectionPostModel()
        {
            SkillLevelsList = new List<SkillLevel>();
        }
    }
}