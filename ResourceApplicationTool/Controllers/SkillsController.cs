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
    public class SkillsController : Controller
    {
        private RATV3Entities db = new RATV3Entities();

        // GET: Skills
        public ActionResult Index()
        {
            if (!Common.CheckIfAdministrator(Session, User))
            {
                return RedirectToAction("NotFound", "Home");
            }
            ViewBag.userAccess = Session[Const.CLAIM.USER_ACCESS_LEVEL];
            var skills = db.Skills.Include(s => s.SkillCategory);

            //skill categories 
            List<SkillCategory> skillCategories = db.SkillCategories.ToList();
            ViewBag.skillCategories = skillCategories;

            return View(skills.ToList());
        }

        // GET: Skills/Create
        public ActionResult Create()
        {
            if (!Common.CheckIfAdministrator(Session, User))
            {
                return RedirectToAction("NotFound", "Home");
            }
            ViewBag.userAccess = Session[Const.CLAIM.USER_ACCESS_LEVEL];
            ViewBag.CategoryID = new SelectList(db.SkillCategories, "CategoryID", "Description");
            return View();
        }

        // POST: Skills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SkillID,Title,Description,CategoryID")] Skill skill)
        {
            if (!Common.CheckIfAdministrator(Session, User))
            {
                return RedirectToAction("NotFound", "Home");
            }
            ViewBag.userAccess = Session[Const.CLAIM.USER_ACCESS_LEVEL];
            if (ModelState.IsValid)
            {
                db.Skills.Add(skill);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.SkillCategories, "CategoryID", "Description", skill.CategoryID);
            return View(skill);
        }

        // GET: Skills/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!Common.CheckIfAdministrator(Session, User))
            {
                return RedirectToAction("NotFound", "Home");
            }
            ViewBag.userAccess = Session[Const.CLAIM.USER_ACCESS_LEVEL];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Skill skill = db.Skills.Find(id);
            if (skill == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.SkillCategories, "CategoryID", "Description", skill.CategoryID);
            return View(skill);
        }

        // POST: Skills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SkillID,Title,Description,CategoryID")] Skill skill)
        {
            if (!Common.CheckIfAdministrator(Session, User))
            {
                return RedirectToAction("NotFound", "Home");
            }
            ViewBag.userAccess = Session[Const.CLAIM.USER_ACCESS_LEVEL];
            if (ModelState.IsValid)
            {
                db.Entry(skill).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.SkillCategories, "CategoryID", "Description", skill.CategoryID);
            return View(skill);
        }

        // GET: Skills/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!Common.CheckIfAdministrator(Session, User))
            {
                return RedirectToAction("NotFound", "Home");
            }
            ViewBag.userAccess = Session[Const.CLAIM.USER_ACCESS_LEVEL];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Skill skill = db.Skills.Find(id);
            if (skill == null)
            {
                return HttpNotFound();
            }
            return View(skill);
        }

        // POST: Skills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Common.CheckIfAdministrator(Session, User))
            {
                return RedirectToAction("NotFound", "Home");
            }

            //removing the existing levels
            List<SkillLevel> skillLevels = db.SkillLevels.Where(x =>x.SkillID == id).ToList();
            foreach(SkillLevel sklvl in skillLevels)
            {
                db.SkillLevels.Remove(sklvl);
            }
            db.SaveChanges();

            ViewBag.userAccess = Session[Const.CLAIM.USER_ACCESS_LEVEL];
            Skill skill = db.Skills.Find(id);
            db.Skills.Remove(skill);
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
