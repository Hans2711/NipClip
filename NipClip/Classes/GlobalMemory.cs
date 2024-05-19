using NipClip.Classes.Keyboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NipClip.Classes
{
    public class GlobalMemory
    {
        public static HashSet<KeyboardHook.VKeys> supressAllKeyDownEvents = new HashSet<KeyboardHook.VKeys>();
        public static HashSet<KeyboardHook.VKeys> supressOnceKeyUpEvent = new HashSet<KeyboardHook.VKeys>();
    }
}
