using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;
using NipClip.Classes;

namespace NipClip.Classes.Clipboard
{
    public class ClipboardStorage
    {

        public BindingList<ClipboardEntry> entries = new BindingList<ClipboardEntry>();
        public List<ClipboardEntry> nEntries = new List<ClipboardEntry>();
        
        Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        public void CheckPossibleNewEntry(StringClipboardEntry entry)
        {
            if (entry == null || entry.StringContent == null)
            {
                // If the entry or its content is null, do nothing
                return;
            }

            // Check if the entries list is empty
            if (entries.Count == 0)
            {
                // If the list is empty, add the entry directly
                entries.Add(entry);
            }
            else
            {
                // Check if the content of the entry matches the content of the last entry in the list
                StringClipboardEntry lastEntry = (StringClipboardEntry) entries.First();
                if (lastEntry != null && lastEntry.Content != null && !lastEntry.Content.Equals(entry.Content))
                {
                    // If the content matches, add the entry to the list
                    this.dispatcher.Invoke(() =>
                    {
                        entries.Insert(0, entry);
                    });
                }
            }
        }

        bool handlerRunning = false;
        public void ClipboardChanged(object sender = null, EventArgs e = null)
        {
            if (handlerRunning)
            {
                return;
            }

            handlerRunning = true;

            try
            {
                if (entries.Count > 0 && entries.First().Content != System.Windows.Clipboard.GetText())
                {
                    StringClipboardEntry stringClipboardEntry = new StringClipboardEntry();
                    stringClipboardEntry.Content = System.Windows.Clipboard.GetText();
                    this.CheckPossibleNewEntry(stringClipboardEntry);
                }

                if (entries.Count == 0)
                {
                    StringClipboardEntry stringClipboardEntry = new StringClipboardEntry();
                    stringClipboardEntry.Content = System.Windows.Clipboard.GetText();
                    this.CheckPossibleNewEntry(stringClipboardEntry);
                }

                Console.WriteLine(System.Windows.Clipboard.GetText());
            }
            catch { }

            handlerRunning = false;
        }

    }
}
