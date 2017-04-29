using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResourceApplicationTool.Models.SecondaryModels
{
    public class ExistingAttendant
    {
        public string value;
        public string text;
        public int empID;

        public ExistingAttendant(
            string _value,
            string _text,
            int _empID) {

            value = _value;
            text = _text;
            empID = _empID;
        }
    }
}