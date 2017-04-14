using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ResourceApplicationTool.Models;
using ResourceApplicationTool.Utils;

namespace ResourceApplicationTool.Controllers
{
    public class DepartmentsController : Controller
    {
        private RATV3Entities db = new RATV3Entities();

        // GET: Departments
        public ActionResult Index()
        {
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
            Request.ApplicationPath.TrimEnd('/') + "/";
            var departments = db.Departments.Include(d => d.File).Include(d => d.File1);

            List<Project> projects = db.Projects.ToList();
            List<Employee> employees = db.Employees.ToList();

            foreach(Department dept in departments)
            {
                dept.projectsNumber = projects.Where(x => x.DepartmentID == dept.DepartmentID).Count();
                dept.employeesNumber = employees.Where(x => x.DepartmentID == dept.DepartmentID).Count();
            }

            ViewBag.baseUrl = baseUrl;
            return View(departments.ToList());
        }

        // GET: Departments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);

            if (department == null)
            {
                return HttpNotFound();
            }
            //department.File = db.Files.ToList().First();
          
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
            Request.ApplicationPath.TrimEnd('/') + "/";
            ViewBag.baseUrl = baseUrl;

            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {

            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
            Request.ApplicationPath.TrimEnd('/') + "/";

            ViewBag.ImgIDSec = baseUrl + Const.PicturePaths.DepartmentPictureUrl;
            ViewBag.BannerImgIDSec = baseUrl + Const.PicturePaths.DepartmentBannerPictureUrl;
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "DepartmentID,Title,MaxSize,DeptDescription,StartDate,MonthlyExpenses")] Department department, 
            HttpPostedFileBase uploadPicture,
            HttpPostedFileBase uploadBanner)
        {
            if (ModelState.IsValid)
            {
                if (uploadPicture != null && uploadPicture.ContentLength > 0)
                {
                    Guid? avatarGuid = CreateImage(uploadPicture);
                    //if file was stored successfully     
                    department.MainImageID = avatarGuid;
                }
                if (uploadBanner != null && uploadBanner.ContentLength > 0)
                {
                    Guid? avatarGuid = CreateImage(uploadBanner);
                    //if file was stored successfully     
                    department.BannerImageID = avatarGuid;
                }

                db.Departments.Add(department);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BannerImageID = new SelectList(db.Files, "FileID", "FileNumber", department.BannerImageID);
            ViewBag.MainImageID = new SelectList(db.Files, "FileID", "FileNumber", department.MainImageID);
            return View(department);
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Include(d => d.File).Include(d => d.File1).SingleOrDefault(s => s.DepartmentID == id);


          

          

            if (department == null)
            {
                return HttpNotFound();
            }
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
          Request.ApplicationPath.TrimEnd('/') + "/";
            ViewBag.baseUrl = baseUrl;
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "DepartmentID,Title,MaxSize,DeptDescription,StartDate,MonthlyExpenses")] Department department,
            HttpPostedFileBase uploadPicture,
            HttpPostedFileBase uploadBanner)
        {
            Department existingDept = null;
            if (ModelState.IsValid)
            {
                //finding the existing dept
                existingDept = db.Departments.Include(d => d.File).Include(d => d.File1).Where(x => x.DepartmentID == department.DepartmentID).FirstOrDefault();
                if (uploadPicture != null && uploadPicture.ContentLength > 0)
                {
                    Guid? avatarGuid = CreateImage(uploadPicture);
                    //if file was stored successfully     
                    existingDept.MainImageID = avatarGuid;
                }
                if (uploadBanner != null && uploadBanner.ContentLength > 0)
                {
                    Guid? avatarGuid = CreateImage(uploadBanner);
                    //if file was stored successfully     
                    existingDept.BannerImageID = avatarGuid;
                }

                existingDept.DeptDescription = department.DeptDescription;
                existingDept.MaxSize = department.MaxSize;
                existingDept.MonthlyExpenses = department.MonthlyExpenses;
                existingDept.StartDate = department.StartDate;
                existingDept.Title = department.Title;

                db.Entry(existingDept).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
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

        private Guid? CreateImage(HttpPostedFileBase uploadPicture)
        {
            try
            {
                Guid avatarGuid = System.Guid.NewGuid();
                var avatar = new File
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


        public ActionResult GenerateMonthlyReport(int departmentID, int month, int year)
        {
            try
            {
                HttpContext.Response.Clear();
                HttpContext.Response.ContentType = "application/ms-excel";
                HttpContext.Response.AddHeader("Content-Disposition",
                "attachment; filename=" + "Report.xlsx" + ";");
                byte[] array = Utils.ExcelReportGenerator.GenerateExcelReportForDepartment(departmentID, month, year, db);

                HttpContext.Response.OutputStream.Write(array, 0, array.Length);
                HttpContext.Response.End();
            }
            catch (Exception ex)
            {
                //error handling
            }

            return View();
        }


    }
}
