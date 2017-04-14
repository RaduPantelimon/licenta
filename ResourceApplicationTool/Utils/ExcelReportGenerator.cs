﻿using System;
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


                    //create header rows
                    List<string> MergeableRows = new List<string>();
                    Row projectRow = CreateProjectRow(firstDayOfMonth, lastDayOfMonth, project, MergeableRows, 1);
                    Row sprintRow = CreateSprintRow(firstDayOfMonth,lastDayOfMonth,sprints, MergeableRows, 2);
                    Row headerRow = CreateDatesRow(firstDayOfMonth, lastDayOfMonth, 3);

                    sheetData.AppendChild(projectRow);
                    sheetData.AppendChild(sprintRow);
                    sheetData.AppendChild(headerRow);


                    //Merging cells
                    MergeCells mergeCells = new MergeCells();
                    for (int i= 0;i<MergeableRows.Count;i+=2)
                    {
                        mergeCells.Append(new MergeCell() { Reference = new StringValue(MergeableRows[i] + ":" + MergeableRows[i+1]) });
                    }

                    //creating content rows
                    int colIndex = 4;
                    foreach (Employee emp in employees)
                    {
                        //creating row for this employee
                        List<Task>  empTasks = tasks.Where(x => x.EmployeeID == emp.EmployeeID).ToList();
                        Row row = CreateEmployeeRow(firstDayOfMonth, lastDayOfMonth, colIndex, emp, empTasks);
                        sheetData.AppendChild(row);

                        colIndex++;
                    }


                    //set col width
                    Columns columns = new Columns();
                    columns.Append(new Column() { Min = 1, Max = 1, Width = 20, CustomWidth = true });
                    columns.Append(new Column() { Min = 2, Max = 100, Width = 12, CustomWidth = true });
                    worksheetPart.Worksheet.Append(columns);

                    //merging cells
                    worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());

                    //save data
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
        private static Row CreateProjectRow(DateTime firstDayOfMonth, DateTime lastDayOfMonth, Project project, List<string> mergeableRows, int rowIndex)
        {
            int index = -1;
            Row projectRow = new Row();
            projectRow.RowIndex = (UInt32)rowIndex;
            DateTime dt = firstDayOfMonth;

            //adding first cell
            mergeableRows.Add(ColumnLetter(index + 1) + rowIndex);
            Cell firstcell = CreateCell(project.Title, ref index, rowIndex);
            projectRow.AppendChild(firstcell);

            while (dt.Date <= lastDayOfMonth.Date)
            {
                string text = "";
                Cell newcell = CreateCell(text, ref index, rowIndex);

                dt = dt.AddDays(1);
                projectRow.AppendChild(newcell);
            }
            mergeableRows.Add(ColumnLetter(index + 1) + rowIndex);
            return projectRow;
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
            Cell totalcell = CreateCell("Total", ref index, rowIndex);
            headerRow.AppendChild(totalcell);

            return headerRow;
        }

        private static Row  CreateEmployeeRow(DateTime firstDayOfMonth, DateTime lastDayOfMonth, int colIndex, Employee employee, List<Task> empTasks)
        {
            // New Row
            Row row = new Row();
            row.RowIndex = (UInt32)colIndex;
            int total = 0, rowIndex = -1;
            DateTime day = firstDayOfMonth;

            Cell titleCell = CreateCell(employee.FirstName + " " + employee.LastName, ref rowIndex, colIndex);
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
                Cell newcell = CreateNumberContentCell(hours, ref rowIndex, colIndex);
                day = day.AddDays(1);
                row.AppendChild(newcell);
                total += hours;
            }
            string finish = ColumnLetter(rowIndex) + colIndex;

            string Formula = "SUM(" + start + ":" + finish + ")";

            Cell totalCell = CreateFormulaCell(Formula,total.ToString(), ref rowIndex, colIndex);
            row.AppendChild(totalCell);


            return row;
        }

        private static Cell CreateNumberContentCell(int value, ref int index, int colIndex)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.Number;
            // Column A1, 2, 3 ... and so on
            cell.CellReference = ColumnLetter(++index) + colIndex;
            // Create Text object
            /*Text t = new Text();
            t.Text = value;

            // Append Text to InlineString object
            InlineString inlineString = new InlineString();
            inlineString.AppendChild(t);*/

            // Append InlineString to Cell
            cell.CellValue = new CellValue(value.ToString());
            return cell;
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
        private static Cell CreateFormulaCell(string Formula, string value, ref int index, int colIndex)
        {
            Cell cell = new Cell() { CellReference = ColumnLetter(++index) + colIndex };
            CellFormula cellformula = new CellFormula();
            cellformula.Text = Formula;
            CellValue cellValue = new CellValue();
            cellValue.Text = value;
            cell.Append(cellformula);
            cell.Append(cellValue);

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