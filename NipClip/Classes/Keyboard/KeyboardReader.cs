using NipClip.Classes.Clipboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NipClip.Classes.Keyboard
{
    public class KeyboardReader
    {
        private KeyboardHook hook;

        public List<KeyboardHook.VKeys> ignoreSupressKeys = new List<KeyboardHook.VKeys>();

        public List<MainWindow> mainWindows { get; set; }

        public bool setNextLeaderKey { get; set; }
        public bool setNextCopyKey { get; set; }
        public bool setNextPasteKey { get; set; }

        public bool redirectEventsToTransparentWindow { get; set; }

        public bool supressNonNativeLeaderUp = false;
        
        public bool nonNative { get; set; }

        public KeyboardReader(ref List<MainWindow> mainWindows)
        {
            this.mainWindows = mainWindows;

            this.hook = new KeyboardHook();
            this.hook.Install();

            this.hook.KeyDown += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyDown);
            this.hook.KeyUp += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);

            if (WindowManager.applicationSettings.leaderKey == KeyboardHook.VKeys.LCONTROL
                || WindowManager.applicationSettings.leaderKey == KeyboardHook.VKeys.RCONTROL
                || WindowManager.applicationSettings.leaderKey == KeyboardHook.VKeys.CONTROL
            )
            {
                this.nonNative = false;
            }
            else
            {
                this.nonNative = true;
            }

            this.ignoreSupressKeys = new List<KeyboardHook.VKeys>();
        }

        ~KeyboardReader()
        {
            this.hook.Uninstall();
        }

        public void fillNextInputToLeaderKey()
        {
            this.setNextLeaderKey = true;
        }

        public void fillNextInputToCopyKey()
        {
            this.setNextCopyKey = true;
        }

        public void fillNextInputToPasteKey()
        {
            this.setNextPasteKey = true;
        }

        private bool keyboardHook_KeyUp(KeyboardHook.VKeys key)
        {
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] KeyUp Event {" + key.ToString() + "}");
            if (this.redirectEventsToTransparentWindow && WindowManager.transparentWindow != null)
            {
                bool result = WindowManager.transparentWindow.keyUp(key);
                if (key == WindowManager.applicationSettings.leaderKey && !supressNonNativeLeaderUp)
                {
                    Console.WriteLine("closing1");
                    WindowManager.CloseTransparentWindow();
                    this.redirectEventsToTransparentWindow = false;
                    ClipboardUtility.sendKeyUp(KeyboardHook.VKeys.LCONTROL);
                }
                if (nonNative)
                {
                    return false;
                }
                return result;
            }

            if (key == WindowManager.applicationSettings.leaderKey && !supressNonNativeLeaderUp)
            {
                Console.WriteLine("closing2");
                WindowManager.CloseTransparentWindow();
                this.redirectEventsToTransparentWindow = false;
            }

            return true;
        }
        private bool keyboardHook_KeyDown(KeyboardHook.VKeys key)
        {
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] KeyDown Event {" + key.ToString() + "}");

            if (ignoreSupressKeys.Contains(key))
            {
                return false;
            }

            if (this.setNextLeaderKey)
            {
                WindowManager.ChangeKeyboardLeader(key);
                this.setNextLeaderKey = false;
                return false;
            }
            if (this.setNextCopyKey)
            {
                WindowManager.ChangeCopyKey(key);
                this.setNextCopyKey = false;
                return false;
            }
            if (this.setNextPasteKey)
            {
                WindowManager.ChangePasteKey(key); 
                this.setNextPasteKey = false;
                return false;
            }

            if (this.redirectEventsToTransparentWindow)
            {
                return WindowManager.transparentWindow.keyDown(key);
            }

            if (key == WindowManager.applicationSettings.leaderKey)
            {
                if (this.nonNative)
                {
                    WindowManager.keyboardReader.supressNonNativeLeaderUp = true;
                }
                if (WindowManager.transparentWindow == null)
                {
                    WindowManager.OpenTransparentWindow();
                    this.redirectEventsToTransparentWindow = true;
                    if (this.nonNative)
                    {
                        return false;
                    }
                }
                else
                {
                    if (WindowManager.transparentWindow.Visibility == System.Windows.Visibility.Hidden)
                    {
                        WindowManager.OpenTransparentWindow();
                        this.redirectEventsToTransparentWindow = true;
                        if (this.nonNative)
                        {
                            return false;
                        }
                    }
                }
            }


            return true;
        }
    }
}
