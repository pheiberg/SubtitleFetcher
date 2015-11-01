using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SharpCompress.Common;
using SharpCompress.Reader;

namespace PortedSubtitleDownloaders.Legacy
{
    public class FileUtils
    {
        private static readonly string TempFile = "subtitledownloader_temp";

        public static string AssemblyDirectory => Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

        public static string GetTempFileName()
        {
            int num = new Random().Next(0, 10);
            return Path.Combine(Path.GetTempPath(), TempFile + num);
        }

        public static string GetFileNameForSubtitle(string subtitleFile, string languageCode, string videoFile)
        {
            string path = videoFile;
            string withoutExtension = Path.GetFileNameWithoutExtension(path);
            string directoryName = Path.GetDirectoryName(path);
            string extension = Path.GetExtension(subtitleFile);
            string languageName = Languages.GetLanguageName(languageCode);
            return $"{directoryName}{Path.DirectorySeparatorChar}{withoutExtension}.{languageName}{extension}";
        }

        public static List<FileInfo> ExtractFilesFromZipOrRarFile(string archiveFile)
        {
            string str = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(str);
            using (Stream stream = File.OpenRead(archiveFile))
            {
                IReader reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                        reader.WriteEntryToDirectory(str, ExtractOptions.Overwrite | ExtractOptions.ExtractFullPath);
                }
            }
            return Directory.GetFiles(str, "*", SearchOption.AllDirectories).Select(fileName => new FileInfo(fileName)).ToList();
        }

        public static void WriteNewFile(string fileNameWithPath, byte[] fileData)
        {
            using (FileStream fileStream = new FileStream(fileNameWithPath, FileMode.CreateNew))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                    binaryWriter.Write(fileData);
            }
        }
    }
}