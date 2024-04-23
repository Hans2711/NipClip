using NipClip.Classes.Clipboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NipClip.Classes.Keyboard
{
    public class CopyShortcut : Shortcut
    {
        public CopyShortcut()
        {
            this.Keys = new List<List<KeyboardHook.VKeys>>();

            this.Keys.Add(new List<KeyboardHook.VKeys>());
            this.Keys.Last().Add(KeyboardHook.VKeys.RMENU);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_C);

            this.Keys.Add(new List<KeyboardHook.VKeys>());
            this.Keys.Last().Add(KeyboardHook.VKeys.RMENU);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_C);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_1);

            this.Keys.Add(new List<KeyboardHook.VKeys>());
            this.Keys.Last().Add(KeyboardHook.VKeys.RMENU);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_C);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_2);

            this.Keys.Add(new List<KeyboardHook.VKeys>());
            this.Keys.Last().Add(KeyboardHook.VKeys.RMENU);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_C);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_3);

            this.Keys.Add(new List<KeyboardHook.VKeys>());
            this.Keys.Last().Add(KeyboardHook.VKeys.RMENU);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_C);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_4);

            this.Keys.Add(new List<KeyboardHook.VKeys>());
            this.Keys.Last().Add(KeyboardHook.VKeys.RMENU);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_C);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_5);

            this.Keys.Add(new List<KeyboardHook.VKeys>());
            this.Keys.Last().Add(KeyboardHook.VKeys.RMENU);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_C);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_6);

            this.Keys.Add(new List<KeyboardHook.VKeys>());
            this.Keys.Last().Add(KeyboardHook.VKeys.RMENU);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_C);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_7);

            this.Keys.Add(new List<KeyboardHook.VKeys>());
            this.Keys.Last().Add(KeyboardHook.VKeys.RMENU);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_C);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_8);

            this.Keys.Add(new List<KeyboardHook.VKeys>());
            this.Keys.Last().Add(KeyboardHook.VKeys.RMENU);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_C);
            this.Keys.Last().Add(KeyboardHook.VKeys.KEY_9);

            this.Keys.Reverse();
        }

        public override void Callback(List<KeyboardHook.VKeys> activationKeys)
        {
            int index = 1;
            if (activationKeys.Contains(KeyboardHook.VKeys.KEY_1))
            {
                index = 1;
            }
            if (activationKeys.Contains(KeyboardHook.VKeys.KEY_2))
            {
                index = 2;
            }
            if (activationKeys.Contains(KeyboardHook.VKeys.KEY_3))
            {
                index = 3;
            }
            if (activationKeys.Contains(KeyboardHook.VKeys.KEY_4))
            {
                index = 4;
            }
            if (activationKeys.Contains(KeyboardHook.VKeys.KEY_5))
            {
                index = 5;
            }
            if (activationKeys.Contains(KeyboardHook.VKeys.KEY_6))
            {
                index = 6;
            }
            if (activationKeys.Contains(KeyboardHook.VKeys.KEY_7))
            {
                index = 7;
            }
            if (activationKeys.Contains(KeyboardHook.VKeys.KEY_8))
            {
                index = 8;
            }
            if (activationKeys.Contains(KeyboardHook.VKeys.KEY_9))
            {
                index = 9;
            }

            KeyboardSender.SendCopyAction();

            string content = System.Windows.Clipboard.GetText();
            StringClipboardEntry entry = new StringClipboardEntry();
            entry.Content = content;    

            foreach (var mainWindow in this.mainWindows)
            {
                if (mainWindow.clipboardID == index)
                {
                    mainWindow.clipboardReader.clipboardStorage.CheckPossibleNewEntry(entry);
                }
            }
        }
    }
}
