using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ResourceApplicationTool.Models;
using ResourceApplicationTool.Models.SecondaryModels;

namespace ResourceApplicationTool.Utils
{
    public partial class Const
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

        public partial class SearchParams
        {
            public static readonly int pageSize = 3;
            public static readonly string EmployeeFilterName = "Employee";
            public static readonly string DepartmentFilterName = "Department";
            public static readonly string ProjectFilterName = "Project";
        }

        public partial class CLAIM
        {
            //Employees
            public static string CLAIM_NAMESPACE = "resourceallocationtool";
            public static string USER_ACCOUNT = "UserAccount";
            public static string USER_ID = "UserID";
            public static string USER_ACCESS_LEVEL = "UserAccessLevel";
            public static string USER_FIRST_NAME = "UserFirstName";
            public static string USER_LAST_NAME = "UserLastName";
            public static string USER_ROLE = "UserRole";
            public static string USER_DEPARTMENT = "UserDepartment";
        }
        public partial class Fields
        {
            //Employees Fields
            public static string EMPLOYEE_FIRST_NAME = "FirstName";
            public static string EMPLOYEE_PASWORD = "Password";
            public static string EMPLOYEE_ACCOUNT = "Account";
            public static string EMPLOYEE_LAST_NAME = "LastName";
            public static string EMPLOYEE_ID = "EmployeeID";
            public static string EMPLOYEE_MANAGER_ID = "ManagerID";
            public static string EMPLOYEE_DEPARTMENT_ID = "DepartmentID";
            public static string EMPLOYEE_ROLE_ID = "RoleID";
            public static string EMPLOYEE_TITLE = "Title";
            public static string EMPLOYEE_CNP = "CNP";
            public static string EMPLOYEE_SALARY = "Salary";
            public static string EMPLOYEE_PRIOR_SALARY = "PriorSalary";
            public static string EMPLOYEE_HIRE_DATE = "HireDate";
            public static string EMPLOYEE_TERMINATION_DATE = "TerminationDate";
            public static string EMPLOYEE_ACCESS = "Administrator";
            public static string EMPLOYEE_MIDDLE_NAME_INITIAL = "MiddleInitial";
            public static string EMPLOYEE_EMAIL = "Email";
            public static string EMPLOYEE_PHONENUMBER = "PhoneNumber";
        }

        public static readonly string[] SkillLevels = {"None","Theory", "Practice", "Proficient","Expert", "Guru"};
        public static readonly string[] SearchModelFilters = { "Employee", "Project", "Department"};

        public static readonly List<SelectListItem> Permissions = new List<SelectListItem>{
                    new SelectListItem{ Text="Employee", Value = "Employee" },
                    new SelectListItem{ Text="Manager", Value = "Manager" },
                    new SelectListItem { Text="Administrator", Value="Administrator" }
                 };

        public static readonly List<SelectListItem> EventTypes = new List<SelectListItem>{
                    new SelectListItem{ Text="", Value = "" },
                    new SelectListItem{ Text="Audit", Value = "Audit" },
                    new SelectListItem{ Text="Performance Review", Value = "Performance Review" },
                    new SelectListItem { Text="Sprint Review Meeting", Value="Sprint Review Meeting" },
                    new SelectListItem { Text="Daily Scrum Meeting", Value="Daily Scrum Meeting" },
                    new SelectListItem { Text="Department Monthly Meeting", Value="Department Monthly Meeting" }
                 };

        public static readonly List<EventTypeInfo> EventTypesinfo = new List<EventTypeInfo>{
                    new EventTypeInfo{ EventType="", DetailInfo = "If left empty, no email will be sent", ViewName="", DisplayWarning=true },
                    new EventTypeInfo{ EventType="Audit", DetailInfo = "Audit",  ViewName="Audit", DisplayWarning=false  },
                    new EventTypeInfo{ EventType="Performance Review", DetailInfo = "Performance Review for a certain Employee. You will only be able to add one Attendant",
                        ViewName="PerformanceReview", DisplayWarning =true },
                    new EventTypeInfo { EventType="Sprint Review Meeting", DetailInfo = "Sprint Review Meeting", ViewName="SprintReviewMeeting", DisplayWarning=false },
                    new EventTypeInfo { EventType="Daily Scrum Meeting", DetailInfo = "Daily Scrum Meeting", ViewName="DailyScrumMeeting", DisplayWarning=false },
                    new EventTypeInfo { EventType="Department Monthly Meeting",
                        DetailInfo = "Will automatically attach a report of your department, for the current month",
                        ViewName="DepartmentMonthlyMeeting",
                        DisplayWarning =true }
                 };
    }
}