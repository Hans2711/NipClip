using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NipClip.Classes.Clipboard.DTO
{

    [Serializable]
    [XmlInclude(typeof(KeywordSortSettings))]
    public abstract class SortSetting
    {
        public virtual List<string> parameters { get; set; }

        public virtual string type { get; set; }

        public virtual bool validateEntry(ClipboardEntry entry)
        {
            return false;
        }

        public SortSetting()
        {
            this.type = "default";
            this.parameters = new List<string>();
        }
    }
}
