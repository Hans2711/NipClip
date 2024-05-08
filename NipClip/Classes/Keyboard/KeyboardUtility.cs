using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NipClip.Classes.Keyboard
{
    public class KeyboardUtility
    {
        public static void nonNativeLeaderToControlKeyDown(KeyboardHook.VKeys key)
        {
            GlobalMemory.supressAllKeyDownEvents.Add(key);
            sendKeyUp(key);
            sendKeyDown(KeyboardHook.VKeys.LCONTROL);
        }

        public static void nonNativeLeaderToControlKeyUp(KeyboardHook.VKeys key)
        {
            sendKeyUp(KeyboardHook.VKeys.LCONTROL);
            GlobalMemory.supressAllKeyDownEvents.Remove(key);
        }

        public static void nonNativeCopyToCKeyDown(KeyboardHook.VKeys key)
        {
            GlobalMemory.supressAllKeyDownEvents.Add(key);
            sendKeyUp(key);
            sendKeyDown(KeyboardHook.VKeys.KEY_C);
        }

        public static void nonNativeCopyToCKeyUp(KeyboardHook.VKeys key)
        {
            sendKeyUp(KeyboardHook.VKeys.KEY_C);
            GlobalMemory.supressAllKeyDownEvents.Remove(key);
        }

        public static void nonNativePasteToVKeyDown(KeyboardHook.VKeys key)
        {
            GlobalMemory.supressAllKeyDownEvents.Add(key);
            sendKeyUp(key);
            sendKeyDown(KeyboardHook.VKeys.KEY_V);
        }

        public static void nonNativePasteToVKeyUp(KeyboardHook.VKeys key)
        {
            sendKeyUp(KeyboardHook.VKeys.KEY_V);
            GlobalMemory.supressAllKeyDownEvents.Remove(key);
        }

        public static void sendKeyDown(KeyboardHook.VKeys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_KEYDOWN, 0);
        }
        public static void sendKeyUp(KeyboardHook.VKeys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
        }

        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_KEYDOWN = 0x0000;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
    }
}
