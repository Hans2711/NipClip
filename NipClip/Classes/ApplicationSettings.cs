using NipClip.Classes.Clipboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using NipClip.Classes.Keyboard;

namespace NipClip.Classes
{
    [Serializable]
    public class ApplicationSettings
    {
        public string EncryptionKey { set; get; }
        public bool DecryptionEnabled { set; get; }

        public List<string> nipLangTemplates = new List<string>();

        public KeyboardHook.VKeys leaderKey { get; set; }

        public bool ClipboardIDReverse = false;

        public List<string> languages = new List<string>();

        public string selectedLanguage { get; set; }

        public ApplicationSettings() { }


        public void saveStringTemplate(string template)
        {
            if (!this.nipLangTemplates.Contains(template))
            {
                this.nipLangTemplates.Add(template);
            }
        }

        public void deleteStringTemplate(string template)
        {
            if (this.nipLangTemplates.Contains(template))
            {
                this.nipLangTemplates.Remove(template);
            }
        }

        public void setCurrentLanguage()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(this.selectedLanguage);
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
                this.nipLangTemplates = settings.nipLangTemplates;
                this.leaderKey = settings.leaderKey;
                this.DecryptionEnabled = settings.DecryptionEnabled;
                this.ClipboardIDReverse = settings.ClipboardIDReverse;
                this.languages = settings.languages;
                this.selectedLanguage = settings.selectedLanguage;

                if (this.nipLangTemplates.Count <= 0)
                {
                    this.nipLangTemplates.Add("var_dump($N0, $N1)");
                    this.nipLangTemplates.Add("[$N0,$N1]");
                    this.nipLangTemplates.Add("$N0,\n$N1");
                    this.nipLangTemplates.Add("@import \"$N\"\\n");
                    this.nipLangTemplates.Add("using $N\n");
                }

                if (this.languages.Count <= 0)
                {
                    this.languages.Add("en");
                    this.languages.Add("de");
                    this.languages.Add("es");
                }


                if (string.IsNullOrEmpty(this.selectedLanguage))
                {
                    this.selectedLanguage = "en";
                }

                this.setCurrentLanguage();

                if (string.IsNullOrEmpty(this.EncryptionKey))
                {
                    this.EncryptionKey = "DefaultKey";
                    this.DecryptionEnabled = true;
                }
            }
        }
    }
}
