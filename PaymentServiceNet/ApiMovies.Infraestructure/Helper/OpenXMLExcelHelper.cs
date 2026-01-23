using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMovies.Infrastructure.Helper
{
    public class OpenXMLExcelHelper : IExcelHelper
    {
        public void CreateTable(string fileName, string[] headers, string[][] data)
        {
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook))
            {
                // Agregar la hoja de cálculo al libro de trabajo
                WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
                sheets.Append(sheet);

                // Agregar los encabezados
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                Row headerRow = new Row();
                for (int i = 0; i < headers.Length; i++)
                {
                    Cell cell = CreateTextCell(GetColumnName(i + 1), 1, headers[i]);
                    headerRow.Append(cell);
                }
                sheetData.Append(headerRow);

                // Agregar los datos
                for (int i = 0; i < data.Length; i++)
                {
                    Row row = new Row();
                    for (int j = 0; j < data[i].Length; j++)
                    {
                        Cell cell = CreateTextCell(GetColumnName(j + 1), (uint)(i + 2), data[i][j]);
                        row.Append(cell);
                    }
                    sheetData.Append(row);
                }

                workbookPart.Workbook.Save();
            }
        }

        public void SetearFormatoTabla(ref object hojaExcel, bool startWithCellBelow)
        {
            throw new NotImplementedException();
        }
        private Cell CreateTextCell(string header, uint index, string text)
        {
            Cell cell = new Cell() { DataType = CellValues.InlineString, CellReference = header + index };
            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = text;
            inlineString.AppendChild(t);
            cell.AppendChild(inlineString);
            return cell;
        }

        private string GetColumnName(int columnIndex)
        {
            int dividend = columnIndex;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
    }
}
