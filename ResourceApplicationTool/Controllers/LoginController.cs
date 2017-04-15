using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ResourceApplicationTool.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            FederatedAuthentication.WSFederationAuthenticationModule.RedirectToIdentityProvider("http://localhost:24983/", "http://localhost:9919/", true);
            return View();
        }
        public ActionResult Logout()
        {
            // Load Identity Configuration
            FederationConfiguration config = FederatedAuthentication.FederationConfiguration;

            // Sign out of WIF.
            WSFederationAuthenticationModule.FederatedSignOut(new Uri(ConfigurationManager.AppSettings["ida:Issuer"]), new Uri(config.WsFederationConfiguration.Realm));

            return View();
        }
    }
}