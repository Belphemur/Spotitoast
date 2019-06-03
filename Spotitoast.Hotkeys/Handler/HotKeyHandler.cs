using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Spotitoast.HotKeys.Handler
{
    public class HotKeyHandler : Form
    {

        private const int WM_HOTKEY = 0x0312;
        private static HotKeyHandler _instance;
        private static ThreadExceptionEventHandler _exceptionEventHandler;
        private readonly Dictionary<Model.HotKeys, int> _registeredHotkeys = new Dictionary<Model.HotKeys, int>();
        private int _hotKeyId;

        private static readonly ISubject<Model.HotKeys> _hotkeyPressedSubject = new Subject<Model.HotKeys>();

        public static IObservable<Model.HotKeys> HotKeyPressed => _hotkeyPressedSubject.AsObservable();


        private HotKeyHandler()
        {

        }

        /// <summary>
        ///     Start the Adapter thread
        /// </summary>
        public static void Start(ThreadExceptionEventHandler exceptionEventHandler = null)
        {
            if (_instance != null)
                throw new InvalidOperationException("Adapter already started");

            _exceptionEventHandler = exceptionEventHandler;

            var t = new Thread(RunForm) { Name = typeof(HotKeyHandler).Name };
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        ///     Stop the adapter thread
        /// </summary>
        public static void Stop()
        {
            if (_instance == null)
                throw new InvalidOperationException("Adapter not started");


            if (_instance.IsDisposed) return;

            try
            {
                _instance.Invoke(new MethodInvoker(_instance.EndForm));
            }
            catch (Exception ex)
            {
                //Can happen when the instance got dispose in its own thread
                //when in the same time the Application thread call the Stop() method.
                Trace.WriteLine("Thread Race Condition: " + ex);
            }
        }

        private static void RunForm()
        {

            Trace.WriteLine("Starting WindowsAPIAdapter thread");
            if (_exceptionEventHandler != null)
                Application.ThreadException += _exceptionEventHandler;

            _instance = new HotKeyHandler();
            _instance.CreateHandle();
            Trace.WriteLine("Handle created. Running the application.");
            Application.Run(_instance);
            Trace.WriteLine("End of the WindowsAPIAdapter thread");

        }

        private void EndForm()
        {
            Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_exceptionEventHandler != null)
                    Application.ThreadException -= _exceptionEventHandler;
                foreach (var hotKeyId in _registeredHotkeys.Values)
                {
                    NativeMethods.UnregisterHotKey(_instance.Handle, hotKeyId);
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///     Registers a HotKey in the system.
        /// </summary>
        /// <param name="hotKeys">Represent the hotkey to register</param>
        public static bool RegisterHotKey(Model.HotKeys hotKeys)
        {
            var count = 0;
            while (_instance == null)
            {
                Thread.Sleep(250);
                if (count++ > 3)
                {
                    throw new ThreadStateException("Instance isn't set even after waiting 750 ms");
                }
            }

            lock (_instance)
            {
                return (bool)_instance.Invoke(new Func<bool>(() =>
                {

                    Trace.WriteLine($"Registering Hotkeys: {hotKeys}");
                    if (_instance._registeredHotkeys.ContainsKey(hotKeys))
                    {

                        Trace.WriteLine($"Already registered Hotkeys: {hotKeys}");
                        return false;
                    }

                    var id = _instance._hotKeyId++;
                    _instance._registeredHotkeys.Add(hotKeys, id);
                    // register the hot key.
                    return NativeMethods.RegisterHotKey(_instance.Handle, id, (uint)hotKeys.Modifier,
                       (uint)hotKeys.Key);

                }));
            }

        }

        /// <summary>
        ///     Unregister a registered HotKey
        /// </summary>
        /// <param name="hotKeys"></param>
        /// <returns></returns>
        public static bool UnRegisterHotKey(Model.HotKeys hotKeys)
        {
            var count = 0;
            while (_instance == null)
            {
                Thread.Sleep(250);
                if (count++ >= 2)
                {
                    throw new ThreadStateException("Instance isn't set even after waiting 750 ms");
                }
            }

            lock (_instance)
            {
                return (bool)_instance.Invoke(new Func<bool>(() =>
                {
                    Trace.WriteLine($"Unregistering Hotkeys: {hotKeys}");
                    if (!_instance._registeredHotkeys.TryGetValue(hotKeys, out var id))
                    {
                        Trace.WriteLine($"Not registered Hotkeys: {hotKeys}");
                        return false;
                    }
                    _instance._registeredHotkeys.Remove(hotKeys);
                    return NativeMethods.UnregisterHotKey(_instance.Handle, id);
                }));
            }
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false);
        }

        protected override void WndProc(ref Message m)
        {
            //Check for shutdown message from windows
            if (m.Msg == WM_HOTKEY) ProcessHotKeyEvent(m);

            base.WndProc(ref m);
        }

        /// <summary>
        ///     To avoid overflow on 64 bit platform use this method
        /// </summary>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private long ConvertLParam(IntPtr lParam)
        {
            try
            {
                return lParam.ToInt32();
            }
            catch (OverflowException)
            {
                return lParam.ToInt64();
            }
        }

        private void ProcessHotKeyEvent(Message m)
        {
            var key = (Model.HotKeys.Keys)((ConvertLParam(m.LParam) >> 16) & 0xFFFF);
            var modifier = (Model.HotKeys.ModifierKeys)(ConvertLParam(m.LParam) & 0xFFFF);

            _hotkeyPressedSubject.OnNext(new Model.HotKeys(key, modifier));

        }



        #region WindowsNativeMethods

        private static class NativeMethods
        {
            // Registers a hot key with Windows.
            [DllImport("user32.dll")]
            internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

            // Unregisters the hot key with Windows.
            [DllImport("user32.dll")]
            internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        }

        #endregion
    }
}