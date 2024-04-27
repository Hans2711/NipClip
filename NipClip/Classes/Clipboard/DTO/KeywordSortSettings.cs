using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NipClip.Classes.Clipboard.DTO
{
    public class KeywordSortSettings : SortSetting
    {
        public KeywordSortSettings() { 
            this.type = "keyword";
            this.parameters = new List<string>();
        }

        public override bool validateEntry(ClipboardEntry entry)
        {
            if (entry.Content is string content)
            {
                string[] words = content.Split(" ");
                // Check for any word that contains any part of any parameter
                foreach (string word in words)
                {
                    if (this.parameters.Any(param => word.Contains(param.ToLower().Trim())))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
