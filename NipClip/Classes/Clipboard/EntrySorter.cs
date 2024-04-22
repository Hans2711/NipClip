using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NipClip.Classes.Clipboard
{
    public class EntrySorter
    {
        public BindingList<ClipboardEntry> sortedEntries;
        public BindingList<ClipboardEntry> entries;
        public EntrySorter(ref BindingList<ClipboardEntry> sortedEntries, ref BindingList<ClipboardEntry> entries) {
            this.sortedEntries = sortedEntries;
            this.entries = entries;

            this.sortedEntries.Clear();
            this.entries.Clear();
        }
    }
}
