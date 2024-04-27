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

        public bool setNextLeaderKey { get; set; }

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

        ~KeyboardReader()
        {
            this.shortcuts.Clear();
            this.hook.Uninstall();
        }

        public void fillNextInputToLeaderKey()
        {
            this.setNextLeaderKey = true;
        }

        private bool checkForShortcut()
        {
            foreach (var shortcut in this.shortcuts)
            {
                foreach (var keys in shortcut.Keys)
                {
                    if (keys.All(item => this.buffer.Contains(item)))
                    {
                        foreach (KeyboardHook.VKeys key in this.buffer)
                        {
                            Console.WriteLine("[Buffer Key]: " + key.ToString());
                        }

                        shortcut.mainWindows = this.mainWindows;
                        this.buffer.Clear();
                        return shortcut.Callback(keys);
                    }
                }
            }
            return false;
        }

        private bool keyboardHook_KeyUp(KeyboardHook.VKeys key)
        {
            this.buffer.Remove(key);
            //Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] KeyUp Event {" + key.ToString() + "}");
            return true;
        }
        private bool keyboardHook_KeyDown(KeyboardHook.VKeys key)
        {
            if (this.setNextLeaderKey)
            {
                WindowManager.ChangeKeyboardLeader(key);
                this.setNextLeaderKey = false;
            }

            if (!this.buffer.Contains(key)) 
            {
                this.buffer.Add(key);
                if (this.checkForShortcut())
                {
                    return false;
                }
            }
            //Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] KeyDown Event {" + key.ToString() + "}");
            return true;
        }
    }
}
