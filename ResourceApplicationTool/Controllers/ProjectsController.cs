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

            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            ViewBag.ContactID = new SelectList(db.Contacts, "ContactID", "ContactName");
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "Title");
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
                db.Projects.Add(project);
                db.SaveChanges();
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
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.ContactID = new SelectList(db.Contacts, "ContactID", "ContactName", project.ContactID1);
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "Title", project.DepartmentID);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProjectID,Title,StartDate,EndDate,Duration,ContractNumber,PJDescription,Budget,DepartmentID,ContactID")] Project project)
        {
            if (ModelState.IsValid)
            {
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

            }

            return View();
        }


        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
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
