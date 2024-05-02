using NipClip.Classes.Clipboard;
using NipClip.Classes.Keyboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;

namespace NipClip.Classes.Clipboard
{
    class ClipboardUtility
    {
        public static string GetHighlightedTextFromForegroundWindow()
        {
            IntPtr foregroundWindowHandle = IntPtr.Zero;
            try
            {
                foregroundWindowHandle = GetForegroundWindow();
                if (foregroundWindowHandle == IntPtr.Zero)
                    return ""; // Could not get the foreground window handle

                SetForegroundWindow(foregroundWindowHandle); // Set the focus to the foreground window

                // Clear the clipboard to ensure fresh content is captured
                System.Windows.Clipboard.Clear();

                // Send CTRL+C using keybd_event
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0); // Press CTRL
                keybd_event(VK_C, 0, KEYEVENTF_KEYUP, 0); // Press CTRL
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, 0); // Press CTRL
                keybd_event(VK_C, 0, KEYEVENTF_KEYDOWN, 0); // Press C
                keybd_event(VK_C, 0, KEYEVENTF_KEYUP, 0); // Release C
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0); // Release CTRL
                Thread.Sleep(100);

                // Wait until clipboard contains text or a timeout occurs
                if (WaitForClipboardText(1000)) // Wait up to 1000 milliseconds
                    return System.Windows.Clipboard.GetText();

                return ""; // No text was copied
            }
            catch (Exception exp)
            {
                // Log or handle the error appropriately
                return ""; // Return empty string on exception
            }
        }

        public static void PasteTextAndRemoveDefaultText(int prefLength, ClipboardEntry entry)
        {
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);

            for (int i = 0; i < prefLength; i++)
            {
                keybd_event(VK_BACKSPACE, 0, KEYEVENTF_KEYDOWN, 0); 
                keybd_event(VK_BACKSPACE, 0, KEYEVENTF_KEYUP, 0);
            }

            ClipboardStorage.ignoreElement = entry;
            entry.pasteToClipboard();

            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(VK_V, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(VK_V, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
        }

        public static void PasteText(ClipboardEntry entry)
        {
            ClipboardStorage.ignoreElement = entry;
            entry.pasteToClipboard();

            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(VK_V, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(VK_V, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
        }

        public static void sendKeyDown(KeyboardHook.VKeys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_KEYDOWN, 0);
        }

        public static void sendKeyUp(KeyboardHook.VKeys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
        }


        private static bool WaitForClipboardText(int timeoutMillis)
        {
            int waited = 0;
            while (!System.Windows.Clipboard.ContainsText())
            {
                System.Threading.Thread.Sleep(10);
                waited += 10;
                if (waited > timeoutMillis)
                    return false;
            }
            return true;
        }


        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll", EntryPoint = "SendMessageW")]
        public static extern int SendMessageW([InAttribute] System.IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        public const int WM_GETTEXT = 13;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetFocus();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowThreadProcessId(int handle, out int processId);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern int AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);
        [DllImport("kernel32.dll")]
        internal static extern int GetCurrentThreadId();

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        const byte VK_CONTROL = 0x11;
        const byte VK_C = 0x43;
        const byte VK_V = 0x56;
        const byte VK_BACKSPACE = 0x08;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_KEYDOWN = 0x0000;



    }
}
