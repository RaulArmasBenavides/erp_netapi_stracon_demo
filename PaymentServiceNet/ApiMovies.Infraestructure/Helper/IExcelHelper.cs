using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMovies.Infrastructure.Helper
{
    public interface IExcelHelper
    {
        void CreateTable(string fileName, string[] headers, string[][] data);
        void SetearFormatoTabla(ref object hojaExcel, bool startWithCellBelow);
    }
}
