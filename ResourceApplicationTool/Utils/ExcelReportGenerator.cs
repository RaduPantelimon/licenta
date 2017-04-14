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
using System.IO;

namespace ResourceApplicationTool.Utils
{
    public class ExcelReportGenerator
    {
        public static byte[] GenerateExcelReport(int ProjectID, int month, int year, RATV3Entities db)
        {
            byte[] result = new byte[0];
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
                while(lastDayOfMonth.DayOfWeek!=DayOfWeek.Sunday)
                {
                    lastDayOfMonth = lastDayOfMonth.AddDays(1);
                }

                //GETTING SPRINTS + SPRINT IDS
                List<Sprint> sprints = db.Sprints.Where(x => x.ProjectID == ProjectID &&
                                                        (DbFunctions.TruncateTime(x.StartDate) >= firstDayOfMonth.Date && 
                                                        DbFunctions.TruncateTime(x.EndDate) <= lastDayOfMonth.Date))
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


                #region GenerateDocument
                MemoryStream memoryStream = new MemoryStream();
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                {
                    var workbookpart = document.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();
                    var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();

                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    var sheets = document.WorkbookPart.Workbook.
                        AppendChild<Sheets>(new Sheets());

                    var sheet = new Sheet()
                    {
                        Id = document.WorkbookPart
                        .GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Sheet 1"
                    };
                    sheets.AppendChild(sheet);

                    List<string> MergeableRows = new List<string>();
                    Row sprintRow = CreateSprintRow(firstDayOfMonth,lastDayOfMonth,sprints, MergeableRows, 1);
                    Row headerRow = CreateDatesRow(firstDayOfMonth, lastDayOfMonth, 2);
                    sheetData.AppendChild(sprintRow);
                    sheetData.AppendChild(headerRow);


                    //create a MergeCells class to hold each MergeCell
                    MergeCells mergeCells = new MergeCells();

                    //append a MergeCell to the mergeCells for each set of merged cells
                    for (int i= 0;i<MergeableRows.Count;i+=2)
                    {
                        mergeCells.Append(new MergeCell() { Reference = new StringValue(MergeableRows[i] + ":" + MergeableRows[i+1]) });
                    }
                    /*mergeCells.Append(new MergeCell() { Reference = new StringValue("C1:F1") });
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("A3:B3") });
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("G5:K5") });*/

                    worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());


                    worksheetPart.Worksheet.Save();
                    document.Close();

                }

                result = memoryStream.ToArray();
                #endregion
            }
            return result;
        }

        private static Row CreateSprintRow(DateTime firstDayOfMonth,DateTime lastDayOfMonth, List<Sprint> sprints,List<string> mergeableRows, int rowIndex)
        {
            int index = 0;
            Row sprintsRow = new Row();
            sprintsRow.RowIndex = (UInt32)rowIndex;
            DateTime dt = firstDayOfMonth;

            int weekCount = 0;
            while (dt.Date <= lastDayOfMonth.Date)
            {
                string text = "";

                if(dt.DayOfWeek == DayOfWeek.Monday && sprints.Any(x => x.StartDate.Date ==dt.Date))
                {
                    text += "W" + (++weekCount);
                    mergeableRows.Add(ColumnLetter(index+1) + rowIndex);
                }
                else if(dt.DayOfWeek == DayOfWeek.Monday)
                {
                    text += "(Empty Weeek)";
                }

                if(dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    mergeableRows.Add(ColumnLetter(index+1) + rowIndex);
                }

                Cell newcell = CreateCell(text, ref index, rowIndex);

                dt = dt.AddDays(1);
                sprintsRow.AppendChild(newcell);
            }

            return sprintsRow;
        }
        private static Row CreateDatesRow(DateTime firstDayOfMonth, DateTime lastDayOfMonth, int rowIndex)
        {
            int index = 0;
            Row headerRow = new Row();
            headerRow.RowIndex = (UInt32)rowIndex;
            DateTime dt = firstDayOfMonth;
            while (dt.Date <= lastDayOfMonth.Date)
            {

                Cell newcell = CreateCell(dt.ToString("dd/MM/yyyy"), ref index, rowIndex);

                dt = dt.AddDays(1);
                headerRow.AppendChild(newcell);
            }
            Cell totalcell = CreateCell("Total", ref index, 1);
            headerRow.AppendChild(totalcell);

            return headerRow;
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