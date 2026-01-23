using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMovies.Infrastructure.Helper
{
    public class ClosedXMLExcelHelper : IExcelHelper
    {
        public void CreateTable(string fileName, string[] headers, string[][] data)
        {
            throw new NotImplementedException();
        }

        public void SetearFormatoTabla(ref object hojaExcelBase, bool startWithCellBelow)
        {
            IXLWorksheet  hojaExcel = (IXLWorksheet)hojaExcelBase;
            var _columns = hojaExcel.Columns();
            foreach (var c in _columns)
            {
                c.AdjustToContents();
            }
            var firstCell = hojaExcel.FirstCellUsed();
            if (startWithCellBelow)
            {
                firstCell = firstCell.CellBelow();
            }
            var lastCell = hojaExcel.LastCellUsed();
            var range = hojaExcel.Range(firstCell.Address, lastCell.Address);
            var table = range.CreateTable();// create the actual table
            table.Theme = XLTableTheme.TableStyleLight12;//apply style
        }
    }
}
