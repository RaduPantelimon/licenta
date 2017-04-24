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
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,StartTime,EndTime,EventType,Location,Title")] Event @event)
        {
            if (ModelState.IsValid)
            {
                if (!(User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ACCESS_LEVEL] != null
                && (
                (Session[Const.CLAIM.USER_ACCESS_LEVEL].ToString() == Const.PermissionLevels.Administrator)
                || (Session[Const.CLAIM.USER_ID] != null && Session[Const.CLAIM.USER_ID].ToString() == @event.CreatorID.ToString()))
              ))
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
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
