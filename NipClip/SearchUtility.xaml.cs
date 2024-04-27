﻿using NipClip.Classes;
using NipClip.Classes.Clipboard;
using NipClip.Classes.Clipboard.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// Interaction logic for SearchUtility.xaml
    /// </summary>
    public partial class SearchUtility : Window
    {
        List<SortSetting> sortSettings = new List<SortSetting>();

        BindingList<ClipboardEntry> clipboardEntries = new BindingList<ClipboardEntry>(); 

        List<MainWindow> mainWindows { get; set; }

        public SearchUtility(ref List<MainWindow> mainWindows)
        {
            this.mainWindows = mainWindows;

            InitializeComponent();

            if (File.Exists("sort-settings.xml"))
            {
                List<SortSetting> sortSettings = XmlConverter.DeserializeXml<List<SortSetting>>("sort-settings.xml");
                this.sortSettings = sortSettings;
            }

            if (sortSettings.Count == 0)
            {
                this.sortSettings.Add(new KeywordSortSettings());
            } 
            else
            {
                foreach (var setting in this.sortSettings)
                {
                    if (setting.GetType().Equals(typeof(KeywordSortSettings)))
                    {
                        this.keywordTextbox.Text = string.Join(" ", setting.parameters.ToArray());
                    }
                }
            }

            this.resultsListBox.ItemsSource = this.clipboardEntries;
        }

        private Dictionary<float, List<ClipboardEntry>> weightsHashmap = new Dictionary<float, List<ClipboardEntry>>();

        private void ProcessFilterChange()
        {
            foreach (var window in this.mainWindows)
            {
                foreach (var entry in window.clipboardReader.clipboardStorage.entries)
                {
                    entry.sortWeight = 0;
                    foreach (var setting in this.sortSettings)
                    {
                        float weight = setting.validateEntry(entry);
                    }

                    if (entry.sortWeight > 0)
                    {
                        if (this.weightsHashmap.ContainsKey(entry.sortWeight))
                        {
                            this.weightsHashmap[entry.sortWeight].Add(entry);
                        }
                        else
                        {
                            this.weightsHashmap.Add(entry.sortWeight, new List<ClipboardEntry>());
                            this.weightsHashmap[entry.sortWeight].Add(entry);
                        }
                    }
                }
            }

            var sortedDict = this.weightsHashmap.OrderByDescending(pair => pair.Key)
                                                    .ToDictionary(pair => pair.Key, pair => pair.Value.OrderByDescending(e => e.crDate).ToList());

            this.clipboardEntries.Clear();
            foreach (var pair in sortedDict)
            {
                foreach (var entry in pair.Value)
                {
                    this.clipboardEntries.Add(entry);
                }
            }
            this.weightsHashmap.Clear();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (var setting in this.sortSettings)
            {
                if (setting.GetType().Equals(typeof(KeywordSortSettings)))
                {
                    setting.parameters.Clear();
                    setting.parameters = new List<string>(this.keywordTextbox.Text.Split(' '));
                }
            }

            this.ProcessFilterChange();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string export = XmlConverter.ConvertToXml<List<SortSetting>>(this.sortSettings);
            export = export.Replace("encoding=\"utf-16\"", "");
            File.WriteAllText("sort-settings.xml", export);
        }
    }
}
