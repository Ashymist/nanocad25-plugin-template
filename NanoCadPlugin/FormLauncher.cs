using System;
using System.Windows.Forms;

namespace NanoCadPlugin
{

    internal static class FormLauncher
    {
        private static PluginForm _instance;

        internal static void ShowWindow()
        {

            if (_instance != null && !_instance.IsDisposed)
            {
                if (_instance.WindowState == FormWindowState.Minimized)
                    _instance.WindowState = FormWindowState.Normal;
                _instance.Activate();
                _instance.BringToFront();
                return;
            }

            _instance = new PluginForm();
            _instance.FormClosed += delegate { _instance = null; };

            try
            {
                IntPtr handle = HostMgd.ApplicationServices.Application.MainWindow.Handle;
                _instance.Show(new OwnerWindow(handle));
            }
            catch
            {

                _instance.Show();
            }
        }

        private sealed class OwnerWindow : IWin32Window
        {
            public OwnerWindow(IntPtr handle)
            {
                Handle = handle;
            }

            public IntPtr Handle { get; private set; }
        }
    }
}
