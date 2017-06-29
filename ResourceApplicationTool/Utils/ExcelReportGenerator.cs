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
                


                #region GenerateDocument
                MemoryStream memoryStream = new MemoryStream();
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                {
                    var workbookpart = document.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();
                    //add Style
                    WorkbookStylesPart stylePart = workbookpart.AddNewPart<WorkbookStylesPart>();
                    stylePart.Stylesheet = GenerateStylesheet();
                    stylePart.Stylesheet.Save();

                    //var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                  

                    var sheets = document.WorkbookPart.Workbook.
                        AppendChild<Sheets>(new Sheets());


                    CreateProjectSheet(document, workbookpart, sheets, ProjectID, month, year, db);



                    //save data
                    //worksheetPart.Worksheet.Save();
                    document.Close();
                }

                result = memoryStream.ToArray();
                #endregion
            }
            return result;
        }

        public static byte[] GenerateExcelReportForDepartment(int DepartmentID, int month, int year, RATV3Entities db)
        {
            byte[] result = new byte[0];
            Department department = db.Departments.Where(x => x.DepartmentID == DepartmentID).FirstOrDefault();
            if (department != null)
            {



                #region GenerateDocument
                MemoryStream memoryStream = new MemoryStream();
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(memoryStream, 
                    SpreadsheetDocumentType.Workbook))
                {
                    var workbookpart = document.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();
                    //add Style
                    WorkbookStylesPart stylePart = workbookpart.AddNewPart<WorkbookStylesPart>();
                    stylePart.Stylesheet = GenerateStylesheet();
                    stylePart.Stylesheet.Save();

                    var sheets = document.WorkbookPart.Workbook.
                        AppendChild<Sheets>(new Sheets());
                    uint sheetID = 1;

                    foreach(Project projectID in department.Projects)
                    {
                        CreateProjectSheet(document, workbookpart, sheets, projectID.ProjectID, month, year, db, sheetID);
                        sheetID++;
                    }
                    
                    //save data
                    document.Close();
                }

                result = memoryStream.ToArray();
                #endregion
            }
            return result;
        }


        private static Sheet CreateProjectSheet(SpreadsheetDocument document,
            WorkbookPart workbookpart,
            Sheets sheets, 
            int ProjectID, 
            int month, 
            int year, 
            RATV3Entities db,
            uint sheetID = 1)
        {
            #region GetData
            //getting the department
            Project project = db.Projects.Where(x => x.ProjectID == ProjectID).FirstOrDefault();
            Department department = db.Departments.Where(x => x.DepartmentID == project.DepartmentID).FirstOrDefault();

            //setting the date interval for our Sprints
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddTicks(-1);

            while (firstDayOfMonth.DayOfWeek != DayOfWeek.Monday)
            {
                firstDayOfMonth = firstDayOfMonth.AddDays(-1);
            }
            while (lastDayOfMonth.DayOfWeek != DayOfWeek.Sunday)
            {
                lastDayOfMonth = lastDayOfMonth.AddDays(1);
            }

            //GETTING SPRINTS + SPRINT IDS
            List<Sprint> sprints = db.Sprints.Where(x => x.ProjectID == ProjectID &&
                                                    (DbFunctions.TruncateTime(x.StartDate) >= firstDayOfMonth.Date &&
                                                    DbFunctions.TruncateTime(x.EndDate) <= lastDayOfMonth.Date))
                                                    .OrderBy(x => x.StartDate).ToList();
            int[] sprintIds = new int[] { 0 };
            if (sprints != null && sprints.Count > 0)
            {
                sprintIds = sprints.Select(x => x.SprintID).ToArray();
            }

            //Getting Employees and Tasks
            List<Employee> employees = db.Employees.Where(x => x.DepartmentID == department.DepartmentID).ToList();
            List<Task> tasks = db.Tasks.Where(x => x.SprintID.HasValue && sprintIds.Contains(x.SprintID.Value)).ToList();
            #endregion

            WorksheetPart newWorksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();

            string relationshipId = document.WorkbookPart.GetIdOfPart(newWorksheetPart);
            var sheet = new Sheet()
            {
                Id = relationshipId,
                SheetId = sheetID,
                Name = project.Title
            };
            sheets.AppendChild(sheet);

            var sheetData = new SheetData();



            newWorksheetPart.Worksheet = new Worksheet(sheetData);

            //create header rows
            DateTime monthDate = new DateTime(year, month, 1);

            List<string> MergeableRows = new List<string>();
            Row projectRow = CreateProjectRow(firstDayOfMonth, lastDayOfMonth, project, MergeableRows, monthDate, 1);
            Row sprintRow = CreateSprintRow(firstDayOfMonth, lastDayOfMonth, sprints, MergeableRows, 2);
            Row headerRow = CreateDatesRow(firstDayOfMonth, lastDayOfMonth, 3);

            sheetData.AppendChild(projectRow);
            sheetData.AppendChild(sprintRow);
            sheetData.AppendChild(headerRow);


            //Merging cells
            MergeCells mergeCells = new MergeCells();
            for (int i = 0; i < MergeableRows.Count; i += 2)
            {
                mergeCells.Append(new MergeCell() { Reference = new StringValue(MergeableRows[i] + ":" + MergeableRows[i + 1]) });
            }

            //creating content rows
            int colIndex = 4;
            foreach (Employee emp in employees)
            {
                //creating row for this employee
                List<Task> empTasks = tasks.Where(x => x.EmployeeID == emp.EmployeeID).ToList();
                Row row = CreateEmployeeRow(firstDayOfMonth, lastDayOfMonth, colIndex, emp, empTasks);
                sheetData.AppendChild(row);

                colIndex++;
            }


            //set col width
            Columns columns = new Columns();
            columns.Append(new Column() { Min = 1, Max = 1, Width = 20, CustomWidth = true });
            columns.Append(new Column() { Min = 2, Max = 100, Width = 12, CustomWidth = true });
            newWorksheetPart.Worksheet.Append(columns);

            //merging cells
            newWorksheetPart.Worksheet.InsertAfter(mergeCells, newWorksheetPart.Worksheet.Elements<SheetData>().First());

            newWorksheetPart.Worksheet.Save();
            return sheet;
        }

        private static Row CreateSprintRow(DateTime firstDayOfMonth,DateTime lastDayOfMonth, List<Sprint> sprints,List<string> mergeableRows, int rowIndex)
        {
            int index = -1;
            Row sprintsRow = new Row();
            sprintsRow.RowIndex = (UInt32)rowIndex;
            DateTime dt = firstDayOfMonth;

            Cell firstcell = CreateCell("", ref index, rowIndex, 3);
            sprintsRow.AppendChild(firstcell);

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
                    text += "(Empty Week)";
                    mergeableRows.Add(ColumnLetter(index + 1) + rowIndex);
                    ++weekCount;
                }

                if(dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    mergeableRows.Add(ColumnLetter(index+1) + rowIndex);
                }

                Cell newcell = CreateCell(text, ref index, rowIndex,3);

                dt = dt.AddDays(1);
                sprintsRow.AppendChild(newcell);
            }

            Cell lastcell = CreateCell("", ref index, rowIndex, 3);
            sprintsRow.AppendChild(lastcell);

            return sprintsRow;
        }
        private static Row CreateProjectRow(DateTime firstDayOfMonth, DateTime lastDayOfMonth, Project project, List<string> mergeableRows, DateTime monthDate, int rowIndex)
        {
            int index = -1;
            Row projectRow = new Row();
            projectRow.RowIndex = (UInt32)rowIndex;
            DateTime dt = firstDayOfMonth;

            //adding first cell
            mergeableRows.Add(ColumnLetter(index + 1) + rowIndex);
            Cell firstcell = CreateCell("  Activity report for " + 
                project.Title + " during " + monthDate.ToString("MMMM yyyy"), ref index, rowIndex,4);
            projectRow.AppendChild(firstcell);

            while (dt.Date <= lastDayOfMonth.Date)
            {
                string text = "";
                Cell newcell = CreateCell(text, ref index, rowIndex,4);

                dt = dt.AddDays(1);
                projectRow.AppendChild(newcell);
            }
            mergeableRows.Add(ColumnLetter(index + 1) + rowIndex);
            return projectRow;
        }

        private static Row CreateDatesRow(DateTime firstDayOfMonth, DateTime lastDayOfMonth, int rowIndex)
        {
            int index = -1;
            Row headerRow = new Row();
            headerRow.RowIndex = (UInt32)rowIndex;
            DateTime dt = firstDayOfMonth;

            Cell firstcell = CreateCell("", ref index, rowIndex, 2);
            headerRow.AppendChild(firstcell);

            while (dt.Date <= lastDayOfMonth.Date)
            {

                Cell newcell = CreateCell(dt.ToString("dd/MM/yyyy"), ref index, rowIndex,2);

                dt = dt.AddDays(1);
                headerRow.AppendChild(newcell);
            }
            Cell totalcell = CreateCell("Total", ref index, rowIndex,2);
            headerRow.AppendChild(totalcell);

            return headerRow;
        }

        private static Row  CreateEmployeeRow(DateTime firstDayOfMonth, DateTime lastDayOfMonth, 
            int colIndex, Employee employee, List<Task> empTasks)
        {
            // New Row
            Row row = new Row();
            row.RowIndex = (UInt32)colIndex;
            int total = 0, rowIndex = -1;
            DateTime day = firstDayOfMonth;
            Cell titleCell = CreateCell(employee.FirstName + " " + employee.LastName, ref rowIndex, colIndex,2);
            row.AppendChild(titleCell);
            //preparing the formula
            string start = ColumnLetter(1) + colIndex;
            while (day.Date <= lastDayOfMonth.Date)
            {
                Task currentTask = empTasks.Where(x =>x.StartDate.HasValue && (x.StartDate.Value.Date == day.Date)).FirstOrDefault();
                int hours = 0;
                if (currentTask != null && currentTask.Estimation.HasValue)
                {
                    hours = currentTask.Estimation.Value;
                }
                if(day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                {
                    Cell newcell = CreateNumberContentCell(hours, ref rowIndex, colIndex,5);
                    row.AppendChild(newcell);
                }
                else
                {
                    Cell newcell = CreateNumberContentCell(hours, ref rowIndex, colIndex);
                    row.AppendChild(newcell);
                }
                
                day = day.AddDays(1);
                total += hours;
            }
            string finish = ColumnLetter(rowIndex) + colIndex, Formula = "SUM(" + start + ":" + finish + ")";
            Cell totalCell = CreateFormulaCell(Formula,total.ToString(), ref rowIndex, colIndex);
            row.AppendChild(totalCell);
            return row;
        }

        private static Cell CreateNumberContentCell(int value, ref int index, int colIndex, uint styleIndex = 0)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.Number;
            cell.StyleIndex = styleIndex;
            // Column A1, 2, 3 ... and so on
            cell.CellReference = ColumnLetter(++index) + colIndex;

            // Append InlineString to Cell
            cell.CellValue = new CellValue(value.ToString());
            return cell;
        }

        private static Cell CreateCell(string value, ref int index, int colIndex, uint styleIndex = 0)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.InlineString;
            // Column A1, 2, 3 ... and so on
            cell.CellReference = ColumnLetter(++index) + colIndex;
            cell.StyleIndex = styleIndex;
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
        private static Cell CreateFormulaCell(string Formula, string value, ref int index, int colIndex, uint styleIndex = 0)
        {
            Cell cell = new Cell() { CellReference = ColumnLetter(++index) + colIndex };
            CellFormula cellformula = new CellFormula();
            cellformula.Text = Formula;
            CellValue cellValue = new CellValue();
            cellValue.Text = value;
            cell.StyleIndex = styleIndex;
            cell.Append(cellformula);
            cell.Append(cellValue);
            return cell;
        }

        private static string ColumnLetter(int intCol)
        {
            var firstDigit = ((intCol) / 676) + 64;
            var secondDigit = ((intCol % 676) / 26) + 64;
            var thirdDigit = (intCol % 26) + 65;

            var firstLetter = (firstDigit > 64)
                ? (char)firstDigit : ' ';
            var secondLetter = (secondDigit > 64)
                ? (char)secondDigit : ' ';
            var thirdLetter = (char)thirdDigit;

            return string.Concat(firstLetter, secondLetter,
                thirdLetter).Trim();
        }


        private static Stylesheet GenerateStylesheet()
         {
             Stylesheet styleSheet = null;

             Fonts fonts = new Fonts(
                new Font( // Index 0 - default
                    new FontSize() { Val = 10 }

                ),
                new Font( // Index 1 - header
                    new FontSize() { Val = 10 },
                    new Bold(),
                    new Color() { Rgb = "FFFFFF" }

                ),
                 new Font(
                    new FontSize() { Val = 10 },
                    new Bold(),
                    new Color() { Rgb = "FFFFFF" }

                ),
                 new Font(
                    new FontSize() { Val = 14 },
                    new Bold(),
                    new Color() { Rgb = "FFFFFF" }

                ));

            Fills fills = new Fills(
                     new Fill(new PatternFill() { PatternType = PatternValues.None }), // Index 0 - default
                     new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }), // Index 1 - default
                     new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "66666666" } })
                     { PatternType = PatternValues.Solid }), // Index 2 - header
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "428BCA" } })
                       { PatternType = PatternValues.Solid }), //projects
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "DDDDDD" } })
                    { PatternType = PatternValues.Solid }),//weekend Days
                     new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "47C9AF" } })
                     { PatternType = PatternValues.Solid })//sprints
                 );

             Borders borders = new Borders(
                     new Border(), // index 0 default
                     new Border( // index 1 black border
                         new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                         new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                         new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                         new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                         new DiagonalBorder())
                 );

             CellFormats cellFormats = new CellFormats(
                     new CellFormat(), // default
                     new CellFormat { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true, ApplyFont = true }, // body
                     new CellFormat {
                         Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center },
                         FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true, ApplyFont = true, ApplyAlignment=true}, // employees / dates
                     new CellFormat{
                         Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center },
                        FontId = 1, FillId = 5, BorderId = 1, ApplyFill = true, ApplyBorder = true, ApplyFont = true, ApplyAlignment=true }, // sprints
                     new CellFormat { FontId = 3, FillId = 3, BorderId = 1, ApplyFill = true, ApplyFont = true }, // project
                     new CellFormat { FontId = 0, FillId = 4, BorderId = 1, ApplyFill = true, ApplyBorder = true, ApplyFont = true }// weekend
                 );

             styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

             return styleSheet;
         }


     
    }
}
 