using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ResourceApplicationTool.Models;
using ResourceApplicationTool.Utils;
using Newtonsoft.Json;
using System.Text;

namespace ResourceApplicationTool.Controllers
{
    public class EducationsController : Controller
    {
        private RATV3Entities db = new RATV3Entities();

        // GET: Educations
        public ActionResult Index()
        {
            var educations = db.Educations.Include(e => e.Employee);
            return View(educations.ToList());
        }

        // GET: Educations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Education education = db.Educations.Find(id);
            if (education == null)
            {
                return HttpNotFound();
            }
            return View(education);
        }

        // GET: Educations/Create
        public ActionResult Create()
        {
            //trying to find out if our page is a modal window or not
            bool isModal = false;
            string isModalString = Request.QueryString[Const.QueryStringParams.isModal]!=null? Request.QueryString[Const.QueryStringParams.isModal].ToString():"";
            string employeeID = Request.QueryString[Const.QueryStringParams.employeeID] != null ? Request.QueryString[Const.QueryStringParams.employeeID].ToString() : "";
            //string 
            if (!String.IsNullOrEmpty(isModalString) && isModalString =="1")
            {
                isModal = true;
            }
            
            ViewBag.IsModal = isModal;
            int emppIntID;
            if(!String.IsNullOrEmpty(employeeID) && int.TryParse(employeeID, out emppIntID))
            {
                ViewBag.empID = employeeID;
                ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "Account", emppIntID);
            }
            else
            {
                ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "Account");
                ViewBag.empID = "";
            }
            
            return PartialView();
        }

        // POST: Educations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EducationID,Title,StartDate,EndDate,Degree,EmployeeID,Duration")] Education education)
        {
            if (ModelState.IsValid)
            {
                db.Educations.Add(education);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "Account", education.EmployeeID);
            return View(education);
            //return "Freak out";
        }

       

        // GET: Educations/Edit/5
        public ActionResult Edit(int? id)
        {
            //trying to find out if our page is a modal window or not
            bool isModal = false;
            string isModalString = Request.QueryString[Const.QueryStringParams.isModal] != null ? Request.QueryString[Const.QueryStringParams.isModal].ToString() : "";
            //string 
            if (!String.IsNullOrEmpty(isModalString) && isModalString == "1")
            {
                isModal = true;
            }
            ViewBag.IsModal = isModal;


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Education education = db.Educations.Find(id);
            if (education == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "Account", education.EmployeeID);
            return View(education);
        }

        // POST: Educations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EducationID,Title,StartDate,EndDate,Degree")] Education education)
        {
            if (ModelState.IsValid)
            {
                db.Entry(education).State = EntityState.Modified;
                db.Entry(education).Property(x => x.EmployeeID).IsModified = false;
                db.Entry(education).Property(x => x.Duration).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "Account", education.EmployeeID);
            return View(education);
        }

        // GET: Educations/Delete/5
        public ActionResult Delete(int? id)
        {
            //trying to find out if our page is a modal window or not
            bool isModal = false;
            string isModalString = Request.QueryString[Const.QueryStringParams.isModal] != null ? Request.QueryString[Const.QueryStringParams.isModal].ToString() : "";
            //string 
            if (!String.IsNullOrEmpty(isModalString) && isModalString == "1")
            {
                isModal = true;
            }
            ViewBag.IsModal = isModal;


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Education education = db.Educations.Find(id);
            if (education == null)
            {
                return HttpNotFound();
            }
            return View(education);
        }

        // POST: Educations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Education education = db.Educations.Find(id);
            db.Educations.Remove(education);
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

        #region RestApiActions 
        //create a new education
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEducation([Bind(Include = "EducationID,Title,StartDate,EndDate,Degree,EmployeeID")] Education education)
        {
            string jsonResponseText = "";
            try
            {
                if (ModelState.IsValid)
                {
                    db.Educations.Add(education);
                    db.SaveChanges();
                    jsonResponseText = JsonConvert.SerializeObject(education);
                    return Json(education);
                }

                //var response = Request.CreateResponse(HttpStatusCode.OK);
                //response.Content = new StringContent(jsonResponseText, Encoding.UTF8, "application/json");
                //return response;
                return Json(new { status = "0", statusMessage = "Model stat was not valid" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "0", statusMessage = ex.InnerException.Message });
            }

        }
        // POST: Educations/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEducation(int id)
        {
            try
            {
                Education education = db.Educations.Find(id);
                db.Educations.Remove(education);
                db.SaveChanges();
                return Json(new { status = "1", statusMessage = "The item was successfully deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { status = "0", statusMessage = ex.InnerException.Message });
            }
        }
        // POST: Educations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEducation([Bind(Include = "EducationID,Title,StartDate,EndDate,Degree,EmployeeID")] Education education)
        {
            string jsonResponseText = "";
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(education).State = EntityState.Modified;
                    db.Entry(education).Property(x => x.EmployeeID).IsModified = false;
                    db.Entry(education).Property(x => x.Duration).IsModified = false;
                    db.SaveChanges();
                    jsonResponseText = JsonConvert.SerializeObject(education);
                    return Json(education);
                }
                
                return Json(new { status = "0", statusMessage = "Model stat was not valid" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "0", statusMessage = ex.InnerException.Message });
            }
        }
        //expecting the employee id
        public HttpResponseMessage GetEducationsForEmployee(int id, HttpRequestMessage request)
        {
            string jsonResponseText = "";
            Employee employee = db.Employees.Include(x => x.Educations).Where(x => x.EmployeeID == id).FirstOrDefault();
            if (employee != null)
            {
                List<Education> educations = employee.Educations.ToList();
                if (educations != null)
                {
                    jsonResponseText = JsonConvert.SerializeObject(educations);
                }
            }
            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResponseText, Encoding.UTF8, "application/json");
            return response;
        }

        #endregion
    }
}
