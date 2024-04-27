using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace NipClip.Classes.Clipboard.DTO
{
    public class UrlSortSettings : SortSetting
    { 
        public UrlSortSettings()
        {
            this.type = "url";
            this.parameters = new List<string>();
        }
        public override float validateEntry(ClipboardEntry entry)
        {
            if (!entry.isUrl() && SearchUtility.UrlFilter)
            {
                entry.sortWeight = -50F;
                return 0.0F;

            }
            return 0.5F;
        }
    }
}
