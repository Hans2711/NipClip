using NipClip.Classes.Clipboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NipClip.Classes.Keyboard
{
    public class KeyboardReader
    {
        private KeyboardHook hook;

        public List<Shortcut> shortcuts = new List<Shortcut>();

        public List<KeyboardHook.VKeys> buffer = new List<KeyboardHook.VKeys>();

        public List<MainWindow> mainWindows { get; set; }

        public KeyboardReader(ref List<MainWindow> mainWindows)
        {
            this.mainWindows = mainWindows;

            this.hook = new KeyboardHook();
            this.hook.Install();

            this.hook.KeyDown += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyDown);
            this.hook.KeyUp += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);

            this.shortcuts.Add(new PasteLastShortcut());
            this.shortcuts.Add(new CopyShortcut());
        }

        private void checkForShortcut()
        {
            foreach (var shortcut in this.shortcuts)
            {
                foreach (var keys in shortcut.Keys)
                {
                    if (keys.All(item => this.buffer.Contains(item)))
                    {
                        shortcut.mainWindows = this.mainWindows;
                        shortcut.Callback(keys);
                        this.buffer.Clear();
                        break;
                    }
                }
            }

        }

        private void keyboardHook_KeyUp(KeyboardHook.VKeys key)
        {
            this.checkForShortcut();
            this.buffer.Remove(key);
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] KeyUp Event {" + key.ToString() + "}");
        }
        private void keyboardHook_KeyDown(KeyboardHook.VKeys key)
        {
            if (!this.buffer.Contains(key)) 
            {
                this.buffer.Add(key);
            }
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] KeyDown Event {" + key.ToString() + "}");
        }
    }
}
