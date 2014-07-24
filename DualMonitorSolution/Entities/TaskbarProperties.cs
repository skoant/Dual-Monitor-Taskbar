using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DualMonitor.Win32;
using System.Windows.Forms;
using System.Collections;

namespace DualMonitor.Entities
{
    [Serializable]
    public class TaskbarProperties
    {
        [Serializable]
        public class TaskbarLocation
        {
            public string DeviceName;
            public Native.ABEdge Location;

            public TaskbarLocation() {}
            public TaskbarLocation(string deviceName, Native.ABEdge location)
            {
                DeviceName = deviceName;
                Location = location;
            }
        }

        [Flags]
        public enum ChangedFields
        {
            None = 0,
            MirrorButtons = 1,
            AutoHide = 2
        }

        public bool ShowLabels { get; set; }
        public bool ShowNotificationArea { get; set; }
        public bool ShowClock { get; set; }
        public bool ShowStartMenu { get; set; }
        public bool AutoHide { get; set; }
        public int AutoHideDelay { get; set; }
        public List<string> PinnedPrograms { get; set; }
        public int HeightMultiplier { get; set; }
        public bool CheckForUpdates { get; set; }
        public int Width { get; set; }
        public bool Locked { get; set; }
        public bool SmallIcons { get; set; }
        public bool MirrorButtons { get; set; }
        public List<TaskbarLocation> ScreenLocation { get; set; }

        public bool UseCustomFont { get; set; }
        public CustomFont CustomFont { get; set; }

        public List<Rule> Rules { get; set; }

        public string Version { get; set; }

        public TaskbarProperties()
        {
            Version = null;
            ShowLabels = true;
            ShowClock = true;
            AutoHide = false;
            AutoHideDelay = 600;
            CheckForUpdates = true;
            ShowNotificationArea = true;
            ShowStartMenu = false;
            SmallIcons = true;
            MirrorButtons = false;
            HeightMultiplier = 1;
            Width = Constants.VerticalTaskbarWidth;
            Locked = false;
            ScreenLocation = new List<TaskbarLocation>();
            Rules = new List<Rule>();
            PinnedPrograms = new List<string>();

            UseCustomFont = false;
            CustomFont = new Entities.CustomFont
            {
                Family = System.Drawing.SystemFonts.IconTitleFont.FontFamily.Name,
                Size = System.Drawing.SystemFonts.IconTitleFont.Size,
                Style = (int)System.Drawing.SystemFonts.IconTitleFont.Style,
                Color = (CustomColor)System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ControlText)
            };
        }

        public Native.ABEdge GetTaskbarLocation(string deviceName)
        {
            Native.ABEdge location;
            int index = ScreenLocation.FindIndex(kvp => kvp.DeviceName.Equals(deviceName));
            if (index < 0)
            {
                location = Native.ABEdge.Bottom;
                ScreenLocation.Add(new TaskbarLocation(deviceName, location));
            }
            else
            {
                location = ScreenLocation[index].Location;
            }
            return location;
        }

        public void SetTaskbarLocation(string deviceName, Native.ABEdge location)
        {
             int index = ScreenLocation.FindIndex(kvp => kvp.DeviceName.Equals(deviceName));
             if (index < 0)
             {
                 ScreenLocation.Add(new TaskbarLocation(deviceName, location));
             }
             else
             {
                 ScreenLocation[index] = new TaskbarLocation(deviceName, location);
             }
        }
    }
}
