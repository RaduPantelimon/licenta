using SingleSignOn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Models.Models;
using Models.Utils;
namespace SingleSignOn.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginModel loginModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Employee emp = Authentication.GetEmployee(loginModel.Username, loginModel.Password);
                if (emp!=null)
                {
                    FormsAuthentication.SetAuthCookie(loginModel.Username, true);
                    return Redirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "The username or password provided is incorrect.");
                }
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(loginModel);
        }
    }
}
