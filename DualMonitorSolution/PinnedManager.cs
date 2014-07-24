using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Entities;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;

namespace DualMonitor
{
    public class PinnedManager
    {
        private List<PinnedApp> Apps { get; set; }

        private static PinnedManager _instance = new PinnedManager();
        public static PinnedManager Instance { get { return _instance; } }

        private string _userDataPath;

        private PinnedManager()
        {
            _userDataPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.ProductName, "Pinned") + "\\";

            Apps = new List<PinnedApp>();
        }

        /// <summary>
        /// Get a readonly list of pinned apps in second taskbar
        /// </summary>
        /// <returns></returns>
        public System.Collections.ObjectModel.ReadOnlyCollection<PinnedApp> GetApps()
        {
            return Apps.AsReadOnly();
        }

        /// <summary>
        /// Pin a program by executable filename
        /// </summary>
        public PinnedApp Pin(ProcessFullPath pfp)
        {
            var pa = GetPinned(pfp.FileName);
            if (pa == null)
            {
                pa = BuildPinnedApp(pfp.FileName, pfp.Arguments);
                Apps.Add(pa);
            }

            return pa;
        }       

        /// <summary>
        /// Pin a process
        /// </summary>
        public PinnedApp Pin(Process process)
        {
            var pa = GetPinned(process);
            if (pa == null)
            {
                pa = BuildPinnedApp(process);
                Apps.Add(pa);
            }

            return pa;
        }

        /// <summary>
        /// Unpin a process
        /// </summary>
        public PinnedApp Unpin(Process process)
        {
            var pa = GetPinned(process);

            if (pa != null)
            {
                Apps.Remove(pa);

                var name = System.Diagnostics.FileVersionInfo.GetVersionInfo(process.MainModule.FileName).FileDescription;
                var shortcut = FormatShortcutName(name);
                if (File.Exists(shortcut))
                {
                    File.Delete(shortcut);
                }
            }

            return pa;
        }

        /// <summary>
        /// Unpin by internal PinnedApp object. These objects are stored by PinnedManager
        /// </summary>
        public void Unpin(PinnedApp app)
        {
            Apps.Remove(app);

            if (File.Exists(app.Shortcut))
            {
                File.Delete(app.Shortcut);
            }
        }

        /// <summary>
        /// Find PinnedApp object by executable filename
        /// </summary>
        public PinnedApp GetPinned(string filename)
        {
            return Apps.Find(pa => pa.Path.Equals(filename, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Find PinnedApp object by process
        /// </summary>
        public PinnedApp GetPinned(Process process)
        {
            if (process.MainModule != null)
            {
                return GetPinned(process.MainModule.FileName);
            }
            return null;
        }

        public bool IsPinned(PinnedApp app)
        {
            return Apps.Exists(pa => pa.Path.Equals(app.Path, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsPinned(Process process)
        {
            if (process.MainModule != null)
            {
                return Apps.Exists(pa => pa.Path.Equals(process.MainModule.FileName, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        public void Load()
        {
            if (!Directory.Exists(_userDataPath))
            {
                return;
            }

            FileInfo[] files = new DirectoryInfo(_userDataPath).GetFiles("*.lnk");
            if (files.Length == 0)
            {
                return;
            }

            foreach (var file in files)
            {
                Apps.Add(CreateShortCutAndBuildApp(file.FullName));
            }

            // sort pinned apps (shortcuts) based on previously saved order
            Apps.Sort((x, y) =>
            {
                int xPos = TaskbarPropertiesManager.Instance.Properties.PinnedPrograms.IndexOf(x.Shortcut);
                int yPos = TaskbarPropertiesManager.Instance.Properties.PinnedPrograms.IndexOf(y.Shortcut);

                if (xPos < 0) xPos = int.MaxValue;
                if (yPos < 0) yPos = int.MaxValue;

                return xPos.CompareTo(yPos);
            });
        }

        private PinnedApp CreateShortCutAndBuildApp(string fullName)
        {
            Bitmap icon;
            string displayName;
            string targetPath;
            string arguments;
            ShortcutUtil.ParseShortcut(fullName, out icon, out displayName, out targetPath, out arguments);

            return new PinnedApp
            {
                Path = targetPath,
                Arguments = arguments,
                Name = displayName,
                Icon = icon,
                Shortcut = fullName
            };
        }

        private PinnedApp BuildPinnedApp(string filename, string arguments)
        {
            var name = System.Diagnostics.FileVersionInfo.GetVersionInfo(filename).FileDescription;
            var path = FormatShortcutName(name);

            if (!Directory.Exists(_userDataPath))
            {
                Directory.CreateDirectory(_userDataPath);
            }

            ShortcutUtil.CreateShortcut(filename, arguments, path);

            return CreateShortCutAndBuildApp(path);
        }

        private PinnedApp BuildPinnedApp(Process process)
        {            
            var fi = new FileInfo(process.MainModule.FileName);
            var name = System.Diagnostics.FileVersionInfo.GetVersionInfo(process.MainModule.FileName).FileDescription;
            var path = FormatShortcutName(name);

            if (!Directory.Exists(_userDataPath))
            {
                Directory.CreateDirectory(_userDataPath);
            }

            // try to get full path including arguments
            string arguments = ProcessUtil.GetCommandLineArguments(process.MainModule.ModuleName);
            
            ShortcutUtil.CreateShortcut(process.MainModule.FileName, arguments, path);

            return CreateShortCutAndBuildApp(path);
        }

        private string FormatShortcutName(string name)
        {
            FixName(ref name);
            return _userDataPath + name + ".lnk";
        }

        private void FixName(ref string name)
        {
            name = new Regex(@"[\\\/\:\,\*\?\""\<\>\|]").Replace(name, "_");
        }        
    }
}
