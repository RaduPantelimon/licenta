using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;

using Models.Utils;

namespace Models.Models
{ 
    public partial class Employee
    {
        public List<Claim> GetClaims(ClaimsPrincipal principal)
        {
            //getting the claims for the token created by the sts
            List<Claim> claims = new List<Claim>() {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", Email),
                new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", Email),
                new Claim("'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",Email),
                new Claim(System.IdentityModel.Claims.ClaimTypes.Name, Email),
                //new Claim(System.IdentityModel.Claims.ClaimTypes.NameIdentifier, Email),
                new Claim(Const.CLAIM.CLAIM_NAMESPACE + "/" + "Email", Email) ,
                new Claim(Const.CLAIM.CLAIM_NAMESPACE + "/" + "PhoneNumber",PhoneNumber),
                new Claim(Const.CLAIM.CLAIM_NAMESPACE + "/" + Const.Fields.EMPLOYEE_ROLE_ID, ManagerID.ToString()),
                new Claim(Const.CLAIM.CLAIM_NAMESPACE + "/" + Const.Fields.EMPLOYEE_FIRST_NAME, FirstName),
                new Claim(Const.CLAIM.CLAIM_NAMESPACE + "/" + Const.Fields.EMPLOYEE_LAST_NAME, LastName),
                new Claim(Const.CLAIM.CLAIM_NAMESPACE + "/" + Const.Fields.EMPLOYEE_ACCESS, Administrator),
                new Claim(Const.CLAIM.CLAIM_NAMESPACE + "/" + Const.Fields.EMPLOYEE_FIRST_NAME, FirstName),
                new Claim(Const.CLAIM.CLAIM_NAMESPACE + "/" + Const.Fields.EMPLOYEE_LAST_NAME, LastName),
                new Claim(Const.CLAIM.CLAIM_NAMESPACE + "/" + Const.Fields.EMPLOYEE_ACCOUNT, Account),
                new Claim(Const.CLAIM.CLAIM_NAMESPACE + "/" + Const.Fields.EMPLOYEE_DEPARTMENT_ID, DepartmentID.ToString()),
                new Claim(Const.CLAIM.CLAIM_NAMESPACE + "/" + Const.Fields.EMPLOYEE_ROLE_ID, RoleID.ToString()),
                new Claim(Const.CLAIM.CLAIM_NAMESPACE + "/" + Const.Fields.EMPLOYEE_ID, EmployeeID.ToString()),
                new Claim(Const.CLAIM.CLAIM_NAMESPACE + "/" + Const.Fields.EMPLOYEE_HIRE_DATE, HireDate.ToString())
             };

            return claims;

        }
    }
}
