using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NipClip.Classes.Clipboard
{
    [Serializable]
    public class StringClipboardEntry : ClipboardEntry, INotifyPropertyChanged
    {
        public override string type { get; set; }

        public virtual string? ToString()
        {
            return (string)Content;
        }

        [XmlIgnore]
        public override object Content
        {
            get
            {
                if (WindowManager.applicationSettings.DecryptionEnabled)
                {
                    return DecryptString(WindowManager.applicationSettings.EncryptionKey);
                }
                else
                {
                    return ByteToString(EncryptedContent);
                }
            }
            set
            {
                Encrypt(WindowManager.applicationSettings.EncryptionKey, (string)value);
                NotifyPropertyChanged();
            }
        }
        public override List<ClipboardEntry> Split(string delimiter)
        {
            List<ClipboardEntry> parts = new List<ClipboardEntry>();
            var contentString = this.Content as string;
            if (contentString == null) return parts; // Return empty if content is not a string

            var splitContents = contentString.Split(new string[] { delimiter }, StringSplitOptions.None);

            foreach (var part in splitContents)
            {
                var entry = new StringClipboardEntry();
                entry.Content = part;
                parts.Add(entry);
            }

            return parts;
        }

        public override void pasteToClipboard()
        {
            if (!string.IsNullOrEmpty((string)this.Content))
            { 
                System.Windows.Clipboard.SetText((string)this.Content);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
