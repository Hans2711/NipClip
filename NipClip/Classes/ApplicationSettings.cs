using NipClip.Classes.Clipboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NipClip.Classes
{
    [Serializable]
    public class ApplicationSettings
    {
        public string EncryptionKey { set; get; }

        public List<string> nipLangTemplates = new List<string>();

        public ApplicationSettings() {
            this.EncryptionKey = "DefaultKey";

            this.nipLangTemplates.Add("var_dump($N0, $N1)");
            this.nipLangTemplates.Add("[$N0,$N1]");
            this.nipLangTemplates.Add("@import \"$N\"" + Environment.NewLine);

        }

        public void save()
        {
            string export = XmlConverter.ConvertToXml<ApplicationSettings>(this);
            export = export.Replace("encoding=\"utf-16\"", "");
            File.WriteAllText("settings.xml", export);
        }

        public void restore()
        {
            if (File.Exists("settings.xml"))
            {
                ApplicationSettings settings = XmlConverter.DeserializeXml<ApplicationSettings>("settings.xml");
                this.EncryptionKey = settings.EncryptionKey;
            }
        }
    }
}
