using NipClip.Classes.Keyboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            this.RefreshClipboards();

            this.label.Content = "Copy: [" + WindowManager.applicationSettings.copyKey.ToString() + "]" + Environment.NewLine +
                "Paste: [" + WindowManager.applicationSettings.pasteKey.ToString() + "]";
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

        public bool keyboardHook_KeyDown(KeyboardHook.VKeys key)
        {

            return false;
        }


        public void StartCopyAction()
        {
            this.activeAction = "copy";
            this.label.Content = "[Copy Operation]";
        }

        public void StartPasteAction()
        {
            this.activeAction = "paste";
            this.label.Content = "paste";
            this.label.Content = "[Paste Operation]";
        }
    }
}
