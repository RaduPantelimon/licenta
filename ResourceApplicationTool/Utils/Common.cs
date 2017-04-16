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