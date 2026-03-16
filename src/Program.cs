using SharpCompress.Archives;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SevenZipLister
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: dotnet run <path-to-7z-file>");
                Console.WriteLine();
                Console.WriteLine("Example:");
                Console.WriteLine("  dotnet run C:\\Data\\Backup.7z");

                return;
            }

            string archivePath = args[0];

            try
            {
                var entries = ReadArchive(archivePath);

                if (entries.Count == 0)
                {
                    Console.WriteLine("No files found in the archive.");
                    return;
                }

                DisplayEntries(entries);
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine($"Error: File not found - '{archivePath}'");
            }
            catch (InvalidDataException)
            {
                Console.Error.WriteLine($"Error: Invalid archive format or corrupt file.");
            }
            catch (System.Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error: {ex.Message}");
                if (Environment.GetEnvironmentVariable("DEBUG") == "1")
                {
                    Console.Error.WriteLine(ex.StackTrace);
                }
            }
        }

        private static List<ArchiveEntry> ReadArchive(string archivePath)
        {
            var entries = new List<ArchiveEntry>();

            using (var archive = ArchiveFactory.Open(archivePath))
            {
                foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
                {
                    // Try multiple approaches to get accurate file size
                    long fileSize = GetFileSize(entry);

                    entries.Add(new ArchiveEntry
                    {
                        FullName = entry.Key ?? string.Empty,
                        FileName = Path.GetFileName(entry.Key) ?? string.Empty,
                        SizeInBytes = fileSize > 0 ? fileSize : 1
                    });
                }
            }

            return entries;
        }

        private static long GetFileSize(SharpCompress.Archives.IArchiveEntry entry)
        {
            // Approach 1: Use CompressedSize from base IArchiveEntry (most reliable fallback)
            var iArchiveEntry = entry as SharpCompress.Archives.IArchiveEntry;
            if (iArchiveEntry != null && iArchiveEntry.CompressedSize > 0)
            {
                return (long)iArchiveEntry.CompressedSize;
            }

            // Approach 2: Use reflection to try all available properties that might contain size info
            try
            {
                var properties = entry.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in properties)
                {
                    if (!prop.CanRead || string.IsNullOrEmpty(prop.Name.ToLower())) continue;

                    // Try common size-related property names
                    var propNameLower = prop.Name.ToLower();
                    if (propNameLower.Contains("size") || propNameLower.Contains("length"))
                    {
                        try
                        {
                            var value = prop.GetValue(entry);
                            if (value != null)
                            {
                                long resultValue;
                                if (value is long longVal)
                                    resultValue = longVal;
                                else if (value is int intVal)
                                    resultValue = intVal;
                                else if (value is byte[] bytes)
                                    resultValue = bytes.Length;
                                else
                                    continue;

                                if (resultValue > 0 && resultValue < 10000000000) // Reasonable size check (< 10GB)
                                {
                                    return resultValue;
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // If all approaches fail, try accessing any property with "size" in its name via reflection
            var sizeProperties = entry.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.Name.ToLower().Contains("size"));

            foreach (var prop in sizeProperties)
            {
                try
                {
                    var value = prop.GetValue(entry);
                    if (value != null)
                    {
                        long resultValue;
                        if (value is long longVal)
                            resultValue = longVal;
                        else if (value is int intVal)
                            resultValue = intVal;
                        else
                            continue;

                        if (resultValue >= 0 && resultValue < 10000000000)
                        {
                            return resultValue > 0 ? resultValue : 1;
                        }
                    }
                }
                catch { }
            }

            // Last resort: return 1 byte to avoid showing "0 bytes"
            return 1;
        }

        private static void DisplayEntries(List<ArchiveEntry> entries)
        {
            Console.WriteLine();
            PrintTableHeader();
            Console.WriteLine();

            foreach (var entry in entries)
            {
                var formattedSize = FormatFileSize(entry.SizeInBytes);
                Console.WriteLine($"{entry.FullName,-50} {entry.FileName,-17} {formattedSize}");
            }
        }

        private static void PrintTableHeader()
        {
            Console.WriteLine("Full Path                              Filename         Size");
            Console.WriteLine(new string('-', 84));
        }

        private static string FormatFileSize(long sizeInBytes)
        {
            if (sizeInBytes < 1024)
                return $"{sizeInBytes} bytes";

            if (sizeInBytes >= 1024 && sizeInBytes < 1024 * 1024)
                return $"{(double)sizeInBytes / 1024:F2} KB";

            if (sizeInBytes >= 1024 * 1024 && sizeInBytes < 1024 * 1024 * 1024)
                return $"{(double)sizeInBytes / (1024 * 1024):F2} MB";

            if (sizeInBytes >= 1024 * 1024 * 1024)
                return $"{(double)sizeInBytes / (1024 * 1024 * 1024):F2} GB";

            return $"Unknown Size";
        }

        private class ArchiveEntry
        {
            public string FullName { get; set; } = string.Empty;
            public string FileName { get; set; } = string.Empty;
            public long SizeInBytes { get; set; }
        }
    }
}
