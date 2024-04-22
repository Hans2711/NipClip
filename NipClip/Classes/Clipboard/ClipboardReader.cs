using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using System.ComponentModel;
using System.Threading;
using System.Xml.Serialization;

namespace NipClip.Classes.Clipboard
{
    public class ClipboardReader
    {
        public ClipboardHook hook;

        public ClipboardStorage clipboardStorage;

        public Window window;

        public Thread workerThread;

        public bool workerBreaker = true;

        public static string encryptionKey = "DefaultKey";

        public static bool encryptionEnabled = true;

        public ClipboardReader(Window window, string encryptionKey)
        {
            ClipboardReader.encryptionKey = encryptionKey;
            this.window = window;
            this.clipboardStorage = new ClipboardStorage();

            if (File.Exists("entries.xml"))
            {
                List<ClipboardEntry> entries = XmlConverter.DeserializeXml<List<ClipboardEntry>>("entries.xml");
                foreach (ClipboardEntry entry in entries)
                {
                    this.clipboardStorage.entries.Add(entry);
                }
            }

            this.hook = new ClipboardHook(this.window);
            this.hook.ClipboardChanged += this.clipboardStorage.ClipboardChanged;
            this.clipboardStorage.ClipboardChanged();

            this.workerThread = new Thread(this.worker);
            this.workerThread.SetApartmentState(ApartmentState.STA);
            this.workerThread.Start();
        }

        private void worker()
        {
            while (this.workerBreaker)
            {
                try
                {
                    StringClipboardEntry entry = new StringClipboardEntry();
                    entry.Content = System.Windows.Clipboard.GetText();

                    this.clipboardStorage.CheckPossibleNewEntry(entry);
                    Thread.Sleep(980);
                } catch (Exception ex) { }
            }
        }

        public void export()
        {
            this.clipboardStorage.nEntries.Clear();
            foreach (var entry in this.clipboardStorage.entries)
            {
                this.clipboardStorage.nEntries.Add(entry);
            }
            string export = XmlConverter.ConvertToXml<List<ClipboardEntry>>(this.clipboardStorage.nEntries);
            export = export.Replace("encoding=\"utf-16\"", "");

            File.WriteAllText("entries.xml", export);
        }
    }
}
