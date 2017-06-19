using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ResourceApplicationTool.Models;
using ResourceApplicationTool.Models.SecondaryModels;
using ResourceApplicationTool.Utils;

namespace ResourceApplicationTool.Controllers
{
    public class EmployeesController : Controller
    {
        private RATV3Entities db = new RATV3Entities();

        // GET: Employees
        public ActionResult Index()
        {
            var employees = db.Employees.Include(e => e.Department).Include(e => e.Employee1).Include(e => e.Role).OrderBy(e => e.DepartmentID);
            List<Department> departmentsForEmployees = db.Departments.ToList();

            ViewBag.departments = departmentsForEmployees;
            return View(employees.ToList());
        }


        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);

            if (employee.Role != null && employee.Role.Name != null)
            {
                ViewBag.RoleName = employee.Role.Name;
            }
            else
            {
                ViewBag.RoleName = "";
            }

            Common.CreateSkillTemplates(employee);

            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
            Request.ApplicationPath.TrimEnd('/') + "/";

            //getting the picture ready
            if (employee.File != null)
            {

                ViewBag.ImgID = Const.PicturePaths.ImgControllerRoot + employee.File.FileNumber;
                ViewBag.ImgIDSec = baseUrl + Const.PicturePaths.ImgControllerRoot + employee.File.FileNumber;
            }
            else
            {
                ViewBag.ImgID = Const.PicturePaths.ProfilePictureUrl;
                ViewBag.ImgIDSec = baseUrl + Const.PicturePaths.ProfilePictureUrl;
            }
            if (employee == null)
            {
                return HttpNotFound();
            }

            employee.SkillLevelsList = employee.SkillLevels.ToList();
            ViewBag.SkillCategories = db.SkillCategories.OrderByDescending(x => x.Skills.Count).ToList();
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            //checking if we have the permission necessary to add a new user
            if(!(User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
            && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
            && Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Administrator))
            {
                return RedirectToAction("NotFound", "Home");
            }

            //initially the user will have the default profile picture
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
            Request.ApplicationPath.TrimEnd('/') + "/";
            ViewBag.ImgID = Const.PicturePaths.ProfilePictureUrl;
            ViewBag.ImgIDSec = baseUrl + Const.PicturePaths.ProfilePictureUrl;

            //no roleName yet 
            ViewBag.RoleName = "";

            //preparing the departments
            List<SerializableDepartment> departments = db.Departments.ToList().Select(x => new SerializableDepartment(x.Title, x.DepartmentID)).ToList();
            foreach (SerializableDepartment dept in departments)
            {
                dept.Manangers = db.Employees.Where(x => x.DepartmentID == dept.DepartmentID && x.Administrator == Const.PermissionLevels.Manager).ToList().
                    Select(x => new SerializableEmployee(x.FirstName, x.LastName, x.Account, x.EmployeeID)).ToList();

            }
            ViewBag.Departments = departments;

            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "Title");
            ViewBag.ManagerID = new SelectList(db.Employees, "EmployeeID", "Account");
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Name");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,RoleID,Account,Password,ConfirmPassword,ManagerID,DepartmentID,FirstName,MiddleInitial,LastName,Title,CNP,Email,PhoneNumber,Salary,PriorSalary,LastRaise,HireDate,TerminationDate,Administrator")] Employee employee,
             HttpPostedFileBase uploadProfilePicture)
        {
            //checking if we have the permission necessary to add a new user
            if (!(User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
            && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
            && Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Administrator))
            {
                return RedirectToAction("NotFound", "Home");
            }


            if (ModelState.IsValid)
            {
                //saving the profile picture
                if (uploadProfilePicture != null && uploadProfilePicture.ContentLength > 0)
                {
                    Guid? avatarGuid = Common.CreateImage(uploadProfilePicture);
                    //if file was stored successfully     
                    employee.ProfileImageID = avatarGuid;
                }
                else
                {
                    employee.ProfileImageID = null;
                }

                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }



            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "Title", employee.DepartmentID);
            ViewBag.ManagerID = new SelectList(db.Employees, "EmployeeID", "Account", employee.ManagerID);
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Name", employee.RoleID);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
           

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Include(e => e.Role).Include(e => e.Educations).SingleOrDefault(x => x.EmployeeID == id);

            //checking if we have the permission necessary to add a new user
            if (!(User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
                  && (
                  (Session[Const.CLAIM.USER_ACCESS_LEVEL] != null && Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Administrator)
                  || (Session[Const.CLAIM.USER_ID] != null && (Session[Const.CLAIM.USER_ID].ToString() == employee.EmployeeID.ToString() ||
                     Session[Const.CLAIM.USER_ID].ToString() == employee.ManagerID.ToString())))
            ))
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (employee == null)
            {
                return HttpNotFound();
            }
            if(employee.Role != null && employee.Role.Name != null)
            {
                ViewBag.RoleName = employee.Role.Name;
            }
            else
            {
                ViewBag.RoleName = "";
            }

            Common.CreateSkillTemplates(employee);

            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
            Request.ApplicationPath.TrimEnd('/') + "/";
            
            //getting the picture ready
            if (employee.File != null)
            {

                ViewBag.ImgID = Const.PicturePaths.ImgControllerRoot + employee.File.FileNumber;
                ViewBag.ImgIDSec = baseUrl + Const.PicturePaths.ImgControllerRoot + employee.File.FileNumber;
            }
            else
            {
                ViewBag.ImgID = Const.PicturePaths.ProfilePictureUrl;
                ViewBag.ImgIDSec = baseUrl + Const.PicturePaths.ProfilePictureUrl;
            }

            //preparing the departments and managers lists
            List<SerializableDepartment> departments = db.Departments.ToList().Select(x => new SerializableDepartment(x.Title, x.DepartmentID)).ToList();
            foreach (SerializableDepartment dept in departments)
            {
                dept.Manangers = db.Employees.Where(x => x.DepartmentID == dept.DepartmentID && x.Administrator == Const.PermissionLevels.Manager).ToList().
                    Select(x => new SerializableEmployee(x.FirstName, x.LastName, x.Account, x.EmployeeID)).ToList();

            }

            employee.SkillLevelsList = employee.SkillLevels.ToList();
            ViewBag.Departments = departments;
            ViewBag.SkillCategories = db.SkillCategories.OrderByDescending(x => x.Skills.Count).ToList();
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "Title", employee.DepartmentID);
            ViewBag.ManagerID = new SelectList(db.Employees, "EmployeeID", "Account", employee.ManagerID);
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Name", employee.RoleID);
            
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "EmployeeID,RoleID,Account,Password,ManagerID,DepartmentID,FirstName,MiddleInitial,LastName,Title,CNP,Email,PhoneNumber,Salary,PriorSalary,LastRaise,HireDate,TerminationDate,Administrator,SkillLevelsList")] Employee employee,
            //Employee employee,
            //SkillLevel[] SkillLevelsList,
            //SkillLevelCollectionPostModel sklvlpostModel,
            HttpPostedFileBase uploadProfilePicture)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            if (ModelState.IsValid)
            {

                //checking if we have the permission necessary to add a new user
                if (!(User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
                      && (
                      (Session[Const.CLAIM.USER_ACCESS_LEVEL] != null && Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Administrator)
                      || (Session[Const.CLAIM.USER_ID] != null && (Session[Const.CLAIM.USER_ID].ToString() == employee.EmployeeID.ToString() ||
                         Session[Const.CLAIM.USER_ID].ToString() == employee.ManagerID.ToString())))
                ))
                {
                    return RedirectToAction("NotFound", "Home");
                }


                employee.Password = "testerino";
                employee.ConfirmPassword = employee.Password;
                //we will disable the properties that different users are not allowed to edit
                if (Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() != Const.PermissionLevels.Administrator)
                {
                    employee.CNP = "1234";
                }
                if (Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() != Const.PermissionLevels.Manager &&
                    Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() != Const.PermissionLevels.Administrator)
                {
                   
                    employee.Email = "testerino@tst.com";
                    employee.CNP = "1234";
                    employee.PhoneNumber = "222";
                }
                    db.Entry(employee).State = EntityState.Modified;
                if (uploadProfilePicture != null && uploadProfilePicture.ContentLength > 0)
                {
                    Guid? avatarGuid = Common.CreateImage(uploadProfilePicture);
                    //if file was stored successfully     
                    employee.ProfileImageID = avatarGuid;
                }
                else
                {
                    db.Entry(employee).Property(X => X.ProfileImageID).IsModified = false;
                }
               
                //saving the changes done to the skills
                foreach(SkillLevel skLvl in employee.SkillLevelsList)
                {
                    db.Entry(skLvl).State = EntityState.Modified;
                }

                //we will disable the properties that different users are not allowed to edit
                if (Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() != Const.PermissionLevels.Administrator)
                {
                    db.Entry(employee).Property(X => X.CNP).IsModified = false;
                    db.Entry(employee).Property(X => X.Administrator).IsModified = false;
                }
                if (Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() != Const.PermissionLevels.Manager && 
                    Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() != Const.PermissionLevels.Administrator)
                {
                    //various info
                    db.Entry(employee).Property(X => X.RoleID).IsModified = false;
                    db.Entry(employee).Property(X => X.ManagerID).IsModified = false;
                    db.Entry(employee).Property(X => X.DepartmentID).IsModified = false;
                    db.Entry(employee).Property(X => X.Salary).IsModified = false;
                    //contact
                    db.Entry(employee).Property(X => X.Email).IsModified = false;
                    db.Entry(employee).Property(X => X.PhoneNumber).IsModified = false;
                    db.Entry(employee).Property(X => X.HireDate).IsModified = false;
                    db.Entry(employee).Property(X => X.TerminationDate).IsModified = false;
                }

                employee.Title = employee.FirstName + " " + employee.LastName;

                db.Entry(employee).Property(X => X.Password).IsModified = false;
                try
                {
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    //handle errors
                    //((System.Data.Entity.Validation.DbEntityValidationException)ex).EntityValidationErrors
                }
                
              
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "Title", employee.DepartmentID);
            ViewBag.ManagerID = new SelectList(db.Employees, "EmployeeID", "Account", employee.ManagerID);
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Name", employee.RoleID);

            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);

            if (employee == null)
            {
                return HttpNotFound();
            }


            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
            Request.ApplicationPath.TrimEnd('/') + "/";

            //checking if we have the permission necessary to add a new user
            if (!(User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
                  && 
                  (Session[Const.CLAIM.USER_ACCESS_LEVEL] != null && Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Administrator)
            ))
            {
                return RedirectToAction("NotFound", "Home");
            }
            //getting the picture ready
            if (employee.File != null)
            {

                ViewBag.ImgID = Const.PicturePaths.ImgControllerRoot + employee.File.FileNumber;
                ViewBag.ImgIDSec = baseUrl + Const.PicturePaths.ImgControllerRoot + employee.File.FileNumber;
            }
            else
            {
                ViewBag.ImgID = Const.PicturePaths.ProfilePictureUrl;
                ViewBag.ImgIDSec = baseUrl + Const.PicturePaths.ProfilePictureUrl;
            }

            //checking if we have the permission necessary to add a new user
            if (!(User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
            && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
            && Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Administrator))
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (employee.Role != null && employee.Role.Name != null)
            {
                ViewBag.RoleName = employee.Role.Name;
            }
            else
            {
                ViewBag.RoleName = "";
            }
            return View(employee);
        }


        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);

            //checking if we have the permission necessary to add a new user
            if (employee == null || !(User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
            && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
            && Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Administrator))
            {
                return RedirectToAction("NotFound", "Home");
            }

            db.Employees.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #region PDFGenerators
        // GET: Employees/GenerateCV/5
        public ActionResult GenerateCV(int? id)
            {

                Employee employee = db.Employees.Find(id);
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

                //initialize baseUrl
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/";

                ViewBag.baseUrl = baseUrl;

                //initializing CV
                employee.SkillLevelsList = employee.SkillLevels.ToList();
                ViewBag.SkillCategories = db.SkillCategories.OrderByDescending(x => x.Skills.Count).ToList();


                //generating the main body
                string generatedPDFHtml = ViewRenderer.RenderView("~/Views/Employees/PdfCVGenerator.cshtml", employee,
                                                     ControllerContext);
                //generating the header
                string headertext = ViewRenderer.RenderView("~/Views/Employees/PdfCVHeader.cshtml", employee,
                                                     ControllerContext);
                byte[] pdfBuffer = PdfGenerator.ConvertHtmlToPDF(generatedPDFHtml, headertext);

                //delete the temporary generated file
                if(!String.IsNullOrEmpty(profilePicUrl))
                {
                    Utils.Common.DeleteLocalImage(profilePicUrl);
                }


                //sending the pdf file to download
                this.HttpContext.Response.ContentType = "application/pdf";
                this.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + "EmployeeCV.pdf");
                this.HttpContext.Response.BinaryWrite(pdfBuffer);
                this.HttpContext.Response.Flush();
                this.HttpContext.Response.Close();

                return View(employee);
            }

            // GET: Employees/GenerateCV/5
            public ActionResult GetCVHtml(int? id)
            {

                Employee employee = db.Employees.Find(id);
                string profilePicUrl = "";

                //getting the picture ready
                Common.CreateSkillTemplates(employee);
                if (employee.File != null)
                {
                    profilePicUrl = Utils.Common.SaveImgLocally(db, employee.File.FileID.ToString());
                    ViewBag.profilePicUrl = profilePicUrl;

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

                //initialize baseUrl
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/";

                ViewBag.baseUrl = baseUrl;

                ViewBag.justHtml = true;

                //delete the temporary generated file
                if (!String.IsNullOrEmpty(profilePicUrl))
                {
                    Utils.Common.DeleteLocalImage(profilePicUrl);
                }
            return View("PdfCVGenerator", employee);
            }

            // GET: Employees/GenerateCV/5
            public ActionResult GetCVHeader(int? id)
            {

                Employee employee = db.Employees.Find(id);
                string profilePicUrl = "";

                //getting the picture ready
                Common.CreateSkillTemplates(employee);
                if (employee.File != null)
                {
                    profilePicUrl = Utils.Common.SaveImgLocally(db, employee.File.FileID.ToString());
                    ViewBag.profilePicUrl = profilePicUrl;

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

                //initialize baseUrl
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/";

                ViewBag.baseUrl = baseUrl;

                ViewBag.justHtml = true;

                //delete the temporary generated file
                if (!String.IsNullOrEmpty(profilePicUrl))
                {
                    Utils.Common.DeleteLocalImage(profilePicUrl);
                }
                return View("PdfCVHeader", employee);
            }
        #endregion

     

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
