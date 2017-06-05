using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using ResourceApplicationTool.Models;
using System.Security.Principal;
using System.Security;

namespace ResourceApplicationTool.Utils
{
    public class CustomActions: ActionFilterAttribute
    {
        private static RATV3Entities db = new RATV3Entities();

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var session = context.HttpContext.Session;
            var currentUser = context.HttpContext.User;

            //if the user is authenticated, we'll set the session
            if (currentUser.Identity.IsAuthenticated)
            {
                setSession(currentUser, session);
            }
            base.OnActionExecuting(context);
        }

        public static void setSession(IPrincipal currentUser, HttpSessionStateBase Session)
        {
            //set authentication session
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)currentUser.Identity;
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


            //setting user Role + Department
            int empID = Convert.ToInt32(Session[Const.CLAIM.USER_ID]);
            Employee emp = db.Employees.Where(x => x.EmployeeID == empID).FirstOrDefault();

            if (emp.Department != null)
            {
                Session[Const.CLAIM.USER_DEPARTMENT] = emp.Department.Title;
            }
            if (emp.Role != null)
            {
                Session[Const.CLAIM.USER_ROLE] = emp.Role.Name;
            }
        }
    }


}