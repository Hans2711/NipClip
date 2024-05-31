using NipClip.Classes.Keyboard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
        }

        public void SendKeys()
        {
            string contentString = this.Content as string;
            if (string.IsNullOrEmpty(contentString)) return;

            foreach (char c in contentString)
            {
                KeyboardHook.VKeys key = CharToVKey(c);

                GlobalMemory.ignoreOnceKeyUpEvent.Add(key);
                GlobalMemory.ignoreOnceKeyDownEvent.Add(key);

                KeyboardUtility.sendKeyDown(key);
                Thread.Sleep(10);  // Small delay to simulate typing
                KeyboardUtility.sendKeyUp(key);
                Thread.Sleep(10);  // Small delay to simulate typing
            }
        }

        private KeyboardHook.VKeys CharToVKey(char c)
        {
            // Convert character to corresponding VKey
            if (char.IsLetter(c))
            {
                return (KeyboardHook.VKeys)Enum.Parse(typeof(KeyboardHook.VKeys), "KEY_" + char.ToUpper(c));
            }
            if (char.IsDigit(c))
            {
                return (KeyboardHook.VKeys)Enum.Parse(typeof(KeyboardHook.VKeys), "KEY_" + c);
            }
            switch (c)
            {
                case ' ': return KeyboardHook.VKeys.SPACE;
                case '\n': return KeyboardHook.VKeys.RETURN;
                case '\t': return KeyboardHook.VKeys.TAB;
                // Add other special character mappings here
                default: return KeyboardHook.VKeys.NONE;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
