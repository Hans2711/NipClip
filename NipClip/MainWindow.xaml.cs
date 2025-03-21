﻿using NipClip.Classes;
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

            this.DataContext = this;

            this.stringManipTemplateSelection.ItemsSource = WindowManager.applicationSettings.nipLangTemplates;
            this.languagesSelection.ItemsSource = WindowManager.applicationSettings.languages;

            ClipboardReader.encryptionKey = WindowManager.applicationSettings.EncryptionKey;

            this.stringManipInput_GotFocus();

            this.currentLeader.ItemsSource = Enum.GetValues(typeof(KeyboardHook.VKeys));
            this.currentLeader.SelectedItem = WindowManager.applicationSettings.leaderKey;

            this.copyKeySelection.ItemsSource = Enum.GetValues(typeof(KeyboardHook.VKeys));
            this.copyKeySelection.SelectedItem = WindowManager.applicationSettings.copyKey;

            this.pasteKeySelection.ItemsSource = Enum.GetValues(typeof(KeyboardHook.VKeys));
            this.pasteKeySelection.SelectedItem = WindowManager.applicationSettings.pasteKey;

            this.languagesSelection.SelectedItem = WindowManager.applicationSettings.selectedLanguage;
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
            WindowManager.applicationSettings.save();
            this.Close();
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
            this.reencryptButton.Content = Properties.Resources.reencrypt;

            if (!string.IsNullOrEmpty(this.encryptionKey.Text))
            {
                if (this.reencryptButton.Content != Properties.Resources.done)
                {
                    WindowManager.RencryptAll(this.encryptionKey.Text);
                    WindowManager.applicationSettings.EncryptionKey = this.encryptionKey.Text;

                    this.clipboardReader.export();
                    this.reencryptButton.Content = Properties.Resources.done;
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
                WindowManager.applicationSettings.DecryptionEnabled = false;
            } else
            {
                WindowManager.applicationSettings.DecryptionEnabled = true;
            }
            
            WindowManager.RefreshAll();
        }

        public void Refresh()
        {
            try
            {
                this.textList.Items.Refresh();

                if (this.currentLeader.SelectedItem != null && WindowManager.applicationSettings.leaderKey != null)
                {
                    if ((KeyboardHook.VKeys)this.currentLeader.SelectedItem != WindowManager.applicationSettings.leaderKey)
                    {
                        this.currentLeader.SelectedItem = WindowManager.applicationSettings.leaderKey;
                        this.changeCurrentLeaderButton.Content = Properties.Resources.change;
                    }

                    if (this.copyKeySelection != null && (KeyboardHook.VKeys)this.copyKeySelection.SelectedItem != WindowManager.applicationSettings.copyKey)
                    {
                        this.copyKeySelection.SelectedItem = WindowManager.applicationSettings.copyKey;
                        this.changeCopyButton.Content = Properties.Resources.change;
                    }

                    if (this.pasteKeySelection != null && (KeyboardHook.VKeys)this.pasteKeySelection.SelectedItem != WindowManager.applicationSettings.pasteKey)
                    {
                        this.pasteKeySelection.SelectedItem = WindowManager.applicationSettings.pasteKey;
                        this.changepasteButton.Content = Properties.Resources.change;
                    }
                }

                this.clipboardIDReverse.IsChecked = WindowManager.applicationSettings.ClipboardIDReverse;
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

        private void currentLeader_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WindowManager.ChangeKeyboardLeader((KeyboardHook.VKeys)this.currentLeader.SelectedItem);
        }

        private void changeCurrentLeaderButton_Click(object sender, RoutedEventArgs e)
        {
            this.changeCurrentLeaderButton.Content = Properties.Resources.waitingForInput;
            WindowManager.ChangeKeyboardLeader();
        }

        private void clipboardIDReverse_Click(object sender, RoutedEventArgs e)
        {
            if (this.clipboardIDReverse.IsChecked == true)
            {
                WindowManager.applicationSettings.ClipboardIDReverse = true;
                WindowManager.RefreshAll();
            }
            else
            {
                WindowManager.applicationSettings.ClipboardIDReverse = false;
                WindowManager.RefreshAll();
            }
        }

        private void generateKey_Click(object sender, RoutedEventArgs e)
        {
            this.encryptionKey.Text = RandomUtility.RandomString(25);
        }

        private void openSearchUtil_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.OpenSearchUtility();
        }

        private void languagesSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WindowManager.applicationSettings.languages.Contains(this.languagesSelection.SelectedItem))
            {
                if ((string)this.languagesSelection.SelectedItem != WindowManager.applicationSettings.selectedLanguage)
                {
                    WindowManager.applicationSettings.selectedLanguage = (string)this.languagesSelection.SelectedItem;
                    WindowManager.Close(null, null);
                    WindowManager.clipboardMainWindows = new List<MainWindow> ();
                    WindowManager.CreateNewClipboardMainWindow();
                    WindowManager.applicationSettings.setCurrentLanguage();
                }
            }
        }

        private void copyKeySelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WindowManager.ChangeCopyKey((KeyboardHook.VKeys)this.copyKeySelection.SelectedItem);
        }

        private void pasteKeySelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WindowManager.ChangePasteKey((KeyboardHook.VKeys)this.pasteKeySelection.SelectedItem);
        }

        private void changepasteButton_Click(object sender, RoutedEventArgs e)
        {
            this.changepasteButton.Content = Properties.Resources.waitingForInput;
            WindowManager.ChangePasteKey();
        }

        private void changeCopyButton_Click(object sender, RoutedEventArgs e)
        {
            this.changeCopyButton.Content = Properties.Resources.waitingForInput;
            WindowManager.ChangeCopyKey();
        }
    }
}
