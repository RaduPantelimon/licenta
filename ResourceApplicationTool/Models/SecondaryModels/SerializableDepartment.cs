using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResourceApplicationTool.Models.SecondaryModels
{
    public class SerializableDepartment
    {
        public int DepartmentID { get; set; }
        public string Title { get; set; }
        public List<SerializableEmployee> Manangers;

        public SerializableDepartment()
        {
            this.Manangers = new List<SerializableEmployee>();

        }
        public SerializableDepartment(string _Title, int _DepartmentID)
        {
            this.Manangers = new List<SerializableEmployee>();
            this.Title = _Title;
            this.DepartmentID = _DepartmentID;

        }
    }
}