# SevenZipLister

A .NET 8 console application that lists all files stored in a 7z archive, displaying their full paths, filenames, and sizes.

## Overview

This tool allows you to quickly inspect the contents of 7z archives without extracting them. It shows:
- Full path of each file within the archive
- Filename (basename only)
- File size with automatic formatting (bytes/KB/MB/GB)

## Requirements

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Windows, Linux, or macOS

## Installation

1. Clone or download this repository
2. Open a terminal in the `src` directory:
   ```powershell
   cd LibRuWorkspace\SevenZipLister\src
   ```
3. Restore dependencies and build:
   ```powershell
   dotnet restore
   dotnet build
   ```

## Usage

Run the application with a path to a `.7z` file as an argument:

```powershell
dotnet run <path-to-7z-file>
```

### File Size Display

The application displays **compressed size** (the actual storage space used in the archive). This is because SharpCompress's `IArchiveEntry.CompressedSize` property provides reliable size information for all 7z entries.

If you need to see uncompressed file sizes, please create an issue or enhance the project with additional libraries that expose uncompressed metadata.

### Examples

```powershell
# List files in archive (relative path)
dotnet run .\backup.7z

# List files in archive (absolute path - Windows)
dotnet run C:\Users\Documents\archive.7z

# List files in archive (Unix/Linux/macOS)
dotnet run /home/user/docs/archive.7z
```

### Output Example

```text
Full Path                              Filename         Size
───────────────────────────────────────────────────────────────
/documents/readme.txt                  readme.txt       2048 bytes
/config/settings.json                  settings.json    1536 bytes
/data/logs/app.log                     app.log          5242.75 KB
```

**Note**: Sizes shown are the **compressed sizes** (storage space used in the archive). This is standard behavior when listing archive contents with SharpCompress library.

## Error Handling

The application handles the following errors gracefully:
- Missing file: Displays "File not found" message
- Invalid archive format: Shows error about corrupt or invalid format
- Empty archive: Indicates no files were found

You can enable debug output by setting the `DEBUG` environment variable to `1`.

## Dependencies

- **SharpCompress** (v0.39.0): Pure C# library for reading 7z archives without native dependencies

## Project Structure

```
SevenZipLister/
├── src/
│   ├── Program.cs                # Main application entry point
│   └── SevenZipLister.csproj     # .NET project file
├── docs/
│   └── specification.md          # Detailed technical specification
└── README.md                     # This file
```

## Testing

To test the application:
1. Create or obtain a sample `.7z` archive
2. Run the application with the archive path as an argument
3. Verify that all files are listed correctly with proper formatting

## Future Enhancements (Optional)

- Add support for filtering by file extension or path pattern
- Include file creation/modification timestamps
- Export results to CSV or JSON format
- Add progress indicators for large archives
- Support for other archive formats (zip, tar, etc.)

## License

This project is provided as-is for educational and practical purposes.

---

**Author**: SevenZipLister Team  
**Version**: 1.0.0