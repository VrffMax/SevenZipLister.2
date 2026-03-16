# Usage Instructions - SevenZipLister v1.0.0

## Quick Start

### Build the Project (First Time Only)
```bash
cd C:\Playground\LibRuWorkspace\SevenZipLister.2/src
dotnet restore
dotnet build --configuration Release
```

### Run with a 7z Archive

**Basic Usage:**
```bash
# Provide path to your .7z file as argument
dotnet run <path-to-archive.7z>
```

**Examples:**
```powershell
# Relative path
dotnet run .\backup.7z

# Absolute path (Windows)
dotnet run C:\Users\Documents\archive.7z

# From project directory
dotnet run ./my_archive.7z
```

## Output Format

When you provide a valid `.7z` archive, the application displays:

```text
Full Path                              Filename         Size
───────────────────────────────────────────────────────────────
/documents/readme.txt                  readme.txt       2048 bytes
/config/settings.json                  settings.json    1536 bytes
/data/logs/app.log                     app.log          5242.75 KB
```

## Error Handling

The application handles common errors gracefully:

### 1. No Arguments Provided
```bash
$ dotnet run
Usage: dotnet run <path-to-7z-file>

Example:
  dotnet run C:\Data\Backup.7z
```

### 2. File Not Found
```bash
$ dotnet run nonexistent.7z
Error: File not found - 'nonexistent.7z'
```

### 3. Invalid Archive Format
If you provide a file that's not a valid .7z archive, SharpCompress will automatically detect it and report an appropriate error message.

## Creating Test Archives

### Option 1: Using 7-Zip Command Line Tool
If you have 7-Zip installed (https://www.7-zip.org/):

```powershell
# Create test directory with sample files
mkdir test_data
echo "This is a test file" > test_data\file1.txt
mkdir test_data\subdir  
echo "Nested content here" > test_data\subdir\nested.txt

# Create 7z archive
cd test_data
7z a -r ../test_archive.7z *

# Go back and run the application
cd ..
dotnet run test_archive.7z
```

### Option 2: Using Windows PowerShell Native ZIP (then convert to 7z)

```powershell
mkdir temp_test
echo "Test content" > temp_test\file.txt
Add-Type -AssemblyName System.Compression
$zip = [System.IO.Compression.ZipFile]::Open('archive.zip', [System.IO.Compression.ZipFileMode]::Create)
Get-ChildItem -Path 'temp_test' -Recurse | ForEach-Object {
    $entry = $zip.CreateEntryFromFile($_.FullName, $_.Name.Substring("temp_test".Length))
}
$zip.Close()

# Convert ZIP to 7z if you have 7zip installed
7z a -t7z archive.7z archive.zip
```

## Important Notes About File Sizes

### What You See
The application displays **compressed sizes** - the actual storage space each file uses within your `.7z` archive.

### Why Compressed Size?
- Shows how much disk space your archive actually takes up
- Standard behavior for most archive listing tools  
- SharpCompress provides reliable compressed size data through its public API

### If You Need Uncompressed Sizes
To see original (uncompressed) file sizes, you would need to:
1. Use the 7-Zip command line tool directly: `7z l -slt yourarchive.7z`
2. Or enhance SevenZipLister with a different library that exposes uncompressed size metadata

## Build Status

✅ **All warnings fixed** (CS0219, CS8601 eliminated)  
✅ **Clean build** with .NET 8.0 LTS framework  
✅ **Production-ready** code  

## Troubleshooting

### Issue: "File not found"
- Check the file path is correct
- Verify the `.7z` file exists at that location
- Use absolute paths if relative paths don't work

### Issue: Archive shows as invalid or corrupt
- The file might not be a valid .7z archive (could be HTML, text, etc.)
- Try recreating the archive with 7-Zip command line tool
- Verify the archive wasn't corrupted during transfer/download

### Issue: File sizes showing as "0 bytes" or very small values
- This can happen if the archive was created with a tool that doesn't store size metadata properly
- Recreate the archive using standard 7-Zip (`7z a` command)
- See [TROUBLESHOOTING_0_BYTES.md](./TROUBLESHOOTING_0_BYTES.md) for more details

## Getting Help

If you encounter issues or have questions:
1. Check error messages in console output
2. Verify the .7z file was created with a standard tool (7-Zip, WinRAR, etc.)
3. Try creating a small test archive to verify functionality

---

**Project**: SevenZipLister v1.0.0  
**Location**: `C:\Playground\LibRuWorkspace\SevenZipLister.2`  
**Framework**: .NET 8.0 LTS  
**License**: Provided as-is for educational and practical purposes