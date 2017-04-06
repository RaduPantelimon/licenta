using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ResourceApplicationTool.Utils
{
    public class Const
    {
        public partial class PermissionLevels
        {
            public static readonly string Employee = "Employee";
            public static readonly string Manager = "Manager";
            public static readonly string Administrator = "Administrator";
        }
        public partial class PicturePaths
        {
            public static readonly string PictureUrlRoot = "Content/Pictures/";
            public static readonly string ImgControllerRoot = "Images/";
            public static readonly string ProfilePictureUrl = "Content/Pictures/default-profile-picture.png";

            public static readonly string DepartmentPictureUrl = "Content/Pictures/default-profile-picture.png";
            public static readonly string DepartmentBannerPictureUrl = "Content/Pictures/departments-picture.png";
        }

        public partial class QueryStringParams
        {
            public static readonly string isModal = "isModal";
            public static readonly string employeeID = "employeeID";
        }

        public static readonly string[] SkillLevels = {"None","Theory", "Practice", "Proficient","Expert", "Guru"};
        public static readonly List<SelectListItem> Permissions = new List<SelectListItem>{
                    new SelectListItem{ Text="Employee", Value = "Employee" },
                    new SelectListItem{ Text="Manager", Value = "Manager" },
                    new SelectListItem { Text="Administrator", Value="Administrator" }
                 }; 
    }
}