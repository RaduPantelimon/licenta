using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ResourceApplicationTool.Models;

namespace ResourceApplicationTool.Controllers
{
    public class EmployeesController : Controller
    {
        private RATV3Entities db = new RATV3Entities();

        // GET: Employees
        public ActionResult Index()
        {
            var employees = db.Employees.Include(e => e.Department).Include(e => e.Employee1).Include(e => e.Role);
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
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
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
        public ActionResult Create([Bind(Include = "EmployeeID,RoleID,Account,Password,ManagerID,DepartmentID,FirstName,MiddleInitial,LastName,Title,CNP,Email,PhoneNumber,Salary,PriorSalary,LastRaise,HireDate,TerminationDate,Administrator")] Employee employee)
        {
            if (ModelState.IsValid)
            {
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
            Employee employee = db.Employees.Include(e => e.Role).SingleOrDefault(x => x.EmployeeID == id);

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
        public ActionResult Edit([Bind(Include = "EmployeeID,RoleID,Account,Password,ManagerID,DepartmentID,FirstName,MiddleInitial,LastName,Title,CNP,Email,PhoneNumber,Salary,PriorSalary,LastRaise,HireDate,TerminationDate,Administrator")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
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
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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
