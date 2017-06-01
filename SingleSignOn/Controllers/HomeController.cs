using SingleSignOn.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Configuration;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace SingleSignOn.Controllers
{
    public class HomeController : Controller
    {
        public const string Action = "wa";
        public const string SignIn = "wsignin1.0";
        public const string SignOut = "wsignout1.0";

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var action = Request.QueryString[Action];

                if (action == SignIn)
                {
                    var formData = ProcessSignIn(Request.Url, (ClaimsPrincipal)User);
                    return new ContentResult() { Content = formData, ContentType = "text/html" };
                }
                else if (action == SignOut)
                {
                    ProcessSignOut(Request.Url, (ClaimsPrincipal)User, (HttpResponse)HttpContext.Items["HttpResponse"]);
                }
            }

            return View();
        }

        private static string ProcessSignIn(Uri url, ClaimsPrincipal principal)
        {
            SignInRequestMessage requestMSG = (SignInRequestMessage)WSFederationMessage.CreateFromUri(url);
            X509SigningCredentials credentials = new X509SigningCredentials
                (GetX509Cert(ConfigurationManager.AppSettings["SigningCertificateName"]));

            SecurityTokenServiceConfiguration config = 
                new SecurityTokenServiceConfiguration(ConfigurationManager.AppSettings["IssuerName"], credentials);

            CustomSecurityTokenService sts = new CustomSecurityTokenService(config);
            SignInResponseMessage finalResponse = 
                FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(requestMSG, principal, sts);
            
            return finalResponse.WriteFormPost();
        }

        private static void ProcessSignOut(Uri uri, ClaimsPrincipal user, HttpResponse response)
        {
            // Prepare url to internal logout page (which signs-out of all relying parties).
            string url = uri.OriginalString;
            int index = url.IndexOf("&wreply=");
            if (index != -1)
            {
                index += 8;
                string baseUrl = url.Substring(0, index);
                string wreply = url.Substring(index, url.Length - index);

                // Get the base url (domain and port).
                string strPathAndQuery = uri.PathAndQuery;
                string hostUrl = uri.AbsoluteUri.Replace(strPathAndQuery, "/");

                wreply = HttpUtility.UrlEncode(hostUrl + "logout?wreply=" + wreply);

                url = baseUrl + wreply;
            }

            // Redirect user to logout page (which signs out of all relying parties and redirects back to originating relying party).
            uri = new Uri(url);

            var requestMessage = (SignOutRequestMessage)WSFederationMessage.CreateFromUri(uri);
            FederatedPassiveSecurityTokenServiceOperations.ProcessSignOutRequest(requestMessage, user, requestMessage.Reply, response);        }
        public static X509Certificate2 GetX509Cert(string subjectName)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            X509Certificate2Collection certificates = null;
            store.Open(OpenFlags.ReadOnly);

            try
            {
                certificates = store.Certificates;
                var certs = certificates.OfType<X509Certificate2>().Where(x => x.SubjectName.Name.Equals(subjectName, StringComparison.OrdinalIgnoreCase)).ToList();

                if (certs.Count == 0)
                    throw new ApplicationException(string.Format("No certificate was found for subject Name {0}", subjectName));
                else if (certs.Count > 1)
                    throw new ApplicationException(string.Format("There are multiple certificates for subject Name {0}", subjectName));

                return new X509Certificate2(certs[0]);
            }
            finally
            {
                if (certificates != null)
                {
                    for (var i = 0; i < certificates.Count; i++)
                    {
                        var cert = certificates[i];
                        cert.Reset();
                    }
                }
                store.Close();
            }
        }
    }



}
