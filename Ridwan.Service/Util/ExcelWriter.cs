using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ridwan.Service.Util
{
    public class ExcelWriter
    {
        public static string ListToExcel(object reportData, string fileName)
        {
            //create timestamp
            String timeStamp = GetTimestamp(DateTime.Now);

            //created file name 
            var fileNameStamp = $"{fileName}_{timeStamp}";
            var location = @"../data";
            var excelPath = Path.Combine(location + "/", fileNameStamp + ".csv");

            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }

            //create file
            File.Create(excelPath).Dispose();

            DataTable table = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(reportData), (typeof(DataTable)));


            //write into excel file
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(excelPath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workBookPart = document.AddWorkbookPart();
                workBookPart.Workbook = new Workbook();

                WorksheetPart workSheetPart = workBookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                workSheetPart.Worksheet = new Worksheet(sheetData);

                Sheets sheets = workBookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = workBookPart.GetIdOfPart(workSheetPart), SheetId = 1, Name = fileName };

                sheets.Append(sheet);

                Row headerRow = new Row();
                List<string> columns = new List<string>();

                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                foreach (DataColumn column in table.Columns)
                {
                    columns.Add(column.ColumnName);
                    string name = column.ColumnName.Replace("_", " ");
                    name = textInfo.ToTitleCase(name);

                    Cell cell = new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(name)
                    };
                    headerRow.AppendChild(cell);
                }

                sheetData.AppendChild(headerRow);

                foreach (DataRow dsrow in table.Rows)
                {
                    Row newRow = new Row();
                    foreach (String col in columns)
                    {

                        Cell cell = new Cell
                        {
                            DataType = CellValues.String
                        };

                        cell.CellValue = new CellValue(dsrow[col].ToString());


                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }

                workBookPart.Workbook.Save();
            }

            var excelLink = fileNameStamp + ".csv";

            return excelLink;
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public static String GetDate(DateTime value)
        {
            return value.ToString("yyyyMMdd");
        }

    }
}
