using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ResourceApplicationTool.Controllers
{
    public class NotificationsController : Controller
    {
        // GET: Notifications
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Audit()
        {
            return View();
        }

        public ActionResult PerformanceReview()
        {
            return View();
        }

        public ActionResult SprintReviewMeeting()
        {
            return View();
        }

        public ActionResult DailyScrumMeeting()
        {
            return View();
        }

        public ActionResult DepartmentMonthlyMeeting()
        {
            return View();
        }
        public ActionResult CanceledEvent()
        {
            return View();
        }
    }
}