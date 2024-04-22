using NipClip.Classes.Clipboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NipClip.Classes.Keyboard
{
    public class PasteLastShortcut : Shortcut
    {
        public PasteLastShortcut() 
        {
            this.Keys = new List<List<KeyboardHook.VKeys>>();

            this.Keys.Add(new List<KeyboardHook.VKeys>());
            this.Keys.Last().Add(KeyboardHook.VKeys.RMENU);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_V);

            this.Keys.Add(new List<KeyboardHook.VKeys>());
            this.Keys.Last().Add(KeyboardHook.VKeys.RMENU);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_V);
            this.Keys.Last().Add(KeyboardHook.VKeys.UP);

            this.Keys.Add(new List<KeyboardHook.VKeys>());
            this.Keys.Last().Add(KeyboardHook.VKeys.RMENU);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_V);
            this.Keys.Last().Add(KeyboardHook.VKeys.UP);
            this.Keys.Last().Add(KeyboardHook.VKeys.UP);

            this.Keys.Add(new List<KeyboardHook.VKeys>());
            this.Keys.Last().Add(KeyboardHook.VKeys.RMENU);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_V);
            this.Keys.Last().Add(KeyboardHook.VKeys.UP);
            this.Keys.Last().Add(KeyboardHook.VKeys.UP);
            this.Keys.Last().Add(KeyboardHook.VKeys.UP);

            this.Keys.Reverse();
        }

        public override void Callback(List<KeyboardHook.VKeys> activationKeys)
        {
            int index = 1;
            if (activationKeys.Contains(KeyboardHook.VKeys.UP))
            {
                int count = activationKeys.Count(key => key == KeyboardHook.VKeys.UP);
                index = index + 1;
            }

            //if (this.clipboardReader.clipboardStorage.entries.Count > index)
            //{
            //    ClipboardStorage.ignoreElement = this.clipboardReader.clipboardStorage.entries[index];

            //    this.clipboardReader.clipboardStorage.entries[index].pasteToClipboard();
            //}
        }

    }
}
