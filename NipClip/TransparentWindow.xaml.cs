using NipClip.Classes.Clipboard;
using NipClip.Classes.Keyboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NipClip
{
    /// <summary>
    /// Interaction logic for TransparentWindow.xaml
    /// </summary>
    public partial class TransparentWindow : Window
    {
        public bool active = false;

        public string activeAction = string.Empty;

        public List<MainWindow> mainWindows { get; set; }

        public List<int> activeClipboardIDs = new List<int>();

        public bool freshCopy = false;

        public TransparentWindow(ref List<MainWindow> mainWindows)
        {
            this.ShowActivated = false;
            SizeChanged += (o, e) =>
            {
                var r = SystemParameters.WorkArea;
                Left = r.Right - ActualWidth;
                Top = r.Bottom - ActualHeight;
            };
            InitializeComponent();
            this.mainWindows = mainWindows;
        }

        public void Reset()
        {
            this.activeAction = string.Empty;
            this.active = false;

            this.label.Content = "Copy: [" + WindowManager.applicationSettings.copyKey.ToString() + "]" + Environment.NewLine +
                "Paste: [" + WindowManager.applicationSettings.pasteKey.ToString() + "]";
        }

        public void Refresh()
        {
           this.activeClipboardIDs.Clear();
            this.RefreshClipboards();
            this.SetActiveClipboardID(0);

            this.label.Content = "Copy: [" + WindowManager.applicationSettings.copyKey.ToString() + "]" + Environment.NewLine +
                "Paste: [" + WindowManager.applicationSettings.pasteKey.ToString() + "]";
        }

        public void Close()
        {
            if (this.activeAction == "copy")
            {
                if (this.activeClipboardIDs.Count > 0)
                {
                    ClipboardEntry entry = mainWindows[0].clipboardReader.clipboardStorage.entries.First();
                    foreach (var window in mainWindows)
                    {
                        if (this.activeClipboardIDs.Contains(window.clipboardID))
                        {
                            window.clipboardReader.clipboardStorage.entries.Insert(0, entry);
                        }
                    }
                    if (this.freshCopy)
                    {
                        mainWindows[0].clipboardReader.clipboardStorage.entries.Remove(entry);
                        mainWindows[0].clipboardReader.clipboardStorage.entries.First().pasteToClipboard();
                    }
                    WindowManager.RefreshAll();
                }
            }

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.Refresh();
        }

        public void RefreshClipboards()
        {
            foreach (var window in mainWindows)
            {
                bool updated = false;
                foreach (TabItem item in clipboards.Items)
                {
                    if (item.Header.ToString() == window.clipboardID.ToString())
                    {
                        if (item.Content.ToString() != window.clipboardReader.clipboardStorage.entries.First().Content)
                        {
                            Label label0 = new Label();
                            label0.Content = window.clipboardReader.clipboardStorage.entries.First().Content;
                            label0.HorizontalAlignment = HorizontalAlignment.Center;
                            label0.VerticalAlignment = VerticalAlignment.Center;

                            item.Content = label0;
                        }
                        updated = true;
                    }
                }
                if (!updated)
                {
                    TabItem tabItem0 = new TabItem();
                    tabItem0.Header = window.clipboardID;

                    Label label0 = new Label();
                    label0.Content = window.clipboardReader.clipboardStorage.entries.First().Content;
                    label0.HorizontalAlignment = HorizontalAlignment.Center;
                    label0.VerticalAlignment = VerticalAlignment.Center;

                    tabItem0.Content = label0;

                    clipboards.Items.Add(tabItem0);
                }
            }
        }

        public void SetActiveClipboardID(int activeClipboardID)
        {
            this.activeClipboardIDs.Add(activeClipboardID);

            foreach (var window in mainWindows)
            {
                if (window.clipboardID != activeClipboardID)
                {
                    continue;
                }

                foreach (TabItem item in clipboards.Items)
                {
                    if (item.Header.ToString() == window.clipboardID.ToString())
                    {
                        clipboards.SelectedValue = item;
                        if (this.activeAction == "copy")
                        {

                        }
                    }
                }
            }


        }

        public bool keyboardHook_KeyDown(KeyboardHook.VKeys key)
        {
            if (
                key == KeyboardHook.VKeys.KEY_1 || 
                key == KeyboardHook.VKeys.KEY_2 ||
                key == KeyboardHook.VKeys.KEY_3 ||
                key == KeyboardHook.VKeys.KEY_4 ||
                key == KeyboardHook.VKeys.KEY_5 ||
                key == KeyboardHook.VKeys.KEY_6 ||
                key == KeyboardHook.VKeys.KEY_7 ||
                key == KeyboardHook.VKeys.KEY_8 ||
                key == KeyboardHook.VKeys.KEY_9
                )
            {
                int clipboardIndex = 0;
                if (key == KeyboardHook.VKeys.KEY_1)
                {
                    clipboardIndex = 1;
                }
                if (key == KeyboardHook.VKeys.KEY_2)
                {
                    clipboardIndex = 2;
                }
                if (key == KeyboardHook.VKeys.KEY_3)
                {
                    clipboardIndex = 3;
                }
                if (key == KeyboardHook.VKeys.KEY_4)
                {
                    clipboardIndex = 4;
                }
                if (key == KeyboardHook.VKeys.KEY_5)
                {
                    clipboardIndex = 5;
                }
                if (key == KeyboardHook.VKeys.KEY_6)
                {
                    clipboardIndex = 6;
                }
                if (key == KeyboardHook.VKeys.KEY_7)
                {
                    clipboardIndex = 7;
                }
                if (key == KeyboardHook.VKeys.KEY_8)
                {
                    clipboardIndex = 8;
                }
                if (key == KeyboardHook.VKeys.KEY_9)
                {
                    clipboardIndex = 9;
                }
                this.SetActiveClipboardID( clipboardIndex );
            }

            if (key == WindowManager.applicationSettings.copyKey)
            {
                this.Close();
            }

            return false;
        }

        private void WaitandRefresh()
        {
            Thread.Sleep(500);
            this.Dispatcher.Invoke(() => { 
                this.RefreshClipboards();
            });
        }


        public void StartCopyAction()
        {
            this.activeAction = "copy";
            this.label.Content = "[Copy Operation]";
            this.freshCopy = true;

            Thread wait = new Thread(this.WaitandRefresh);
            wait.Start();
        }

        public void StartPasteAction()
        {
            this.activeAction = "paste";
            this.label.Content = "paste";
            this.label.Content = "[Paste Operation]";
        }
    }
}
