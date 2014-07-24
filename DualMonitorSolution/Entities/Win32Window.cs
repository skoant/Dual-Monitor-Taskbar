using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using DualMonitor.Win32;
using System.Threading;

namespace DualMonitor.Entities
{
    public class Win32Window : IWin32Window
    {
        protected Win32Window(IntPtr handle)
        {
            this._handle = handle;
        }

        public static Win32Window FromHandle(IntPtr handle)
        {
            return new Win32Window(handle);
        }

        public static Win32Window FromClassName(string className)
        {
            IntPtr hwnd = Native.FindWindow(className, null);
            return Win32Window.FromHandle(hwnd);
        }

        protected IntPtr _handle;
        protected string _title;
        protected string _path;
        protected string _arguments;
        protected Icon _icon;
        protected Icon _smallIcon;
        protected string _className;

        public string Title
        {
            get
            {
                if (_title == null)
                {
                    _title = Native.GetWindowTextWithTimeout(Handle, 500);
                }
                return _title;
            }
        }

        public string Path
        {
            get
            {
                if (_path == null)
                {
                    _path = ProcessUtil.GetProcessPathByWindowHandle(Handle);
                }
                return _path;
            }
        }

        public string Arguments
        {
            get
            {
                if (_arguments == null)
                {
                    _arguments = ProcessUtil.GetCommandLineArguments(Handle);
                }
                return _arguments;
            }
        }

        public Icon Icon
        {
            get
            {
                if (_icon == null)
                {
                    try
                    {
                        _icon = Native.GetIconWithTimeout(Handle, 500);
                    }
                    catch
                    {
                        _icon = null;
                    }
                }
                return _icon;
            }
        }

        public Icon SmallIcon
        {
            get
            {
                if (_smallIcon == null)
                {
                    _smallIcon = Native.GetSmallIconWithTimeout(Handle, 500);                        

                    if (_smallIcon == null)
                    {
                        _smallIcon = Icon;
                    }
                }
                return _smallIcon;
            }
        }

        public string ClassName
        {
            get
            {
                if (_className == null)
                {
                    _className = Native.GetClassName(Handle);
                }

                return _className;
            }
        }

        public IntPtr Handle
        {
            get { return _handle; }
        }        

        public bool IsMinimized
        {
            get
            {
                return Native.IsIconic(this.Handle);
            }
        }

        public Screen Screen
        {
            get
            {
                return Screen.FromHandle(Handle);
            }
        }

        public Rectangle Bounds
        {
            get
            {
                Native.RECT r;
                Native.GetWindowRect(this.Handle, out r);

                return (Rectangle)r;
            }
        }

        public void ActivateWindow(bool toggle)
        {
            if (IsMinimized)
            {
                Native.ShowWindowAsync(Handle, Native.ShowWinCmdShow);
                Native.ShowWindowAsync(Handle, Native.ShowWinCmdRestore);
                Native.SetForegroundWindow(Handle);
            }
            else
            {
                if (toggle)
                {
                    Native.ShowWindowAsync(Handle, Native.ShowWinCmdMinimized);
                }
                else
                {
                    Native.SetForegroundWindow(Handle);
                }
            }
        }
     
        public void MoveWindowTo(Screen screen)
        {
            if (!this.Screen.Equals(screen))
            {
                Native.RECT r;
                Native.GetWindowRect(Handle, out r);

                int dx = r.left - Screen.PrimaryScreen.Bounds.Left;
                int dy = r.top - Screen.PrimaryScreen.Bounds.Top;

                Native.MoveWindow(Handle, screen.Bounds.Left + dx, screen.Bounds.Top + dy, r.right - r.left, r.bottom - r.top, true);
            }
        }

        public Win32Window FindWindow(string className)
        {
            if (this.Handle == IntPtr.Zero)
            {
                return Win32Window.FromHandle(IntPtr.Zero);
            }

            IntPtr hwnd = Native.FindWindowEx(this.Handle, IntPtr.Zero, className, null);
            return Win32Window.FromHandle(hwnd);
        }

        public void Minimize()
        {
            Native.ShowWindowAsync(this.Handle, Native.ShowWinCmdMinimized);
        }

        public void Restore()
        {
            Native.ShowWindowAsync(this.Handle, Native.ShowWinCmdRestore);
        }
/*
        private T CallNativeWithTimeout<T>(Func<T> func, int timeout, T def)
        {
            T result = def;

            Console.WriteLine("New thread");

            Thread t = new Thread(delegate()
            {
                result = func();
            });

            t.Start();

            if (!t.Join(timeout))
            {
                t.Abort();
            }

            return result;
        }
 */
    }
}
