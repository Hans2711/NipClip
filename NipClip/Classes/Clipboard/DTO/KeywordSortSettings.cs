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

        public float partialWeight = 0.5F;
        public float completeWeight = 1F;
        public float caseSensitiveCompleteWeight = 1.2F;

        public override float validateEntry(ClipboardEntry entry)
        {
            float weight = 0;
            if (entry.Content is string content)
            {
                string[] words = content.Split(" ");
                foreach (string word in words)
                {
                    foreach (var param in this.parameters)
                    {
                        if (word.ToLower().Trim().Contains(param.ToLower().Trim()))
                        {
                            weight += partialWeight;
                        }
                        if (word.ToLower().Trim().Equals(param.ToLower().Trim()))
                        {
                            weight += completeWeight;
                        }
                        if (word.Trim().Equals(param.ToLower().Trim()))
                        {
                            weight += caseSensitiveCompleteWeight;
                        }
                    }
                }

                if (entry.urlContent != null)
                {
                    foreach (var param in this.parameters)
                    {
                        if (entry.urlContent.ToLower().Contains(param.ToLower()))
                        {
                            weight += partialWeight - 0.1F;;
                        }
                    }
                }
            }
            entry.sortWeight += weight;
            return weight;
       }
    }
}
