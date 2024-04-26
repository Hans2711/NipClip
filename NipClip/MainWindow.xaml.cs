using NipClip.Classes;
using NipClip.Classes.Clipboard;
using NipClip.Classes.Keyboard;
using NipClip.Classes.NipLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Forms = System.Windows.Forms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Printing;

namespace NipClip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ClipboardReader clipboardReader { get; set; }

        public KeyboardReader keyboardReader { get; set; }

        public bool nonNativeClipboard = false;

        public int clipboardID = 0;

        public Forms.NotifyIcon notifyIcon { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.Title = "NipClip | Windows Default";
        }
        
        public MainWindow(int clipboardID)
        {
            InitializeComponent();
            this.nonNativeClipboard = true;
            this.clipboardID = clipboardID;
            this.Title = "NipClip | Custom Clipboard: " + this.clipboardID;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (this.nonNativeClipboard)
            {
                this.clipboardReader = new ClipboardReader(this, "entries-" + this.clipboardID + ".xml");
            }
            else
            {
                this.clipboardReader = new ClipboardReader(this, "entries-default.xml");
            }
            this.textList.ItemsSource = clipboardReader.clipboardStorage.entries;

            this.DataContext = clipboardReader.clipboardStorage.entries;

            this.stringManipTemplateSelection.ItemsSource = WindowManager.applicationSettings.nipLangTemplates;

            ClipboardReader.encryptionKey = WindowManager.applicationSettings.EncryptionKey;

            this.sortingDataGrid.ItemsSource = this.clipboardReader.clipboardStorage.entries;

            this.notifyIcon = new Forms.NotifyIcon();
            this.notifyIcon.Icon = new System.Drawing.Icon("logo-black.ico");
            this.notifyIcon.Visible = true;
            this.notifyIcon.Text = this.Title;

            this.notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            this.notifyIcon.ContextMenuStrip.Items.Add(this.Title, null, null);
            this.notifyIcon.ContextMenuStrip.Items.Add("Copy Last Entry", null, this.copyLastItem);
            this.notifyIcon.ContextMenuStrip.Items.Add("Kill All", null, this.killAll);
            this.notifyIcon.ContextMenuStrip.Items.Add("Kill", null, this.kill);
            this.notifyIcon.ContextMenuStrip.Items.Add("Close All", null, this.closeAll);
            this.notifyIcon.ContextMenuStrip.Items.Add("Close", null, this.close);

            this.notifyIcon.Click +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                };

            this.stringManipInput_GotFocus();
        }

        public void copyLastItem(Object sender, System.EventArgs e)
        {
            this.clipboardReader.clipboardStorage.entries.First().pasteToClipboard();
        }

        public void kill(Object sender, System.EventArgs e)
        {
            this.clipboardReader.purgeData();
            this.Close();
        }

        public void killAll(Object sender, System.EventArgs e)
        {
            WindowManager.KillAll();
        }

        public void close(Object sender, System.EventArgs e)
        {
            this.clipboardReader.export();
            this.Close();
        }

        public void closeAll(Object sender, System.EventArgs e)
        {
            WindowManager.CloseAll();
        }


        private void stringManipTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.stringManipTemplateSelection.SelectedItem != null)
            {
                this.stringManipInput.Text = this.stringManipTemplateSelection.SelectedItem.ToString();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.clipboardReader.export();
            WindowManager.applicationSettings.save();

            e.Cancel = true;
            this.Hide();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool notEmpty = false;
            List<ClipboardEntry> processList = new List<ClipboardEntry>();

            for (int i = 0; i < this.textList.SelectedItems.Count; i++)
            {
                if (this.textList.SelectedItems[i] is ClipboardEntry entry)
                {
                    var split = entry.Split(this.stringManipInnerDelimiter.Text.Replace("\\n", Environment.NewLine));
                    foreach (var entryy in split)
                    {
                        if (entryy.Content == null)
                        {
                            continue;
                        }
                        processList.Add(entryy);
                    }
                }

                notEmpty = true;
            }

            if (notEmpty == false)
            {
                return;
            }

            var languageProcessor = new LanguageProcessor();
            this.clipboardReader.export();
            string output = languageProcessor.Process(processList, this.stringManipInput.Text);

            System.Windows.Clipboard.SetText(output);

            this.stringManipOutput.Text = output;
        }

        private void reencryptButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.reencryptButton.Content = "Reencrypt";

            if (!string.IsNullOrEmpty(this.encryptionKey.Text))
            {
                if (this.reencryptButton.Content != "Done")
                {
                    WindowManager.RencryptAll(this.encryptionKey.Text);
                    WindowManager.applicationSettings.EncryptionKey = this.encryptionKey.Text;
                    ClipboardReader.encryptionKey = WindowManager.applicationSettings.EncryptionKey;

                    this.clipboardReader.export();
                    this.reencryptButton.Content = "Done";
                    this.Refresh();
                } else
                {
                    this.reencryptButton.Content = "No Need to do it Twice";
                }
            } 
            else
            {
                this.reencryptButton.Content = "Key is Empty";
            }
        }

        private void saveStringTemplateClick(object sender, RoutedEventArgs e)
        {
            WindowManager.SaveStringTemplate(this.stringManipInput.Text);
        }

        private void deleteStringTemplateClick(object sender, RoutedEventArgs e)
        {
            WindowManager.DeleteStringTemplate(this.stringManipInput.Text);
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.disableOnDemandDecryption.IsChecked)
            {
                ClipboardReader.encryptionEnabled = false;
            } else
            {
                ClipboardReader.encryptionEnabled = true;
            }
            
            WindowManager.RefreshAll();
        }

        public void Refresh()
        {
            try
            {
                this.sortingDataGrid.Items.Refresh();
                this.textList.Items.Refresh();
            } catch { }
        }

        private void createNewClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.CreateNewClipboardMainWindow();
        }

        private void stringManipInput_GotFocus(object sender = null, RoutedEventArgs e = null)
        {
            if (WindowManager.applicationSettings.nipLangTemplates.Contains(this.stringManipInput.Text)) {
                foreach (string item in stringManipTemplateSelection.Items)
                {
                    if (item == this.stringManipInput.Text)
                    {
                        this.stringManipTemplateSelection.Text = item;
                        this.stringManipTemplateSelection.SelectedItem = item;
                    }
                }

            }
        }
    }
}
