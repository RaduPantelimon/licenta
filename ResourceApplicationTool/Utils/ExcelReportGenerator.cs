using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ResourceApplicationTool.Models;
using System.Globalization;


using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

namespace ResourceApplicationTool.Utils
{
    public class ExcelReportGenerator
    {
        public static byte[] GenerateExcelReport(int ProjectID, int month, int year, RATV3Entities db)
        {

            Project project = db.Projects.Where(x => x.ProjectID == ProjectID).FirstOrDefault();
            if(project!= null)
            {
                #region GetData
                //getting the department
                Department department = db.Departments.Where(x => x.DepartmentID == project.DepartmentID).FirstOrDefault();
                
                //setting the date interval for our Sprints
                DateTime firstDayOfMonth = new DateTime(year, month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddTicks(-1);

                while (firstDayOfMonth.DayOfWeek != DayOfWeek.Monday)
                {
                    firstDayOfMonth = firstDayOfMonth.AddDays(-1);
                }

                //GETTING SPRINTS + SPRINT IDS
                List<Sprint> sprints = db.Sprints.Where(x => x.ProjectID == ProjectID &&
                                                        (x.StartDate.Date >= firstDayOfMonth.Date && x.EndDate.Date <= lastDayOfMonth.Date))
                                                        .OrderBy(x => x.StartDate).ToList();
                int[] sprintIds = new int[] { 0 };
                if(sprints!= null && sprints.Count>0)
                {
                    sprintIds = sprints.Select(x => x.SprintID).ToArray();
                }

                //Getting Employees and Tasks
                List<Employee> employees = db.Employees.Where(x => x.DepartmentID == department.DepartmentID).ToList();
                List<Task> tasks = db.Tasks.Where(x => x.SprintID.HasValue && sprintIds.Contains(x.SprintID.Value)).ToList();
                #endregion
            }



            byte[] result = new byte[10000];

            return result;
        }

        private static Cell CreateCell(string value, ref int index, int colIndex)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.InlineString;
            // Column A1, 2, 3 ... and so on
            cell.CellReference = ColumnLetter(++index) + colIndex;
            // Create Text object
            Text t = new Text();
            t.Text = value;

            // Append Text to InlineString object
            InlineString inlineString = new InlineString();
            inlineString.AppendChild(t);

            // Append InlineString to Cell
            cell.AppendChild(inlineString);
            return cell;
        }
        private static string ColumnLetter(int intCol)
        {
            var intFirstLetter = ((intCol) / 676) + 64;
            var intSecondLetter = ((intCol % 676) / 26) + 64;
            var intThirdLetter = (intCol % 26) + 65;

            var firstLetter = (intFirstLetter > 64)
                ? (char)intFirstLetter : ' ';
            var secondLetter = (intSecondLetter > 64)
                ? (char)intSecondLetter : ' ';
            var thirdLetter = (char)intThirdLetter;

            return string.Concat(firstLetter, secondLetter,
                thirdLetter).Trim();
        }
    }
}