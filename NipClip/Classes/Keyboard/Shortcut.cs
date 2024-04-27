using NipClip.Classes.Clipboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NipClip.Classes.Keyboard
{
    public class Shortcut
    {
        public virtual List<List<KeyboardHook.VKeys>> Keys { get; set; }

        public virtual List<MainWindow> mainWindows { get; set; }

        public virtual bool isControlLeaderKey(ref List<KeyboardHook.VKeys> keys)
        {
            foreach (KeyboardHook.VKeys key in keys)
            {
                if (key.Equals(WindowManager.applicationSettings.leaderKey))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool Callback(List<KeyboardHook.VKeys> activationKeys)
        {

            return true;
        }
    }
}
