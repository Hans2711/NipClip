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

        [XmlIgnore]
        public string StringContent = string.Empty;

        public virtual string? ToString()
        {
            return (string)Content;
        }

        [XmlIgnore]
        public override object Content
        {
            get { return StringContent; }
            set
            {
                if (value != this.StringContent)
                {
                    this.StringContent = (string)value;
                    NotifyPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
