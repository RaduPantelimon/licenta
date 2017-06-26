using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ResourceApplicationTool.Models;
using System.Globalization;

using ResourceApplicationTool.Utils;

namespace ResourceApplicationTool.Controllers
{
    public class ProjectsController : Controller
    {
        private RATV3Entities db = new RATV3Entities();

        // GET: Projects
        public ActionResult Index()
        {
            var projects = db.Projects.Include(p => p.Contact1).Include(p => p.Department).Include(p => p.Sprints);

            //calculating the total hours effort, per project
            List<Sprint> sprints = db.Sprints.ToList();
            List<Task> tasks = db.Tasks.ToList();

            foreach(Project pj in projects)
            {
                List<int> currentSprintsIDs = sprints.Where(x => x.ProjectID == pj.ProjectID).Select(x => x.SprintID).ToList();
                List<Task> currentTasks = tasks.Where(x => x.SprintID.HasValue && currentSprintsIDs.Contains(x.SprintID.Value)).ToList();
                int sum = 0;
                foreach (Task t in currentTasks)
                {
                    if(t.Estimation.HasValue) sum += t.Estimation.Value;
                }
                pj.ManHoursEffort = sum;
            }

            List<Department> departmentsForProjects = db.Departments.ToList();
            ViewBag.departments = departmentsForProjects;
            ViewBag.userAccess = Session[Const.CLAIM.USER_ACCESS_LEVEL];
            return View(projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            //making sure that the masterpage will render the <base href> elem for our angular module
            ViewBag.showBaseHref = true;
            ViewBag.hrefVal = "/Projects/Details/" + id;

            //adding months to viewbag
            ViewBag.months = Months();

            //determining the base url
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
            Request.ApplicationPath.TrimEnd('/') + "/";
            ViewBag.baseUrl = baseUrl;

            string accessLevel = Common.CheckProjectAuthentication(Session, User, project);
            ViewBag.userAccess = accessLevel;

            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            if(!User.Identity.IsAuthenticated || 
                Session[Const.CLAIM.USER_ACCESS_LEVEL] == null ||
                (Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Employee))
            {
                //a normal employee shouldn't be able to create a new project
                return RedirectToAction("NotFound", "Home");
            }

            if (User.Identity.IsAuthenticated)
            {
                //
            }

            ViewBag.ContactID = new SelectList(db.Contacts, "ContactID", "ContactName");

            //a manager will only be able to add a project for his department


            if ((Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Manager))
            {
                int empID = Convert.ToInt32(Session[Const.CLAIM.USER_ID]);
                Employee emp = db.Employees.Where(x => x.EmployeeID == empID).FirstOrDefault();
                if(emp != null)
                {
                    ViewBag.DepartmentID = new SelectList(db.Departments.Where(X => X.DepartmentID == emp.DepartmentID), "DepartmentID", "Title");
                }
                
            }
            else
            {
                ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "Title");
            }

           

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectID,Title,StartDate,EndDate,Duration,ContractNumber,PJDescription,Budget,DepartmentID,ContactID")] Project project)
        {
            if (ModelState.IsValid)
            {
                string accessLevel = Common.CheckProjectAuthentication(Session, User, project);
                if(accessLevel != Const.PermissionLevels.Administrator && 
                    accessLevel != Const.PermissionLevels.Manager)
                {
                    //a manager should only create a project only for his department
                    return RedirectToAction("NotFound", "Home");
                }
                try
                {
                    db.Projects.Add(project);
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    //handle exception
                }
                return RedirectToAction("Index");
            }

            ViewBag.ContactID = new SelectList(db.Contacts, "ContactID", "ContactName", project.ContactID1);
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "Title", project.DepartmentID);
            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            string accessLevel = Common.CheckProjectAuthentication(Session, User, project);
            if (accessLevel != Const.PermissionLevels.Administrator &&
                accessLevel != Const.PermissionLevels.Manager)
            {
                //a manager should only edit a project only for his department
                return RedirectToAction("NotFound", "Home");
            }

            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.ContactID = new SelectList(db.Contacts, "ContactID", "ContactName", project.ContactID1);
            if(accessLevel == Const.PermissionLevels.Manager)
            {
                int empID = Convert.ToInt32(Session[Const.CLAIM.USER_ID]);
                Employee emp = db.Employees.Where(x => x.EmployeeID == empID).FirstOrDefault();
                if (emp != null)
                {
                    ViewBag.DepartmentID = new SelectList(db.Departments.Where(X => X.DepartmentID == emp.DepartmentID), "DepartmentID", "Title");
                }
            }
            else
            {
                ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "Title", project.DepartmentID);
            }
            ViewBag.userAccess = accessLevel;
            return View(project);
        }

        // POST: Projects/Edit/5
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit([Bind(Include = "ProjectID,Title,StartDate,EndDate,Duration,ContractNumber,PJDescription,Budget,DepartmentID,ContactID")] Project project)
        {
            if (ModelState.IsValid)
            {
                string accessLevel = Common.CheckProjectAuthentication(Session, User, project);
                if (accessLevel != Const.PermissionLevels.Administrator &&
                    accessLevel != Const.PermissionLevels.Manager)
                {
                    //a manager should only edit a project only for his department
                    return RedirectToAction("NotFound", "Home");
                }
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ContactID = new SelectList(db.Contacts, "ContactID", "ContactName", project.ContactID1);
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "Title", project.DepartmentID);
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            string accessLevel = Common.CheckProjectAuthentication(Session, User, project);
            if (accessLevel != Const.PermissionLevels.Administrator &&
                accessLevel != Const.PermissionLevels.Manager)
            {
                //a manager should only edit a project only for his department
                return RedirectToAction("NotFound", "Home");
            }
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        public ActionResult GenerateMonthlyReport(int projectID,int month, int year)
        {
            try
            {
                HttpContext.Response.Clear();
                HttpContext.Response.ContentType = "application/ms-excel";
                HttpContext.Response.AddHeader("Content-Disposition",
                "attachment; filename=" + "ProjectName.xlsx" + ";");
                byte[] array = Utils.ExcelReportGenerator.GenerateExcelReport(projectID,month,year,db);

                HttpContext.Response.OutputStream.Write(array, 0, array.Length);
                HttpContext.Response.End();
            }
            catch (Exception ex)
            {
                //error handling
            }

            return View();
        }


        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            string accessLevel = Common.CheckProjectAuthentication(Session, User, project);
            if (accessLevel != Const.PermissionLevels.Administrator &&
                accessLevel != Const.PermissionLevels.Manager)
            {
                //a manager should only edit a project only for his department
                return RedirectToAction("NotFound", "Home");
            }
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IEnumerable<SelectListItem> Months()
        {
                return DateTimeFormatInfo
                       .InvariantInfo
                       .MonthNames
                       .Select((monthName, index) => new SelectListItem
                       {
                           Value = (index + 1).ToString(),
                           Text = monthName
                       });

        }

        //save the tasks added by the user
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public ActionResult SaveTask(TaskTemplate data)
        { 
            try
            {
                Task templateTask;
                if(data.directDescendant > 0)
                {
                    templateTask = db.Tasks.Where(x => x.TaskID == data.directDescendant).FirstOrDefault();
                }
               else
                {
                    templateTask = db.Tasks.Where(x => x.TaskID == data.templateTaskID).FirstOrDefault();
                }
                if (templateTask != null)
                {
                    DateTime startDate = DateTime.Parse(data.startDate);
                    Task task = new Task();

                    task.TaskDescription = templateTask.TaskDescription;
                    task.SprintID = data.sprintID;
                    task.EmployeeID = data.employeeID;
                    task.StartDate = startDate;
                    task.EndDate = startDate;
                    task.Difficulty = templateTask.Difficulty;

                    if(templateTask.Estimation.HasValue)
                    {
                        task.Estimation = templateTask.Estimation;
                    }
                    if (!templateTask.TemplateID.HasValue || templateTask.TemplateID.Value <=0)
                    {
                        task.TemplateID = data.templateTaskID;
                    }
                    else
                    {
                        task.TemplateID = templateTask.TemplateID;
                    }
                   
                    


                    string accessLevel = Common.CheckSprintAuthentication(Session,User);
                    if (accessLevel != Const.PermissionLevels.Administrator && accessLevel != Const.PermissionLevels.Manager)
                    {
                       
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Could not save the new task. Insufficient permissions");
                    }

                    db.Tasks.Add(task);
                    db.SaveChanges();
                    //jsonResponseText = JsonConvert.SerializeObject(task);
                    return Json(task);
                }
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Could not save the new task.");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Could not save the new task");
            }

        }


        //save the tasks added by the user
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public ActionResult SaveTasks(List<TaskTemplate> templates)
        {
            string accessLevel = Common.CheckSprintAuthentication(Session, User);
            if (accessLevel != Const.PermissionLevels.Administrator && accessLevel != Const.PermissionLevels.Manager)
            {

                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Could not save the new task. Insufficient permissions");
            }

            List<Task> savedResults = new List<Task>();
            try
            {
                foreach (TaskTemplate data in templates)
                {
                    Task templateTask;
                    if (data.directDescendant > 0)
                    {
                        templateTask = db.Tasks.Where(x => x.TaskID == data.directDescendant).FirstOrDefault();
                    }
                    else
                    {
                        templateTask = db.Tasks.Where(x => x.TaskID == data.templateTaskID).FirstOrDefault();
                    }
                    if (templateTask != null)
                    {
                        Task newSaveTask = createTask(templateTask, data);
                        if(newSaveTask != null)
                        {
                            savedResults.Add(newSaveTask);
                        }
                    }  
                }

                if (savedResults.Count > 0)
                {
                    return Json(savedResults);
                }
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Could not save the new tasks.");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Could not save the new task");
            }

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private Task createTask(Task templateTask, TaskTemplate data)
        {
            try
            {
                DateTime startDate = DateTime.Parse(data.startDate);
                Task task = new Task();

                task.TaskDescription = templateTask.TaskDescription;
                task.SprintID = data.sprintID;
                task.EmployeeID = data.employeeID;
                task.StartDate = startDate;
                task.EndDate = startDate;
                task.Difficulty = templateTask.Difficulty;

                if (templateTask.Estimation.HasValue)
                {
                    task.Estimation = templateTask.Estimation;
                }
                else
                {
                    task.Estimation = 0;
                }
                if (!templateTask.TemplateID.HasValue || templateTask.TemplateID.Value <= 0)
                {
                    task.TemplateID = data.templateTaskID;
                }
                else
                {
                    task.TemplateID = templateTask.TemplateID;
                }


                db.Tasks.Add(task);
                db.SaveChanges();
                return task;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
