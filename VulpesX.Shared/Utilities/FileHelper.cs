using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public static class FileHelper
    {
        public static bool IsOpen(string Filename)
        {
            FileStream? stream = null;

            try
            {
                stream = File.Open(Filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return false; // File is NOT locked
            }
            catch (IOException)
            {
                return true; // File is locked
            }
            finally
            {
                stream?.Close();
            }
        }

        public static void Open(string Filename)
        {
            var psi = new ProcessStartInfo
            {
                FileName =Filename,
                UseShellExecute = true  // This tells Windows to open with the default program
            };

            Process.Start(psi);
        }

        #region PDF
        public static string GeneratePDFFilename(string Filename)
        {
            Filename = string.Concat(Filename.Split(Path.GetInvalidFileNameChars()));
            var originalFullPath = Path.GetTempPath() + Filename;
            string fullPath = originalFullPath;
            int progressive = 1;
            while (File.Exists(fullPath) && CheckFileInUse(fullPath))
            {
                fullPath = originalFullPath.Replace(".pdf", $"_{progressive++}.pdf");
            }
            return fullPath;
        }
        #endregion

        #region ZIP
        public static string? GenerateZIPFilename(string? Filename)
        {
            if(string.IsNullOrEmpty(Filename)) return null;

            Filename = string.Concat(Filename.Split(Path.GetInvalidFileNameChars()));
            var originalFullPath = Path.GetTempPath() + Filename;
            string fullPath = originalFullPath;
            int progressive = 1;
            while (File.Exists(fullPath) && CheckFileInUse(fullPath))
            {
                fullPath = originalFullPath.Replace(".zip", $"_{progressive++}.zip");
            }
            return fullPath;
        }

        public static string? CreateZipStream(string Filename, string[] Paths)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (var path in Paths)
                    {
                        ZipArchiveEntry entry = zip.CreateEntry(Path.GetFileName(path), CompressionLevel.Optimal);
                        using (var entryStream = entry.Open())
                        {
                            using (MemoryStream bs = new MemoryStream(File.ReadAllBytes(path)))
                            {
                                bs.CopyTo(entryStream);
                            }
                        }
                    }
                }
                File.WriteAllBytes(Filename, ms.ToArray());
                return null;
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }
        #endregion

        #region Utils
        public static string FilesizeGetText(long Size)
        {
            if (Size < 1000000)
                return Math.Round(Size / 1024m, 2).ToString() + " KiB";
            else if (Size < 1000000000)
                return Math.Round(Size / (1024m * 1024m), 2).ToString() + " MiB";
            else
                return Math.Round(Size / (1024m * 1024m * 1024m), 2).ToString() + " GiB";
        }
        #endregion

        #region Private methods
        private static bool CheckFileInUse(string FullPath)
        {
            try
            {
                using (Stream stream = new FileStream(FullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }
        #endregion
    }
}
