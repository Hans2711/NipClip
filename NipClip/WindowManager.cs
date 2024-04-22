using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NipClip
{
    class WindowManager
    {
        public static List<MainWindow> clipboardMainWindows = new List<MainWindow>();
        public static void CreateNewClipboardMainWindow()
        {
            if (clipboardMainWindows.Count <= 0)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                WindowManager.clipboardMainWindows.Add(mainWindow);

                DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());
                FileInfo[] Files = d.GetFiles("entries*.xml");
                foreach (FileInfo file in Files)
                {
                    string fileName = file.Name;
                    if (fileName.StartsWith("entries-") && fileName.EndsWith(".xml"))
                    {
                        string numberPart = fileName.Substring("entries-".Length, fileName.Length - "entries-".Length - ".xml".Length);
                        int number;
                        if (int.TryParse(numberPart, out number))
                        {
                            MainWindow mainWindow1 = new MainWindow(number);
                            mainWindow1.Show();
                            WindowManager.clipboardMainWindows.Add(mainWindow1);
                        }
                    }
                }
            }
            else
            {
                MainWindow mainWindow = new MainWindow(WindowManager.clipboardMainWindows.Last().clipboardID + 1);
                mainWindow.Show();
                WindowManager.clipboardMainWindows.Add(mainWindow);
            }
        }
    }
}
