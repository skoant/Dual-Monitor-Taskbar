using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using DualMonitor.Entities;
using Microsoft.Win32;

namespace DualMonitor
{
    public class TaskbarPropertiesManager
    {
        private static TaskbarPropertiesManager _instance = new TaskbarPropertiesManager();
        public static TaskbarPropertiesManager Instance { get { return _instance; } }

        public TaskbarProperties Properties { get; private set; }
        private string _userDataPath;
        private string _userDataFile;

        private const string FileName = "config.xml";

        private TaskbarPropertiesManager()
        {
            _userDataPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.ProductName);

            _userDataFile = Path.Combine(_userDataPath, FileName);

            Properties = new TaskbarProperties();
        }

        public void Load()
        {
            if (File.Exists(_userDataFile))
            {
                using (StreamReader sr = new StreamReader(_userDataFile))
                {
                    try
                    {
                        System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(typeof(TaskbarProperties));
                        Properties = xml.Deserialize(sr) as TaskbarProperties;
                    }
                    catch
                    {
                        // properties file may be corrupt, ignore it
                    }
                }

                UpgradeRulesFromDualMonitorVersion(Properties);
            }
            else
            {
                Properties.SmallIcons = 
                    RegistryProxy.GetKey<int>("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\", "TaskbarSmallIcons") != 0;
                Properties.ShowLabels = 
                    RegistryProxy.GetKey<int>("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\", "TaskbarGlomLevel") != 0;
            }
        }

        /// <summary>
        /// Upgrade rules from DualMonitor version to MultiMonitor (1.22)
        /// </summary>
        /// <param name="Properties"></param>
        private void UpgradeRulesFromDualMonitorVersion(TaskbarProperties Properties)
        {
            if (Properties == null || Properties.Rules == null) return;

            if (!string.IsNullOrEmpty(Properties.Version)) return;

            foreach (var rule in Properties.Rules)
            {
                if (rule.MoveAction.Equals("Monitor1"))
                {
                    rule.MoveAction = Screen.PrimaryScreen.DeviceName;
                } 
                else if (rule.MoveAction.Equals("Monitor2")) 
                {
                    var secondary = Screen.AllScreens.FirstOrDefault(s => !s.Primary);
                    if (secondary != null)
                    {
                        rule.MoveAction = secondary.DeviceName;
                    }
                }
                else
                {
                    rule.MoveAction = Rule.MOVE_MONITOR_WITH_CURSOR;
                }
            }

            Properties.Version = "1.22";

            Save();
        }

        public void Save()
        {
            if (!Directory.Exists(_userDataPath)) Directory.CreateDirectory(_userDataPath);

            using (StreamWriter sr = new StreamWriter(_userDataFile, false))
            {
                try
                {
                    XmlSerializer xml = new XmlSerializer(typeof(TaskbarProperties));
                    xml.Serialize(sr, Properties);
                }
                catch
                {
                }
            }
        }
    }
}
