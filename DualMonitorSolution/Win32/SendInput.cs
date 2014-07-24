using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DualMonitor.Win32
{
    internal class SendInputApi
    {

        /// <summary>
        /// The mouse data structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct MouseInputData
        {
            /// <summary>
            /// The x value, if ABSOLUTE is passed in the flag then this is an actual X and Y value
            /// otherwise it is a delta from the last position
            /// </summary>
            internal int dx;
            /// <summary>
            /// The y value, if ABSOLUTE is passed in the flag then this is an actual X and Y value
            /// otherwise it is a delta from the last position
            /// </summary>
            internal int dy;
            /// <summary>
            /// Wheel event data, X buttons
            /// </summary>
            internal uint mouseData;
            /// <summary>
            /// ORable field with the various flags about buttons and nature of event
            /// </summary>
            internal MouseEventFlags dwFlags;
            /// <summary>
            /// The timestamp for the event, if zero then the system will provide
            /// </summary>
            internal uint time;
            /// <summary>
            /// Additional data obtained by calling app via GetMessageExtraInfo
            /// </summary>
            internal IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            internal ushort wVk;
            internal ushort wScan;
            internal uint dwFlags;
            internal uint time;
            internal IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            internal int uMsg;
            internal short wParamL;
            internal short wParamH;
        }

        /// <summary>
        /// Captures the union of the three three structures.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        internal struct MouseKeybdhardwareInputUnion
        {
            /// <summary>
            /// The Mouse Input Data
            /// </summary>
            [FieldOffset(0)]
            internal MouseInputData mi;

            /// <summary>
            /// The Keyboard input data
            /// </summary>
            [FieldOffset(0)]
            internal KEYBDINPUT ki;

            /// <summary>
            /// The hardware input data
            /// </summary>
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }

        /// <summary>
        /// The Data passed to SendInput in an array.
        /// </summary>
        /// <remarks>Contains a union field type specifies what it contains </remarks>
        [StructLayout(LayoutKind.Sequential)]
        internal struct INPUT
        {
            /// <summary>
            /// The actual data type contained in the union Field
            /// </summary>
            internal SendInputEventType type;
            internal MouseKeybdhardwareInputUnion mkhi;
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr GetMessageExtraInfo();

        /// <summary>
        /// The event type contained in the union field
        /// </summary>
        internal enum SendInputEventType : int
        {
            /// <summary>
            /// Contains Mouse event data
            /// </summary>
            InputMouse,
            /// <summary>
            /// Contains Keyboard event data
            /// </summary>
            InputKeyboard,
            /// <summary>
            /// Contains Hardware event data
            /// </summary>
            InputHardware
        }

        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_SCANCODE = 0x0008;

        /// <summary>
        /// Used in mouseData if XDOWN or XUP specified
        /// </summary>
        [Flags]
        enum MouseDataFlags : uint
        {
            /// <summary>
            /// First button was pressed or released
            /// </summary>
            XBUTTON1 = 0x0001,
            /// <summary>
            /// Second button was pressed or released
            /// </summary>
            XBUTTON2 = 0x0002
        }

        /// <summary>
        /// The flags that a MouseInput.dwFlags can contain
        /// </summary>
        [Flags]
        internal enum MouseEventFlags : uint
        {
            /// <summary>
            /// Movement occured
            /// </summary>
            MOUSEEVENTF_MOVE = 0x0001,
            /// <summary>
            /// button down (pair with an up to create a full click)
            /// </summary>
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            /// <summary>
            /// button up (pair with a down to create a full click)
            /// </summary>
            MOUSEEVENTF_LEFTUP = 0x0004,
            /// <summary>
            /// button down (pair with an up to create a full click)
            /// </summary>
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            /// <summary>
            /// button up (pair with a down to create a full click)
            /// </summary>
            MOUSEEVENTF_RIGHTUP = 0x0010,
            /// <summary>
            /// button down (pair with an up to create a full click)
            /// </summary>
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            /// <summary>
            /// button up (pair with a down to create a full click)
            /// </summary>
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            /// <summary>
            /// button down (pair with an up to create a full click)
            /// </summary>
            MOUSEEVENTF_XDOWN = 0x0080,
            /// <summary>
            /// button up (pair with a down to create a full click)
            /// </summary>
            MOUSEEVENTF_XUP = 0x0100,
            /// <summary>
            /// Wheel was moved, the value of mouseData is the number of movement values
            /// </summary>
            MOUSEEVENTF_WHEEL = 0x0800,
            /// <summary>
            /// Map X,Y to entire desktop, must be used with MOUSEEVENT_ABSOLUTE
            /// </summary>
            MOUSEEVENTF_VIRTUALDESK = 0x4000,
            /// <summary>
            /// The X and Y members contain normalised Absolute Co-Ords. If not set X and Y are relative
            /// data to the last position (i.e. change in position from last event)
            /// </summary>
            MOUSEEVENTF_ABSOLUTE = 0x8000
        }

        /// <summary>
        /// SendsInput using Win32 API Function
        /// </summary>
        /// <param name="nInputs">The number input structures in the array.</param>
        /// <param name="pInputs">The pointer to array of input structures.</param>
        /// <param name="cbSize">Size of the structure in the array.</param>
        /// <returns>The function returns the number of events that it successfully
        /// inserted into the keyboard or mouse input stream. If the function returns zero,
        /// the input was already blocked by another thread. To get extended error information,
        /// call GetLastError.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        internal static void DoMouse(MouseEventFlags mouseEventFlags, System.Drawing.Point point)
        {
            INPUT input = new INPUT();
            MouseInputData mi = new MouseInputData();
            input.type = SendInputEventType.InputMouse;
            input.mkhi.mi = mi;
            input.mkhi.mi.dwExtraInfo = IntPtr.Zero;
            input.mkhi.mi.dx = point.X * (65536 / Screen.PrimaryScreen.Bounds.Width);
            input.mkhi.mi.dy = point.Y * (65536 / Screen.PrimaryScreen.Bounds.Height);
            input.mkhi.mi.time = 0;
            input.mkhi.mi.mouseData = 0;
            input.mkhi.mi.dwFlags = mouseEventFlags;
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            SendInput(1, ref input, cbSize);            
        }
    }
}
