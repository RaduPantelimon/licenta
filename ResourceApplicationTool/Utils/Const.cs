using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        }
    }
}