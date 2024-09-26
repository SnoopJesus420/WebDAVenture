# WebDAVenture
C# tool for mounting and executing a file on a remote WebDAV share.

# Instructions
Edit the following variables below

```C#
string driveLetter = "drive letter as (Z:)";
string webdavUrl = "https://your-webdav-share";
string fileToExecute = Path.Combine(driveLetter, "change-me.bat"); // Adjust the file name and extension
string username = "example"; // Optional - set this to null if no username is needed
string password = "example"; // Optional - set this to null if no pass is needed
```
Then compile with msbuild
```
msbuild <your-project.sln>
```

# Things to come
1. Remove console window

# Things added
Better error handling
D/Invoke for WinAPI calls
