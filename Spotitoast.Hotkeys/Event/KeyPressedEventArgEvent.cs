using System;

namespace Spotitoast.HotKeys.Event
{
    /// <summary>
    ///     Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class KeyPressedEventArgs : EventArgs
    {
        public KeyPressedEventArgs(Model.HotKeys hotKeys)
        {
            HotKeys = hotKeys;
        }

        public Model.HotKeys HotKeys { get; set; }
    }
}