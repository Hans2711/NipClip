using NipClip.Classes.Clipboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NipClip.Classes.Keyboard
{
    public class KeyboardReader
    {
        public KeyboardHook hook;

        public List<KeyboardHook.VKeys> buffer = new List<KeyboardHook.VKeys>();

        public List<MainWindow> mainWindows { get; set; }

        public bool setNextLeaderKey { get; set; }
        public bool setNextCopyKey { get; set; }
        public bool setNextPasteKey { get; set; }

        public bool nonNativeLeaderKey = false;

        public bool nonNativeCopyKey = false;

        public bool nonNativePasteKey = false;

        public KeyboardReader(ref List<MainWindow> mainWindows)
        {
            this.mainWindows = mainWindows;

            this.hook = new KeyboardHook();
            this.hook.Install();

            this.hook.KeyDown += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyDown);
            this.hook.KeyUp += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);

            if (
                WindowManager.applicationSettings.leaderKey != KeyboardHook.VKeys.CONTROL &&
                WindowManager.applicationSettings.leaderKey != KeyboardHook.VKeys.LCONTROL &&
                WindowManager.applicationSettings.leaderKey != KeyboardHook.VKeys.RCONTROL
                )
            {
                this.nonNativeLeaderKey = true;
            }

            if (WindowManager.applicationSettings.copyKey != KeyboardHook.VKeys.KEY_C)
            {
                this.nonNativeCopyKey = true;
            }

            if (WindowManager.applicationSettings.pasteKey != KeyboardHook.VKeys.KEY_V)
            {
                this.nonNativePasteKey = true;
            }
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
            if (key == WindowManager.applicationSettings.leaderKey)
            {
                if (this.nonNativeLeaderKey)
                {
                    KeyboardUtility.nonNativeLeaderToControlKeyUp(key);
                    WindowManager.CloseTransparentWindow();
                    this.redirectKeyDownEvent = false;
                    return false;
                }
                WindowManager.CloseTransparentWindow();
                this.redirectKeyDownEvent = false;
                return true;
            }

            if (key == WindowManager.applicationSettings.copyKey && this.nonNativeCopyKey)
            {
                KeyboardUtility.nonNativeCopyToCKeyUp(key);
            }

            if (key == WindowManager.applicationSettings.pasteKey && this.nonNativePasteKey)
            {
                KeyboardUtility.nonNativePasteToVKeyUp(key);
            }

            //Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] KeyUp Event {" + key.ToString() + "}");
            return true;
        }

        private bool redirectKeyDownEvent = false;

        private bool keyboardHook_KeyDown(KeyboardHook.VKeys key)
        {
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

            if (GlobalMemory.supressAllKeyDownEvents.Contains(key))
            {
                return false;
            }

            if (key == WindowManager.applicationSettings.leaderKey)
            {

                if (this.nonNativeLeaderKey)
                {
                    KeyboardUtility.nonNativeLeaderToControlKeyDown(key);
                    WindowManager.OpenTransparentWindow();
                    return false;
                }

                WindowManager.OpenTransparentWindow();
                return true;
            }

            if (WindowManager.transparentWindow != null)
            {
                if (WindowManager.transparentWindow.active == true && WindowManager.transparentWindow.activeAction == string.Empty && key == WindowManager.applicationSettings.copyKey)
                {
                    if (this.nonNativeCopyKey)
                    {
                        KeyboardUtility.nonNativeCopyToCKeyDown(key);
                        WindowManager.transparentWindow.StartCopyAction();
                        this.redirectKeyDownEvent = true;
                        return false;
                    }

                    WindowManager.transparentWindow.StartCopyAction();
                    this.redirectKeyDownEvent = true;
                    return true;
                }

                if (WindowManager.transparentWindow.active == true && WindowManager.transparentWindow.activeAction == string.Empty && key == WindowManager.applicationSettings.pasteKey)
                {
                    if (this.nonNativePasteKey)
                    {
                        KeyboardUtility.nonNativePasteToVKeyDown(key);
                        WindowManager.transparentWindow.StartPasteAction();
                        this.redirectKeyDownEvent = true;
                        return false;
                    }
                    WindowManager.transparentWindow.StartPasteAction();
                    this.redirectKeyDownEvent = true;
                    return true;
                }

                return true;
            }

            if (this.redirectKeyDownEvent)
            {
                return WindowManager.transparentWindow.keyboardHook_KeyDown(key);
            }

            //Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] KeyUp Event {" + key.ToString() + "}");
            return true;
        }
    }
}
