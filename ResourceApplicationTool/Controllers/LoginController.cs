using System;
using System.Collections.Generic;
using System.Configuration;

using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.Security.Claims;

using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;

using ResourceApplicationTool.Utils;
using ResourceApplicationTool.Models;


namespace ResourceApplicationTool.Controllers
{
    public class LoginController : Controller
    {
        private RATV3Entities db = new RATV3Entities();

        // GET: Login
        public ActionResult Index()
        {
            FederatedAuthentication.WSFederationAuthenticationModule.RedirectToIdentityProvider("http://localhost:24983/", "http://localhost:9919/Login/SetLoginConfiguration", true);
            return View();
        }

        //will be called after returning from the identity provider
        public ActionResult SetLoginConfiguration()
        {
            if (User.Identity.IsAuthenticated)
            {
                //set authentication session
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
                IEnumerable<Claim> claims = claimsIdentity.Claims;

                Session[Const.CLAIM.USER_ACCESS_LEVEL] = claims.Where(x => x.Type == Const.CLAIM.CLAIM_NAMESPACE
                                + "/" + Const.Fields.EMPLOYEE_ACCESS).FirstOrDefault().Value;
                Session[Const.CLAIM.USER_ACCOUNT] = claims.Where(x => x.Type == Const.CLAIM.CLAIM_NAMESPACE
                                + "/" + Const.Fields.EMPLOYEE_ACCOUNT).FirstOrDefault().Value;
                Session[Const.CLAIM.USER_ID] = claims.Where(x => x.Type == Const.CLAIM.CLAIM_NAMESPACE
                + "/" + Const.Fields.EMPLOYEE_ID).FirstOrDefault().Value;
                string firstName = claims.Where(x => x.Type == Const.CLAIM.CLAIM_NAMESPACE
                    + "/" + Const.Fields.EMPLOYEE_FIRST_NAME).FirstOrDefault().Value;
                string lastName = claims.Where(x => x.Type == Const.CLAIM.CLAIM_NAMESPACE
                    + "/" + Const.Fields.EMPLOYEE_LAST_NAME).FirstOrDefault().Value;

                Session[Const.CLAIM.USER_FIRST_NAME] = firstName;
                Session[Const.CLAIM.USER_LAST_NAME] = lastName;

            }
            return RedirectToAction("Index","Home");
        }

        //will delete session and call the identity provider
        public ActionResult Logout()
        {
            //delete authentication session
            Session[Const.CLAIM.USER_ACCESS_LEVEL] = null;
            Session[Const.CLAIM.USER_ACCOUNT] = null;
            Session[Const.CLAIM.USER_ID] = null;
            Session[Const.CLAIM.USER_FIRST_NAME] = null;
            Session[Const.CLAIM.USER_LAST_NAME] = null;

            // Load Identity Configuration
            FederationConfiguration config = FederatedAuthentication.FederationConfiguration;

            // Sign out of WIF.
            WSFederationAuthenticationModule.FederatedSignOut(new Uri(ConfigurationManager.AppSettings["ida:Issuer"]), new Uri(config.WsFederationConfiguration.Realm));

            return View();
        }

        public ActionResult UserProfile()
        {
            if (User.Identity.IsAuthenticated && Session[Const.CLAIM.USER_ID] != null)
            {
                try
                {
                    int empID = Convert.ToInt32(Session[Const.CLAIM.USER_ID]);
                    Employee emp = db.Employees.Where(x => x.EmployeeID == empID).FirstOrDefault();

                    string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                    Request.ApplicationPath.TrimEnd('/') + "/";

                    //getting the picture ready
                    if (emp.File != null)
                    {

                        ViewBag.ImgID = Const.PicturePaths.ImgControllerRoot + emp.File.FileNumber;
                        ViewBag.ImgIDSec = baseUrl + Const.PicturePaths.ImgControllerRoot + emp.File.FileNumber;
                    }
                    else
                    {
                        ViewBag.ImgID = Const.PicturePaths.ProfilePictureUrl;
                        ViewBag.ImgIDSec = baseUrl + Const.PicturePaths.ProfilePictureUrl;
                    }
                    if (emp.Role != null && emp.Role.Name != null)
                    {
                        ViewBag.RoleName = emp.Role.Name;
                    }
                    else
                    {
                        ViewBag.RoleName = "";
                    }

                    if (emp != null)
                    {
                        return View(emp);
                    }
                }
                catch (Exception)
                {
                    //possible db or conversion error
                    //handle errors
                    return RedirectToAction("NotFound", "Home");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Employees/Edit/5
        public ActionResult ChangePassword(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Include(e => e.Role).Include(e => e.Educations).SingleOrDefault(x => x.EmployeeID == id);

            //checking if we have the permission necessary to change the password for this user
            if (!User.Identity.IsAuthenticated ||
                     Session[Const.CLAIM.USER_ID].ToString() != employee.EmployeeID.ToString())
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (employee == null)
            {
                return HttpNotFound();
            }
            if (employee.Role != null && employee.Role.Name != null)
            {
                ViewBag.RoleName = employee.Role.Name;
            }
            else
            {
                ViewBag.RoleName = "";
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(
           int employeeID,
           string password,
           string confirmPassword,
           string oldPassword)
        {
            if (ModelState.IsValid)
            {

                Employee employee = db.Employees.Where(x => x.EmployeeID == employeeID).FirstOrDefault();

                //checking if we have the permission necessary to change the password for this user
                if (employee == null || !User.Identity.IsAuthenticated ||
                         Session[Const.CLAIM.USER_ID].ToString() != employee.EmployeeID.ToString())
                {
                    return RedirectToAction("NotFound", "Home");
                }

                if(oldPassword != employee.Password)
                {
                    ModelState.AddModelError("IncorrectOldPassword", "Old Password not correct");
                    return View();
                }
                if(password != confirmPassword)
                {
                    ModelState.AddModelError("PasswordsDoNotMatch", "Passwords Do Not Match");
                    return View();
                }

                db.Entry(employee).Property(X => X.Password).IsModified = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();

        }
    }
}