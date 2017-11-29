using System;
using System.Collections.Generic;
using System.IO;

namespace LibraryCatalog
{
    class Program
    {
        //log file to store scan results.
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();

        public static string Path()// { get; private set; }
        {
            string defineOutputPath = @"c:\LibraryCatalog.txt";
            return defineOutputPath;
            //A feature I would like to add later is to include the currentDate in the logfile name.
        }
        //GetLogicalDrives method will obtain names of all logical drives on the computer and display it.
        //https://msdn.microsoft.com/en-us/library/system.io.directory.getlogicaldrives(v=vs.110).aspx
        public static void getLogicalDrives()
        {
            string[] drives = System.IO.Directory.GetLogicalDrives();
            try
            {
                foreach (string str in drives)
                {
                    System.Console.WriteLine(str);
                }
            }
            catch (System.IO.IOException)
            {
                System.Console.WriteLine("An I/O error occurs.");
            }
            catch (System.Security.SecurityException)
            {
                System.Console.WriteLine("The caller does not have the required permission.");
            }

        }

        public static void WalkDirectoryTree(System.IO.DirectoryInfo root)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;
            string outputPath = Path();
            if (!File.Exists(outputPath))
            {
                //Create a file to write
                String createText = "Library Catalog Program scan results for" + root.FullName + Environment.NewLine;
                File.WriteAllText(outputPath, createText);
            }

            //Add text to file
            string appendText = root.FullName + " ----------------------------------------------------------------"
                + Environment.NewLine;
            File.AppendAllText(outputPath, appendText);

            //First process all the files directly under this folder.
            try
            {
                files = root.GetFiles("*.*");

            }
            catch (UnauthorizedAccessException e)
            {
                log.Add(e.Message);
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    //Console.WriteLine(fi.FullName + " " + " " + fi.Length + " " + fi.Attributes );
                    File.AppendAllText(outputPath, fi.FullName + " " + " " + BytesToString(fi.Length) + " " + fi.Attributes + Environment.NewLine);
                }
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    //Recursive call for each directory.
                    WalkDirectoryTree(dirInfo);
                }
            }
            

        }
        
        static String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        static void Main(string[] args)
        {
            //Parsing delimiter  to split drive letter that users entered
            //https://msdn.microsoft.com/en-us/library/ms228388.aspx
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string driveEntered;
            //string scanType;
            string trail = @":\";

            Console.WriteLine("This Application will scan your computer drives and catalog all files to a textfile. Please select the drive you would like to scan:");
            
            // Find all drives on computer and display it for user selection
            string[] drives = System.IO.Directory.GetLogicalDrives();
            foreach (string str in drives)
            {
                System.Console.WriteLine(str+"\n");
            }
            
            Console.WriteLine("Enter the drive letter separated by a ',' ");
            driveEntered = Console.ReadLine();
            String[] driveToScan = driveEntered.Split(delimiterChars);
            Console.WriteLine("The following drives will be scanned: " + String.Join(", ", driveToScan));

            for(int i = 0; i < driveToScan.Length; i++)
            {
                try {

                    String drive = driveToScan[i] + trail;
                    //Console.WriteLine("driveToScan is  " + driveToScan[i]);
                    //Console.WriteLine("Drive is  " + drive);
                    DirectoryInfo dir = new DirectoryInfo(drive); //@"C:\");//driveToScan[i]);
                    Console.WriteLine("Scanning drive " + dir);
                    WalkDirectoryTree(dir);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught " + ex);
                }
                    
            }




            //Display C:\ Drive info
            /*
            System.IO.DriveInfo di = new System.IO.DriveInfo(@"C:\");
            Console.WriteLine(di.AvailableFreeSpace);
            Console.WriteLine(di + ": " + di.VolumeLabel);


            //Get Root directory and print information about it.
            System.IO.DirectoryInfo dirInfo = di.RootDirectory;
            System.IO.FileInfo[] fileNames = dirInfo.GetFiles("*.*");

            foreach (System.IO.FileInfo fi in fileNames)
            {
                Console.WriteLine("{0}: {1}: {2}:", fi.Name, fi.LastAccessTime, fi.Length);
            }

            */
            /*
            //RecurvsiveFileSearch test
            string[] drives = System.Environment.GetLogicalDrives();

            foreach (string dr in drives)
            {
                System.IO.DriveInfo dir = new System.IO.DriveInfo(dr);
            

            if (!dir.IsReady)
            {
                Console.WriteLine("The drive {0} could not be read", di.Name);
                continue;
            }
            System.IO.DirectoryInfo rootDir = dir.RootDirectory;
                
                System.IO.DirectoryInfo testDir = new System.IO.DirectoryInfo(args[0]);

                WalkDirectoryTree(testDir);
            }
            Console.WriteLine("Files with resetricted access: ");
            foreach (string s in log)
            {
            Console.WriteLine(s);
            }

           */

            Console.WriteLine("Output file generated at the following location: " + Path());
            //File.Open(outputPath, FileMode.Open);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

        }
    }
}
