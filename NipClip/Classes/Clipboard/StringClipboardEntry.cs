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
                if (ClipboardReader.encryptionEnabled)
                {
                    return DecryptString(ClipboardReader.encryptionKey);
                }
                else
                {
                    return ByteToString(EncryptedContent);
                }
            }
            set
            {
                Encrypt(ClipboardReader.encryptionKey, (string)value);
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
