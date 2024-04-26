using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;

namespace NipClip.Classes.Clipboard
{
    public class ClipboardReaderSettings
    {
        [XmlIgnore]
        private string filename { get; set; }

        public ClipboardReaderSettings(string filename)
        {
            this.filename = filename.Replace("entries", "settings");
        }

        public ClipboardReaderSettings()
        {

        }

        public void save()
        {
            string export = XmlConverter.ConvertToXml<ClipboardReaderSettings>(this);
            export = export.Replace("encoding=\"utf-16\"", "");
            File.WriteAllText(this.filename, export);
        }

        public void restore()
        {
            if (File.Exists(this.filename))
            {
                ClipboardReaderSettings settings = XmlConverter.DeserializeXml<ClipboardReaderSettings>(this.filename);
            }
        }

    }
}
