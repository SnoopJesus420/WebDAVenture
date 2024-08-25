# WebDAVenture
C# tool for mounting and executing a file on a remote WebDAV share.

# Instructions
Edit the following variables below and then compile :)

```C#
tring driveLetter = "drive letter as (Z:)";
string webdavUrl = "https://your-webdav-share";
string fileToExecute = Path.Combine(driveLetter, "change-me.bat"); // Adjust the file name and extension
string username = "example"; // Optional - set this to null if no username is needed
string password = "example"; // Optional - set this to null if no pass is needed
```
