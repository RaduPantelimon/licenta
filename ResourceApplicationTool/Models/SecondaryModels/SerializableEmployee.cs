using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResourceApplicationTool.Models.SecondaryModels
{
    public class SerializableEmployee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Account { get; set; }
        public int EmployeeID { get; set; }
        public int ManagerID { get; set; }
        public int DepartmentID { get; set; }
        public int RoleID { get; set; }
        public SerializableEmployee()
        {

        }
        public SerializableEmployee(string _FirstName, string _LastName, string _Account, int _EmployeeID)
        {
            this.FirstName = _FirstName;
            this.LastName = _LastName;
            this.Account = _Account;
            this.EmployeeID = _EmployeeID;
        }
    }
}