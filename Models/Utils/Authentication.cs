using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models;

namespace Models.Utils
{
    public class Authentication
    {
        private static RATV3Entities2 db = new RATV3Entities2();

        public static Employee GetEmployee(string account, string password)
        {
            Employee emp = db.Employees.Where(x => x.Account == account && x.Password == password).FirstOrDefault();

            return emp;
        }

        public static Employee GetEmployee(string account)
        {
            Employee emp = db.Employees.Where(x => x.Account == account).FirstOrDefault();

            return emp;
        }

    }
}
