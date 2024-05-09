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
using System.Windows.Automation.Peers;
using System.Windows.Forms;

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

        public static void ChangeCopyKey(KeyboardHook.VKeys key)
        {
            WindowManager.applicationSettings.copyKey = key;
            WindowManager.applicationSettings.save();
            WindowManager.RefreshAll();

            keyboardReader = null;
            keyboardReader = new KeyboardReader(ref clipboardMainWindows);
        }

        public static void ChangeCopyKey()
        {
            keyboardReader.fillNextInputToCopyKey();
        }

        public static void ChangePasteKey(KeyboardHook.VKeys key)
        {
            WindowManager.applicationSettings.pasteKey = key;
            WindowManager.applicationSettings.save();
            WindowManager.RefreshAll();

            keyboardReader = null;
            keyboardReader = new KeyboardReader(ref clipboardMainWindows);
        }

        public static void ChangePasteKey()
        {
            keyboardReader.fillNextInputToPasteKey();
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

        public static void Close(Object sender, System.EventArgs e)
        {
            foreach (MainWindow window in clipboardMainWindows)
            {
                window.clipboardReader.export();
            }
            applicationSettings.save();
            if (sender != null)
            {
                keyboardReader.hook.Uninstall();
                Environment.Exit(0);
            }
            else
            {
                foreach (MainWindow window in clipboardMainWindows)
                {
                    window.Close();
                }
            }
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

        public static NotifyIcon NotifyIcon { get; set; }

        public static void InitNotifyIcon()
        {
            NotifyIcon = new NotifyIcon();
            NotifyIcon.Icon = new System.Drawing.Icon("logo-black.ico");
            NotifyIcon.Visible = true;
            NotifyIcon.Text = "NipClip";

            NotifyIcon.ContextMenuStrip = new ContextMenuStrip();
            NotifyIcon.ContextMenuStrip.Items.Add("Close", null, Close);
        }

        public static void ToggleWindow(Object sender, System.EventArgs e)
        {
            int index = GetIndexFromInput(sender.ToString());

            if (index < 0)
                return;

            foreach (var window in clipboardMainWindows)
            {
                if (window.clipboardID == index)
                {
                    if (window.Visibility == Visibility.Visible)
                    {
                        window.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        window.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        public static int GetIndexFromInput(string input)
        {
            string[] parts = input.Split('|');

            if (parts.Length > 1)
            {
                string lastPart = parts[1].Trim();
                if (lastPart.Equals("Windows Default", StringComparison.OrdinalIgnoreCase))
                {
                    return 0;
                }
                else if (int.TryParse(lastPart, out int index))
                {
                    return index;
                }
            }

            return -1;
        }

        public static void AddMainWindowToIcon(ref MainWindow window)
        {
            if (window.clipboardID == 0)
            {
                NotifyIcon.ContextMenuStrip.Items.Add("Clipboard | Windows Default", null, ToggleWindow);
            }
            else
            {
                NotifyIcon.ContextMenuStrip.Items.Add("Clipboard | " + window.clipboardID, null, ToggleWindow);
            }
        }

        public static TransparentWindow transparentWindow { get; set; }
        public static void OpenTransparentWindow()
        {
            if (transparentWindow == null)
            {
                transparentWindow = new TransparentWindow(ref clipboardMainWindows);
                transparentWindow.Show();
                transparentWindow.active = true;
            }
            else
            {
                transparentWindow.Show();
                transparentWindow.active = true;
            }
        }

        public static void CloseTransparentWindow()
        {
            if (transparentWindow != null)
            {
                transparentWindow.Hide();
                transparentWindow.Reset();
                transparentWindow.active = false;
                transparentWindow.activeAction = string.Empty;
            }
        }

        public static void CreateNewClipboardMainWindow(int index = 0)
        {
            if (index != 0)
            {
                if (index < 10)
                {
                    MainWindow mainWindow1 = new MainWindow(index);
                    mainWindow1.Show();
                    AddMainWindowToIcon(ref mainWindow1);
                    WindowManager.clipboardMainWindows.Add(mainWindow1);
                }
            }
            else
            {
                if (clipboardMainWindows.Count <= 0)
                {
                    if (File.Exists("settings.xml"))
                    {
                        WindowManager.applicationSettings = new ApplicationSettings();
                        WindowManager.applicationSettings.restore();
                    }
                    else
                    {
                        WindowManager.applicationSettings = new ApplicationSettings();
                        WindowManager.applicationSettings.save();
                        WindowManager.applicationSettings.restore();
                    }

                    WindowManager.keyboardReader = new KeyboardReader(ref WindowManager.clipboardMainWindows);

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    WindowManager.clipboardMainWindows.Add(mainWindow);
                    Thread.Sleep(100);

                    if (NotifyIcon == null)
                    {
                        InitNotifyIcon();
                        AddMainWindowToIcon(ref mainWindow);
                    }

                    DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());
                    FileInfo[] Files = d.GetFiles("entries*.xml");
                    if (applicationSettings.ClipboardIDReverse)
                    {
                        Files = Files.Reverse().ToArray();
                    }
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
                                AddMainWindowToIcon(ref mainWindow1);
                                WindowManager.clipboardMainWindows.Add(mainWindow1);
                                Thread.Sleep(100);
                            }
                        }
                    }

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
                        AddMainWindowToIcon(ref mainWindow);
                        WindowManager.clipboardMainWindows.Add(mainWindow);
                    }
                }
            }
        }
    }
}
