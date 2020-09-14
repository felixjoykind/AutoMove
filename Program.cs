using System;
using System.IO;
using System.Runtime.InteropServices;

namespace AutoMove
{
    class Program
    {
        // all paths
        static string folderPath = @"D:\sort shit";
        static string defaultDestinationPath = @"D:\";
        static string imagesDestinationPath = @"D:\images";
        static string iconsDestinationPath = @"D:\icons";
        static string appsDestinationPath = @"D:\apps";
        static string textsDestinationPath = @"D:\texts";

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);

            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = folderPath;

                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Filter = "*.*";
                watcher.Changed += OnChanged;
                watcher.EnableRaisingEvents = true;

                Console.WriteLine("Press 'q' to quit the sample.");
                while (Console.Read() != 'q') ;
            }
        }

        static void OnChanged(object source, FileSystemEventArgs e)
        {
            string fileName = Path.GetFileNameWithoutExtension(e.FullPath);
            string fileExtension = Path.GetExtension(e.FullPath);
            string destination;

            // setting destination path
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

            // choosing correct file name
            int i = 0;
            if (File.Exists($@"{destination}\{e.Name}"))
            {
                while (File.Exists($@"{destination}\{fileName} ({i}){fileExtension}"))
                    i++;
                fileName += $" ({i}){fileExtension}";
            }
            else
                fileName += fileExtension;

            File.Move(e.FullPath, $@"{destination}\{fileName}");
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
            // returning FileType based on file extension
            string extension = Path.GetExtension(e.FullPath);
            if (extension == ".png" || extension == ".jpg" || extension == ".HEIC"
                || extension == ".jfif" || extension == ".mp4")
                return FileType.Image;
            else if (extension == ".txt" || extension == ".docx" || extension == ".xlsx")
                return FileType.Text;
            else if (extension == ".exe")
                return FileType.App;
            else if (extension == ".ico")
                return FileType.Icon;
            else
                return FileType.Other;
        }
    }
}
