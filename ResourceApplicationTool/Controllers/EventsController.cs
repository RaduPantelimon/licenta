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
using Newtonsoft.Json;


namespace ResourceApplicationTool.Controllers
{
    public class EventsController : Controller
    {
        private RATV3Entities db = new RATV3Entities();

        // GET: Events
        public ActionResult Index()
        {
            var events = db.Events.Include(x => x.Employee);
            return View(events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Details/5
        public ActionResult EmailTest(int? id)
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

                //initialize baseUrl
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/";

                ViewBag.baseUrl = baseUrl;

                //initializing CV
                employee.SkillLevelsList = employee.SkillLevels.ToList();
                ViewBag.SkillCategories = db.SkillCategories.OrderByDescending(x => x.Skills.Count).ToList();
                #endregion

            }



            //after the save actions are completed, we'll send the notifications to the attendants
            Mailer mailer = new Mailer();
            mailer.SendMail(db, @event, @event.Attendants.Select(x => x.Employee).ToList(), @event.Employee, ControllerContext);
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            int currentUserID;

            if (!User.Identity.IsAuthenticated ||
                String.IsNullOrEmpty(Session[Const.CLAIM.USER_ID].ToString()) ||
                !int.TryParse(Session[Const.CLAIM.USER_ID].ToString(), out currentUserID))
            {
                return RedirectToAction("NotFound", "Home");
            }

            ViewBag.CreatorID = new SelectList(db.Employees, "EmployeeID", "Account");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,StartTime,EndTime,EventType,Location,CreatorID,Title")] Event @event,
            string AttendantsIDs)
        {
            //checking if we have the permission necessary to add a new event

            int currentUserID;

            if (!User.Identity.IsAuthenticated || 
                String.IsNullOrEmpty(Session[Const.CLAIM.USER_ID].ToString()) ||
                !int.TryParse(Session[Const.CLAIM.USER_ID].ToString(), out currentUserID))
            {
                return RedirectToAction("NotFound", "Home");
            }
            if (ModelState.IsValid)
            {
                @event.CreatorID = currentUserID;

                //finding employees
                List<Employee> attendantEmployees = new List<Employee>();
                if (!String.IsNullOrEmpty(AttendantsIDs))
                {
                    string[] ids = AttendantsIDs.Split(';');
                    List<int> parsedIDs = new List<int>();
                    foreach (string id in ids)
                    {
                        int resultID;
                        if(int.TryParse(id, out resultID))
                        {
                            parsedIDs.Add(resultID);
                        }
                    }

                    //get attendants
                   attendantEmployees = db.Employees.Where(x => parsedIDs.Contains(x.EmployeeID)).ToList();

                }
                

                db.Events.Add(@event);
                db.SaveChanges();
                
                //if we found any attendants, add them to the database
                if(attendantEmployees != null && attendantEmployees.Count() > 0)
                {
                    try
                    {

                        foreach(Employee att in attendantEmployees)
                        {
                            Attendant newAtt = new Attendant();
                            newAtt.EmployeeID = att.EmployeeID;
                            newAtt.EventID = @event.EventID;

                            db.Attendants.Add(newAtt);
                        }
                        db.SaveChanges();

                    }
                    catch(Exception ex)
                    {
                        //handle exception
                    }
                }

                //after the save actions are completed, we'll send the notifications to the attendants
                Mailer mailer = new Mailer();
                mailer.SendMail(db, @event, attendantEmployees, @event.Employee, ControllerContext);


                return RedirectToAction("Index");
            }

            ViewBag.CreatorID = new SelectList(db.Employees, "EmployeeID", "Account", @event.CreatorID);
            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (!(User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
                 && (
                 (Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Administrator)
                 || (Session[Const.CLAIM.USER_ID] != null && Session[Const.CLAIM.USER_ID].ToString() == @event.CreatorID.ToString()))
           ))
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (@event == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatorID = new SelectList(db.Employees, "EmployeeID", "Account", @event.CreatorID);

            //getting the existing attendants
            List<Attendant> attendants = db.Attendants.Where(X => X.EventID == @event.EventID).ToList();
            List<ExistingAttendant> existing = attendants.Select(x => new ExistingAttendant(
                                "/Employees/Details/" + x.EmployeeID,
                                x.Employee.FirstName + " " + x.Employee.LastName, 
                                x.EmployeeID)).ToList();

            string attendantIDs = String.Join(";",attendants.Select(x => x.EmployeeID.ToString()));

            ViewBag.AttendantIDs = attendantIDs + ";";
            ViewBag.AttendantNames = JsonConvert.SerializeObject(existing);

            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "EventID,StartTime,EndTime,EventType,Location,Title")] Event @event, 
            string AttendantsIDs,
           string AttendantsNames)
        {
            if (ModelState.IsValid)
            {
                Event ev = db.Events.Where(x => x.EventID == @event.EventID).FirstOrDefault();
                int creatorID = ev.CreatorID;

                ev.StartTime = @event.StartTime;
                ev.EndTime = @event.EndTime;
                ev.EventType = @event.EventType;
                ev.Title = @event.Title;

                if (!(User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
                && (
                (Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Administrator)
                || (Session[Const.CLAIM.USER_ID] != null && Session[Const.CLAIM.USER_ID].ToString() == ev.CreatorID.ToString()))
              ))
                {
                    return RedirectToAction("NotFound", "Home");
                }
                int.TryParse(Session[Const.CLAIM.USER_ID].ToString(), out creatorID);

                ev.CreatorID = creatorID;
                db.Entry(ev).State = EntityState.Modified;
                db.SaveChanges();

                //updating the attendants
                List<Employee> attendantEmployees = new List<Employee>();
                List<int> parsedIDs = new List<int>();

                if (!String.IsNullOrEmpty(AttendantsIDs))
                {
                    string [] ids = AttendantsIDs.Split(';');
                    
                    foreach (string id in ids)
                    {
                        int resultID;
                        if (int.TryParse(id, out resultID))
                        {
                            parsedIDs.Add(resultID);
                        }
                    }

                    
                }
                List<Attendant> existingAttendants = db.Attendants.Where(x => x.EventID == @event.EventID).ToList();
                
                //delete removed attendants
                List<Attendant> removedAttendants = existingAttendants.Where(x => !parsedIDs.Contains(x.EmployeeID)).ToList();
                foreach(Attendant att in removedAttendants)
                {
                    db.Attendants.Remove(att);
                }

                //get new attendants
                parsedIDs = parsedIDs.Where(x => !existingAttendants.Any(y => y.EmployeeID == x)).ToList();
                attendantEmployees = db.Employees.Where(x => parsedIDs.Contains(x.EmployeeID)).ToList();

                //add new attendants
                foreach (Employee att in attendantEmployees)
                {
                 
                    Attendant newAtt = new Attendant();
                    newAtt.EmployeeID = att.EmployeeID;
                    newAtt.EventID = @event.EventID;

                    db.Attendants.Add(newAtt);
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.AttendantIDs = AttendantsIDs;
            ViewBag.AttendantNames = AttendantsNames;

            ViewBag.CreatorID = new SelectList(db.Employees, "EmployeeID", "Account", @event.CreatorID);
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);

            //allow users to delete an event only if they are an admin or if they created it
            if (!(User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
                && (
                (Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Administrator)
                || (Session[Const.CLAIM.USER_ID] != null && Session[Const.CLAIM.USER_ID].ToString() == @event.CreatorID.ToString()))
            ))
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            //allow users to delete an event only if they are an admin or if they created it
            if (!(User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
                && (
                (Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Administrator)
                || (Session[Const.CLAIM.USER_ID] != null && Session[Const.CLAIM.USER_ID].ToString() == @event.CreatorID.ToString()))
            ))
            {
                return RedirectToAction("NotFound", "Home");
            }

            db.Events.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("Index");
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
