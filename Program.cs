using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace AutoMove
{
    class Program
    {
        // All paths
        static string folderPath = @"D:\sort shit";
        static string defaultDestinationPath = @"D:\";
        static string imagesDestinationPath = @"D:\images";
        static string iconsDestinationPath = @"D:\icons";
        static string appsDestinationPath = @"D:\Apps";
        static string textsDestinationPath = @"D:\Texts";

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();

            // Hide / Show
            ShowWindow(handle, SW_SHOW);

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = folderPath;

            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            watcher.Changed += OnChanged;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press 'q' to quit the sample.");
            while (Console.Read() != 'q') ;
        }

        static void MoveDirectory(string path, string destination, string directoryName)
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories));

            // Picking correct directory name
            int i = 0;
            if (Directory.Exists($@"{destination}\{directoryName}"))
            {
                while (Directory.Exists($@"{destination}\{directoryName} ({i})"))
                    i++;
                directoryName += $" ({i})";
            }

            foreach (var file in files)
                Console.WriteLine(file);

            // Moving directory
            DirectoryInfo mDir = new DirectoryInfo(path);
            mDir.MoveTo(destination + "\\" + directoryName);
            Console.WriteLine("Directory: " + path + " moved to: " + destination + "\\" + directoryName);
        }
        
        static void MoveFile(string path, string destination, string fileName)
        {
            string fileExtension = Path.GetExtension(path);

            // Picking correct file name
            int i = 0;
            if (File.Exists($@"{destination}\{fileName}{fileExtension}"))
            {
                while (File.Exists($@"{destination}\{fileName} ({i}){fileExtension}"))
                    i++;
                fileName += $" ({i}){fileExtension}";
            }
            else
                fileName += fileExtension;

            // Moving file
            FileInfo mFile = new FileInfo(path);
            mFile.MoveTo(destination + "\\" + fileName);
            Console.WriteLine("File: " + path + " moved to: " + destination + "\\" + fileName);
        }

        static void OnChanged(object source, FileSystemEventArgs e)
        {
            string fileName = Path.GetFileNameWithoutExtension(e.FullPath);
            string destination;

            // Setting destination path
            switch (e.GetFileType())
            {
                case FileType.Image:
                    destination = imagesDestinationPath;
                    break;
                case FileType.Text:
                    destination = textsDestinationPath;
                    break;
                case FileType.App:
                    destination = appsDestinationPath;
                    break;
                case FileType.Other:
                    destination = defaultDestinationPath;
                    break;
                case FileType.Icon:
                    destination = iconsDestinationPath;
                    break;
                default:
                    destination = defaultDestinationPath;
                    break;
            }

            string path = e.FullPath;
            // If directory was added
            if (Directory.Exists(path))
                MoveDirectory(path, destination, fileName);
            // If file was added
            else if (File.Exists(path))
                MoveFile(path, destination, fileName);
        }
    }

    enum FileType
    {
        Image,
        Icon,
        Text,
        App,
        Other
    }

    static class Helper
    {
        public static FileType GetFileType(this FileSystemEventArgs e)
        {
            // Returning FileType based on file extension
            string extension = Path.GetExtension(e.FullPath);
            if (extension == ".png" || extension == ".jpg" || extension == ".HEIC"
                || extension == ".jfif" || extension == ".mp4")
                return FileType.Image;
            else if (extension == ".txt" || extension == ".docx" || extension == ".xlsx")
                return FileType.Text;
            else if (extension == ".exe" || extension == ".jar")
                return FileType.App;
            else if (extension == ".ico")
                return FileType.Icon;
            else
                return FileType.Other;
        }
    }
}
