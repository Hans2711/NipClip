using NipClip.Classes.Clipboard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Xml.Serialization;

namespace NipClip.Classes.Keyboard
{
    class ImageClipboardEntry : ClipboardEntry, INotifyPropertyChanged
    {

        [XmlIgnore]
        public virtual object Content { 
            get {
                return this.Image;
            }
            set
            {
                this.Image = (Image)value;
            }
        }
    }
}
