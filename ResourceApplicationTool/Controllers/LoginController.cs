using System;
using System.Collections.Generic;
using System.Configuration;

using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.Security.Claims;

using System.Linq;
using System.Web;
using System.Web.Mvc;

using ResourceApplicationTool.Utils;

namespace ResourceApplicationTool.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            FederatedAuthentication.WSFederationAuthenticationModule.RedirectToIdentityProvider("http://localhost:24983/", "http://localhost:9919/Login/SetLoginConfiguration", true);
            return View();
        }
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
                return View();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}