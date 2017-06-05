using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ResourceApplicationTool.Models;
using System.IO;
using System.Security.Principal;

using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.Security.Claims;

namespace ResourceApplicationTool.Utils
{
    public class Common
    {
        private static RATV3Entities db = new RATV3Entities();

        public static void GeneratePDFViewBag(int id, dynamic ViewBag)
        {
            Event @event = db.Events.Find(id);
            if (@event.EventType == "Performance Review" && @event.Attendants != null && @event.Attendants.Count > 0)
            {
                #region PrepareViewBag
                Attendant at = @event.Attendants.FirstOrDefault();
                Employee employee = db.Employees.Find(at.EmployeeID);
                string profilePicUrl = "";

                //getting the picture ready
                Common.CreateSkillTemplates(employee);
                if (employee.File != null)
                {
                    profilePicUrl = Utils.Common.SaveImgLocally(db, employee.File.FileID.ToString());
                    ViewBag.profilePicUrl = profilePicUrl;

                }
                else
                {
                    ViewBag.profilePicUrl = System.Web.HttpContext.Current.Server.MapPath("~/Content/Pictures/");
                }

                //getting the projects for this employee
                List<Task> tasks = db.Tasks.Include(x => x.Sprint).Where(x => x.EmployeeID == employee.EmployeeID).ToList();
                List<Project> projects = (from t in db.Tasks
                                          join s in db.Sprints on t.SprintID equals s.SprintID
                                          join p in db.Projects on s.ProjectID equals p.ProjectID
                                          where t.EmployeeID == employee.EmployeeID
                                          select p).Distinct().ToList();

                foreach (Project p in projects)
                {
                    //selecting the correct tasks for each project
                    List<Task> projectTasks = tasks.Where(x => x.Sprint != null && x.Sprint.ProjectID == p.ProjectID).ToList();
                    foreach (Task t in projectTasks)
                    {
                        if (t.Estimation.HasValue)
                        {
                            p.ManHoursEffort += t.Estimation.Value;
                        }

                    }

                }


                ViewBag.projects = projects;
                ViewBag.headertext = "CV: " + employee.FirstName + " " + employee.LastName;



                //initializing CV
                employee.SkillLevelsList = employee.SkillLevels.ToList();
                ViewBag.SkillCategories = db.SkillCategories.OrderByDescending(x => x.Skills.Count).ToList();
                #endregion

            }
        }

        public static Guid? CreateImage(HttpPostedFileBase uploadPicture)
        {
            try
            {
                Guid avatarGuid = System.Guid.NewGuid();
                var avatar = new Models.File
                {
                    FileNumber = System.IO.Path.GetFileName(uploadPicture.FileName),
                    FileID = avatarGuid,
                    FileDescription = uploadPicture.ContentType
                };

                avatar.FileNumber = avatar.FileNumber.Substring(0, Math.Min(avatar.FileNumber.Length, 45));
                using (var reader = new System.IO.BinaryReader(uploadPicture.InputStream))
                {
                    avatar.ItemImage = reader.ReadBytes(uploadPicture.ContentLength);
                }
                db.Files.Add(avatar);
                db.SaveChanges();
                return avatarGuid;
            }
            catch (Exception ex)
            {
                //log
            }
            return null;
        }

        public static string SaveImgLocally(RATV3Entities db, string guid)
        {
            string path = "";
            try
            {
                //get file by GUID
                Models.File fileToRetrieve = db.Files.Where(x => (x.FileID.ToString() == guid)).FirstOrDefault();

                //if getting the file by ID did not work, we'll try to get it by its Title
                if (fileToRetrieve == null)
                {
                    fileToRetrieve = db.Files.Where(x => (x.FileNumber == guid)).FirstOrDefault();
                }
                if (fileToRetrieve != null)
                {

                    //file successfully retrieved
                    byte[] file = fileToRetrieve.ItemImage;
                    //generating the name under which we will save the file locally for the PDF Generation
                    string fileExtension = fileToRetrieve.FileNumber.Split('.').Last();
                    string uniqueComponent = Guid.NewGuid().ToString();

                    string imgPath = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Temp/"),
                                       uniqueComponent + "." + fileExtension).ToString();

                    using (var fileStream = new FileStream(imgPath, FileMode.Create, FileAccess.Write))
                    {
                        fileStream.Write(fileToRetrieve.ItemImage,0,fileToRetrieve.ItemImage.Length);
                    }

                    path = imgPath;
                }
               
            }
            catch(Exception ex)
            {
                //could not save img locally
                //error handling
            }


            return path;
        }
        public static void DeleteLocalImage(string imgPath)
        {
            if (System.IO.File.Exists(imgPath)) { System.IO.File.Delete(imgPath); }
        }


        public static void CreateSkillTemplates(Employee emp)
        {
            try
            {

                List<Skill> skills = db.Skills.ToList();

                foreach(Skill skill in skills)
                {
                        SkillLevel sk = db.SkillLevels.Where(x => x.SkillID == skill.SkillID && x.EmployeeID == emp.EmployeeID).FirstOrDefault();
                        if(sk == null)
                        {
                            sk = new SkillLevel();
                            sk.Level = 0;
                            sk.SkillID = skill.SkillID;
                            sk.EmployeeID = emp.EmployeeID;
                        
                            db.SkillLevels.Add(sk);
                        
                        }
                }
                db.SaveChanges();

            }
            catch(Exception ex)
            {
                //error handling
            }
        }

        public static bool CheckDepartmentAuthentication(HttpSessionStateBase Session, IPrincipal User, Department department)
        {
            //we check if the user is either an administrator or a manager for the department
            bool isValid = false;
            if (User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ID] != null)
            {
                int empID = Convert.ToInt32(Session[Const.CLAIM.USER_ID]);
                Employee emp = db.Employees.Where(x => x.EmployeeID == empID).FirstOrDefault();
                if (emp != null &&
                    (emp.Administrator == Const.PermissionLevels.Administrator ||
                    (emp.DepartmentID == department.DepartmentID && emp.Administrator == Const.PermissionLevels.Manager)))
                {
                     isValid = true;

                }
            }

            return isValid;
        }
        public static int CheckAuthentication(HttpSessionStateBase Session, IPrincipal User)
        {
            //we check if the user is either an administrator or a manager for the department
            int empID = 0;
            if (User.Identity.IsAuthenticated)
            {
                //getting  the id of the current authenticated user
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
                IEnumerable<Claim> claims = claimsIdentity.Claims;

                string tokenID = claims.Where(x => x.Type == Const.CLAIM.CLAIM_NAMESPACE
                + "/" + Const.Fields.EMPLOYEE_ID).FirstOrDefault().Value;
                empID = Convert.ToInt32(Session[Const.CLAIM.USER_ID]);
            }

            return empID;
        }

        public static bool CheckEventAuthentication(HttpSessionStateBase Session, IPrincipal User, Event @event) {
            //we check if the user is either an administrator orthe event creator
            bool isValid = false;

            if (User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
              && (
              (Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Administrator)
              || (Session[Const.CLAIM.USER_ID] != null && Session[Const.CLAIM.USER_ID].ToString() == @event.CreatorID.ToString()))
            )
            {
                isValid = true;
            }

            return isValid;
        }

        public static string CheckProjectAuthentication(HttpSessionStateBase Session, IPrincipal User, Project project)
        {
            //we check if the user is either an administrator or a manager for the department
            string accessLevel = Const.PermissionLevels.Employee;
            if (User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ID] != null)
            {
                int empID = Convert.ToInt32(Session[Const.CLAIM.USER_ID]);
                Employee emp = db.Employees.Where(x => x.EmployeeID == empID).FirstOrDefault();

                if (emp != null &&
                    (emp.Administrator == Const.PermissionLevels.Administrator))
                {
                    accessLevel = Const.PermissionLevels.Administrator;
                }
                if (emp != null && (emp.DepartmentID == project.DepartmentID && emp.Administrator == Const.PermissionLevels.Manager))
                {
                    accessLevel = Const.PermissionLevels.Manager;

                }
            }

            return accessLevel;
        }
        public static string CheckTaskAuthentication(IPrincipal User, int? sprintID)
        {
            //we check if the user is either an administrator or a manager for the department
            string accessLevel = Const.PermissionLevels.Employee;
            try
            {

            
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
                IEnumerable<Claim> claims = claimsIdentity.Claims;

               string userID = claims.Where(x => x.Type == Const.CLAIM.CLAIM_NAMESPACE
                + "/" + Const.Fields.EMPLOYEE_ID).FirstOrDefault().Value;

                if (User.Identity.IsAuthenticated && userID != null && sprintID.HasValue)
                {
                    int empID = Convert.ToInt32(userID);
                    Employee emp = db.Employees.Where(x => x.EmployeeID == empID).FirstOrDefault();
                    if (emp != null &&
                       (emp.Administrator == Const.PermissionLevels.Administrator))
                    {
                        accessLevel = Const.PermissionLevels.Administrator;
                    }
                    else
                    {
                        Sprint sprint = db.Sprints.Include(x => x.Project).Where(x => x.SprintID == sprintID.Value).FirstOrDefault();
                        if (emp != null && sprint != null && (emp.DepartmentID == sprint.Project.DepartmentID && emp.Administrator == Const.PermissionLevels.Manager))
                        {
                            accessLevel = Const.PermissionLevels.Manager;
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                //handle exception
            }
            return accessLevel;
        }

        public static string CheckSprintAuthentication(IPrincipal User)
        {
            //we check if the user is either an administrator or a manager for the department
            string accessLevel = Const.PermissionLevels.Employee;
            try
            {


                ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
                IEnumerable<Claim> claims = claimsIdentity.Claims;

                string userID = claims.Where(x => x.Type == Const.CLAIM.CLAIM_NAMESPACE
                 + "/" + Const.Fields.EMPLOYEE_ID).FirstOrDefault().Value;

                if (User.Identity.IsAuthenticated && userID != null )
                {
                    int empID = Convert.ToInt32(userID);
                    Employee emp = db.Employees.Where(x => x.EmployeeID == empID).FirstOrDefault();
                    if (emp != null &&
                       (emp.Administrator == Const.PermissionLevels.Administrator))
                    {
                        accessLevel = Const.PermissionLevels.Administrator;
                    }
                    else if(emp.Administrator == Const.PermissionLevels.Manager)
                    {
                            accessLevel = Const.PermissionLevels.Manager;
                    }

                }
            }
            catch (Exception ex)
            {
                //handle exception
            }
            return accessLevel;
        }

        public static string CheckSprintAuthentication(HttpSessionStateBase Session, IPrincipal User)
        {
            //we check if the user is either an administrator or a manager for the department
            string accessLevel = Const.PermissionLevels.Employee;
            try
            {

                if (User.Identity.IsAuthenticated)
                {
                  
                    if (Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Administrator)
                    {
                        accessLevel = Const.PermissionLevels.Administrator;
                    }
                    else if (Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Manager)
                    {
                        accessLevel = Const.PermissionLevels.Manager;
                    }

                }
            }
            catch (Exception ex)
            {
                //handle exception
            }
            return accessLevel;
        }
    }
}