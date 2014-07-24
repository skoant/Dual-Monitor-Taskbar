using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IWshRuntimeLibrary;
using System.Drawing;
using System.Runtime.InteropServices;
using DualMonitor.Win32;
using System.Text.RegularExpressions;
using System.IO;

namespace DualMonitor
{
    public class ShortcutUtil
    {
        /// <summary>
        /// Create Windows Shorcut
        /// </summary>
        /// <param name="SourceFile">A file you want to make shortcut to</param>
        /// <param name="ShortcutFile">Path and shorcut file name including file extension (.lnk)</param>
        /// <param name="Description">Shortcut description</param>
        /// <param name="Arguments">Command line arguments</param>
        /// <param name="HotKey">Shortcut hot key as a string, for example "Ctrl+F"</param>
        /// <param name="WorkingDirectory">"Start in" shorcut parameter</param>
        public static void CreateShortcut(string SourceFile, string arguments, string ShortcutFile)
        {
            // Check necessary parameters first:
            if (String.IsNullOrEmpty(SourceFile))
                throw new ArgumentNullException("SourceFile");
            if (String.IsNullOrEmpty(ShortcutFile))
                throw new ArgumentNullException("ShortcutFile");

            // Create WshShellClass instance:
            IWshShell3 wshShell =  new WshShellClass();

            // Create shortcut object:
            IWshRuntimeLibrary.IWshShortcut shorcut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(ShortcutFile);

            shorcut.Arguments = arguments;

            // Assign shortcut properties:
            shorcut.TargetPath = SourceFile;
            
            // Save the shortcut:
            shorcut.Save();
        }

        public static void ParseShortcut(string filename, out Bitmap icon, out string displayName, out string targetPath, out string arguments)
        {
            Native.SHFILEINFO info = new Native.SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            Native.SHGFI flags;
            flags = Native.SHGFI.Icon | Native.SHGFI.DisplayName | Native.SHGFI.SysIconIndex;

            IntPtr sysImageList = Native.SHGetFileInfo(filename, 0, ref info, (uint)cbFileInfo, (uint)flags);
            displayName = info.szDisplayName;

            if (sysImageList != IntPtr.Zero && info.iIcon != 0)
            {
                icon = Icon.FromHandle(Native.ImageList_GetIcon(sysImageList, info.iIcon, 0)).ToBitmap();
            }
            else
            {
                throw new Exception("Cannot pin !");
            }

            // Create WshShellClass instance:
            IWshShell3 wshShell = new WshShellClass();
            IWshRuntimeLibrary.IWshShortcut shorcut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(filename);
            targetPath = shorcut.TargetPath;

            if (!string.IsNullOrWhiteSpace(shorcut.Arguments))
            {
                arguments = shorcut.Arguments;
            }
            else
            {
                arguments = string.Empty;
            }
        }       
    }
}
