using NipClip.Classes.Clipboard;
using NipClip.Classes.Keyboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public List<MainWindow> mainWindows { get; set; }

        public KeyboardHook.VKeys operation { get; set; }

        public bool ignoreOpration = false;

        public bool nonNative { get; set; }

        public List<KeyboardHook.VKeys> ignoreKeys { get; set; }

        public bool firstEvent = true;

        public TransparentWindow(ref List<MainWindow> mainWindows)
        {
            this.ShowActivated = false;
            this.mainWindows = mainWindows;
            SizeChanged += (o, e) =>
            {
                var r = SystemParameters.WorkArea;
                Left = r.Right - ActualWidth;
                Top = r.Bottom - ActualHeight;
            };
            InitializeComponent();

            if (WindowManager.applicationSettings.leaderKey == KeyboardHook.VKeys.LCONTROL
                || WindowManager.applicationSettings.leaderKey == KeyboardHook.VKeys.RCONTROL
                || WindowManager.applicationSettings.leaderKey == KeyboardHook.VKeys.CONTROL
                )
            {
                this.nonNative = false;
            }
            else
            {
                this.nonNative = true;
            }

            this.ignoreKeys = new List<KeyboardHook.VKeys>();
        }

        public void SupressionTry()
        {
            if (this.nonNative)
            {
                WindowManager.keyboardReader.supressNonNativeLeaderUp = true;
                ClipboardUtility.sendKeyUp(WindowManager.applicationSettings.leaderKey);
                ClipboardUtility.sendKeyDown(KeyboardHook.VKeys.LCONTROL);
            }
        }

        public void DesupressionTry()
        {
            if (this.nonNative)
            {
                ClipboardUtility.sendKeyUp(KeyboardHook.VKeys.LCONTROL);
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

        public void Refresh()
        {
            this.RefreshClipboards();

            this.label.Content = "Copy: [" + WindowManager.applicationSettings.copyKey.ToString() + "]" + Environment.NewLine +
                "Paste: [" + WindowManager.applicationSettings.pasteKey.ToString() + "]";

            this.operation = KeyboardHook.VKeys.NONE;
        }

        private void initCopyOperation()
        {
            this.label.Content = "[Copy Operation]";
            this.ignoreKeys.Add(WindowManager.applicationSettings.copyKey);
            if (WindowManager.applicationSettings.copyKey != KeyboardHook.VKeys.KEY_C)
            {
                this.ignoreKeys.Add(KeyboardHook.VKeys.KEY_C);
            }
            if (this.nonNative)
            {
                this.ignoreKeys.Add(KeyboardHook.VKeys.CONTROL);
                this.ignoreKeys.Add(KeyboardHook.VKeys.LCONTROL);
                this.ignoreKeys.Add(KeyboardHook.VKeys.RCONTROL);

                Console.WriteLine("Trying");
                this.ignoreOpration = true;
                ClipboardUtility.sendKeyDown(KeyboardHook.VKeys.CONTROL);
                ClipboardUtility.sendKeyDown(KeyboardHook.VKeys.KEY_C);
                ClipboardUtility.sendKeyUp(KeyboardHook.VKeys.KEY_C);
                ClipboardUtility.sendKeyUp(KeyboardHook.VKeys.CONTROL);
            }

        }

        bool isDefaultCopy = true;
        private bool copyOperationKeyDown(KeyboardHook.VKeys key)
        {
            this.isDefaultCopy = false;

            return false;
        }

        private bool finishCopyOperation()
        {
            if (this.isDefaultCopy)
            {
                if (!nonNative)
                {
                    //ClipboardUtility.sendKeyDown(KeyboardHook.VKeys.KEY_C);
                }

            }
            Console.WriteLine("default: " + this.isDefaultCopy);

            this.operation = KeyboardHook.VKeys.NONE;
            this.ignoreKeys.Clear();

            //WindowManager.keyboardReader.supressNonNativeLeaderUp = false;
            return false;
        }

        private void initPasteOperation()
        {
            this.label.Content = "[Paste Operation]";
        }

        public bool keyDown(KeyboardHook.VKeys key)
        {
            if (key == WindowManager.applicationSettings.leaderKey)
            {
                return false;
            }

            if (this.operation != KeyboardHook.VKeys.NONE)
            {
                if (key != this.operation)
                {
                    if (this.operation == WindowManager.applicationSettings.copyKey)
                    {
                        return this.copyOperationKeyDown(key);
                    }
                }
            }

            if (this.ignoreKeys.Contains(key))
            {
                return true;
            }

            this.RefreshClipboards();

            if (this.operation == KeyboardHook.VKeys.NONE && key == WindowManager.applicationSettings.copyKey)
            {
                this.operation = key;
                this.initCopyOperation();
                if (!this.nonNative)
                {
                    return true;
                }
            }

            if (this.operation == KeyboardHook.VKeys.NONE && key == WindowManager.applicationSettings.pasteKey)
            {
                this.operation = key;
                this.initPasteOperation();
            }

            return false;
        }

        public bool keyUp(KeyboardHook.VKeys key)
        {
            if (key == WindowManager.applicationSettings.leaderKey) {
                return true;
            }

            if (this.operation != KeyboardHook.VKeys.NONE)
            {
                if (key == WindowManager.applicationSettings.copyKey)
                {
                    return this.finishCopyOperation();
                }
            }

            return false;
        }
    }
}
