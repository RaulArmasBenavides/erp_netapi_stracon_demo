using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMovies.Infrastructure.Helper
{
    public static class FileHelper
    {
        public static Stream ReadFileFromServer(string rutaArchivo)
        {
            using FileStream fileStream = new FileStream(rutaArchivo, FileMode.Open, FileAccess.Read);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                fileStream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
        }

        public static Stream ReadFileFromServer(string rutaArchivo, FileMode fileMode, FileAccess fileAccess)
        {
            using (FileStream fileStream = new FileStream(rutaArchivo, fileMode, fileAccess))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    return memoryStream;
                }
            }
        }

        public static string GetExtension(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                int lastDotIndex = fileName.LastIndexOf('.');
                if (lastDotIndex != -1)
                {
                    string extension = fileName.Substring(lastDotIndex + 1);
                    return extension;
                }
            }

            return string.Empty;
        }
    }
}
