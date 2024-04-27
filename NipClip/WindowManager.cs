using NipClip.Classes;
using NipClip.Classes.Keyboard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NipClip
{
    class WindowManager
    {
        public static List<MainWindow> clipboardMainWindows = new List<MainWindow>();

        public static KeyboardReader keyboardReader;
        public static ApplicationSettings applicationSettings { get; set; }


        public static void RefreshAll()
        {
            foreach (var window in clipboardMainWindows)
            {
                window.Refresh();
            }
        }

        public static void RencryptAll(string newKey)
        {
            foreach (MainWindow window in clipboardMainWindows)
            {
                foreach (var entry in window.clipboardReader.clipboardStorage.entries)
                {
                    entry.Reencrypt(newKey);
                }
            }

            RefreshAll();
        }

        public static void ChangeKeyboardLeader(KeyboardHook.VKeys key)
        {
            WindowManager.applicationSettings.leaderKey = key;
            WindowManager.applicationSettings.save();
            WindowManager.RefreshAll();

            keyboardReader = null;
            keyboardReader = new KeyboardReader(ref clipboardMainWindows);
        }
        public static void ChangeKeyboardLeader()
        {
            keyboardReader.fillNextInputToLeaderKey();
        }

        public static void CloseAll()
        {
            foreach (MainWindow window in clipboardMainWindows)
            {
                window.clipboardReader.export();
            }
            applicationSettings.save();
            Environment.Exit(0);
        }

        public static void KillAll()
        {
            foreach (MainWindow window in clipboardMainWindows)
            {
                window.clipboardReader.purgeData();
            }
            Environment.Exit(0);
        }

        public static void SaveStringTemplate(string template)
        {
            applicationSettings.saveStringTemplate(template);

            foreach (MainWindow window in clipboardMainWindows)
            {
                window.stringManipTemplateSelection.Items.Refresh();
            }
            applicationSettings.save();
        }

        public static void DeleteStringTemplate(string template)
        {
            applicationSettings.deleteStringTemplate(template);

            foreach (MainWindow window in clipboardMainWindows)
            {
                window.stringManipTemplateSelection.Items.Refresh();
                window.stringManipTemplateSelection.Text = string.Empty;
                window.stringManipTemplateSelection.SelectedItem = string.Empty;
                window.stringManipInput.Text = string.Empty;   
            }
            applicationSettings.save();
        }

        static SearchUtility searchUtility = null;

        public static void OpenSearchUtility()
        {
            if (searchUtility == null)
            {
                searchUtility = new SearchUtility(ref clipboardMainWindows);
                searchUtility.Show();
            } 
            else
            {
                searchUtility.Show();
            }
        }

        public static void CreateNewClipboardMainWindow()
        {
            if (clipboardMainWindows.Count <= 0)
            {
                WindowManager.applicationSettings = new ApplicationSettings();
                WindowManager.applicationSettings.restore();

                WindowManager.keyboardReader = new KeyboardReader(ref WindowManager.clipboardMainWindows);

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                WindowManager.clipboardMainWindows.Add(mainWindow);
                Thread.Sleep(100);

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
                            Thread.Sleep(100);
                        }
                    }
                }
                
                //TransparentWindow transparentWindow = new TransparentWindow();
                //transparentWindow.Show();
            }
            else
            {
                int biggestIndex = 0;
                int smallestIndex = int.MaxValue;

                foreach (MainWindow window in clipboardMainWindows)
                {
                    if (window.clipboardID != 0)
                    {
                        if (biggestIndex < window.clipboardID)
                        {
                            biggestIndex = window.clipboardID;
                        }

                        if (smallestIndex > window.clipboardID)
                        {
                            smallestIndex = window.clipboardID;
                        }
                    }
                }

                int newIndex = 0;

                if (applicationSettings.ClipboardIDReverse)
                {
                    for (int i = 9; i >= 1; i--)
                    {
                        if (!clipboardMainWindows.Any(window => window.clipboardID == i))
                        {
                            newIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= 9; i++)
                    {
                        if (!clipboardMainWindows.Any(window => window.clipboardID == i))
                        {
                            newIndex = i;
                            break;
                        }
                    }
                }
                if (newIndex > 0)
                {
                    MainWindow mainWindow = new MainWindow(newIndex);
                    mainWindow.Show();
                    WindowManager.clipboardMainWindows.Add(mainWindow);
                }
            }
        }
    }
}
