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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NipClip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ClipboardReader clipboardReader { get; set; }

        public ApplicationSettings applicationSettings { get; set; }

        public KeyboardReader keyboardReader { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.applicationSettings = new ApplicationSettings();
            this.applicationSettings.restore();

            this.clipboardReader = new ClipboardReader(this, this.applicationSettings.EncryptionKey);
            this.textList.ItemsSource = clipboardReader.clipboardStorage.entries;
            this.DataContext = clipboardReader.clipboardStorage.entries;

            this.stringManipTemplateSelection.Items.Add("var_dump($N0, $N1)");
            this.stringManipTemplateSelection.Items.Add("[$N0,$N1]");
            this.stringManipTemplateSelection.Items.Add("@import \"$N\"" + Environment.NewLine);

            this.keyboardReader = new KeyboardReader();
            this.keyboardReader.clipboardReader = this.clipboardReader;

            ClipboardReader.encryptionKey = this.applicationSettings.EncryptionKey;

            this.sortingDataGrid.ItemsSource = this.clipboardReader.clipboardStorage.entries;
        }
        private void stringManipTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.stringManipInput.Text = this.stringManipTemplateSelection.SelectedItem.ToString();
        }

        private void textList_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.clipboardReader.export();
            this.applicationSettings.save();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool notEmpty = false;
            List<ClipboardEntry> processList = new List<ClipboardEntry>();
            foreach (ClipboardEntry item in this.textList.SelectedItems)
            {
                var split = item.Split(this.stringManipInnerDelimiter.Text);
                foreach (var entry in split)
                {
                    processList.Add(entry);
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

            this.stringManipOutput.Text = output;
        }

        private void reencryptButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.reencryptButton.Content = "Reencrypt";

            if (!string.IsNullOrEmpty(this.encryptionKey.Text))
            {
                if (this.reencryptButton.Content != "Done")
                {
                    foreach (var entry in this.clipboardReader.clipboardStorage.entries)
                    {
                        entry.Reencrypt(this.encryptionKey.Text);
                    }
                    this.applicationSettings.EncryptionKey = this.encryptionKey.Text;
                    ClipboardReader.encryptionKey = this.applicationSettings.EncryptionKey;

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

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.disableOnDemandDecryption.IsChecked)
            {
                ClipboardReader.encryptionEnabled = false;
            } else
            {
                ClipboardReader.encryptionEnabled = true;
            }
            this.Refresh();
        }

        public void Refresh()
        {
            try
            {
                this.sortingDataGrid.Items.Refresh();
                this.textList.Items.Refresh();
            } catch { }
        }
    }
}
