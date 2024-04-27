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
using System.Security.Policy;
using System.Windows.Threading;
using NipClip.Classes.Keyboard;

namespace NipClip.Classes.Clipboard
{
    public class ClipboardReader
    {
        public ClipboardHook hook;

        public ClipboardStorage clipboardStorage;

        public MainWindow window;

        public Thread workerThread;

        public bool workerBreaker = true;

        public static string encryptionKey = "DefaultKey";

        public static bool encryptionEnabled = true;

        public string filename = "entries-default.xml";

        public bool nonNativeClipboard = false;

        public ClipboardReaderSettings settings {  get; set; }

        public Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        public ClipboardReader(MainWindow window, string filename = null)
        {
            this.settings = new ClipboardReaderSettings(filename);
            this.settings.restore();

            if (filename != null && !filename.Contains("default"))
            {
                this.filename = filename;
                this.nonNativeClipboard = true;
            }

            ClipboardReader.encryptionKey = encryptionKey;
            this.window = window;
            this.clipboardStorage = new ClipboardStorage();

            if (File.Exists(this.filename))
            {
                List<ClipboardEntry> entries = XmlConverter.DeserializeXml<List<ClipboardEntry>>(this.filename);
                foreach (ClipboardEntry entry in entries)
                {
                    this.clipboardStorage.entries.Add(entry);
                }
            }

            if (!this.nonNativeClipboard)
            {
                this.hook = new ClipboardHook(this.window);
                this.hook.ClipboardChanged += this.clipboardStorage.ClipboardChanged;
                this.clipboardStorage.ClipboardChanged();

                this.workerThread = new Thread(this.worker);
                this.workerThread.SetApartmentState(ApartmentState.STA);
                this.workerThread.Start();
            }
            else
            {
                if (this.clipboardStorage.entries.Count == 0)
                {
                    StringClipboardEntry entry = new StringClipboardEntry();
                    entry.Content = "====";
                    this.clipboardStorage.entries.Add(entry);
                }
            }
        }

        public void kill()
        {
            if (this.workerThread != null) { this.workerThread.Abort(); }

            this.clipboardStorage.entries.Clear();
            File.Delete(this.filename); 

            this.window.notifyIcon.Visible = false;
            this.window.Close();
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

                    ImageClipboardEntry imageClipboardEntry = new ImageClipboardEntry();
                    imageClipboardEntry.Content = System.Windows.Clipboard.GetImage();

                    //this.clipboardStorage.CheckPossibleNewEntry(imageClipboardEntry);

                    foreach (ClipboardEntry entryy in this.clipboardStorage.entries)
                    {
                        if (entryy.toBeDeleted)
                        {
                            this.dispatcher.Invoke(() =>
                            {
                                this.clipboardStorage.entries.Remove(entryy);
                            });
                        }
                    }
                } catch (Exception ex) { }
                Thread.Sleep(880);
            }
        }

        public void export()
        {
            this.clipboardStorage.nEntries.Clear();
            foreach (var entry in this.clipboardStorage.entries)
            {
                if (entry.Content == null)
                {
                    continue;
                }
                this.clipboardStorage.nEntries.Add(entry);
            }

            string export = XmlConverter.ConvertToXml<List<ClipboardEntry>>(this.clipboardStorage.nEntries);
            export = export.Replace("encoding=\"utf-16\"", "");

            this.settings.save();

            File.WriteAllText(this.filename, export);
        }

        public void purgeData()
        {
            this.clipboardStorage.nEntries.Clear();
            this.clipboardStorage.entries.Clear();

            this.export();
        }
    }
}
