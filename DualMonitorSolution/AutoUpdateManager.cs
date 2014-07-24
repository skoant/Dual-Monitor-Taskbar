using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.Xml;
using System.Reflection;
using DualMonitor.Forms;
using System.Windows.Forms;

namespace DualMonitor
{
    public class AutoUpdateManager : IDisposable
    {       
        private const string VersionFile = "http://sites.google.com/site/dualmonitortb/version.xml";
        private const string ChangesFile = "http://sites.google.com/site/dualmonitortb/Changes.xml";
        
        private static AutoUpdateManager Instance = null;

        private System.Threading.Timer _timer;

        private bool _showingDialog = false;
        private Form _mainForm;

        public AutoUpdateManager(Form form)
        {
            _mainForm = form;
            _timer = new System.Threading.Timer(OnTimer, null, TimeSpan.FromSeconds(/*20*/10), TimeSpan.FromHours(5));
        }

        private void OnTimer(object state)
        {
            if (_showingDialog) return;

            CheckForUpdates();
        }        

        private void CheckForUpdates()
        {
            Version version = GetLatestOnlineVersion();
            if (version != null)
            {
                Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                if (version > currentVersion)
                {
                    _showingDialog = true;

                    string changes = ReadChangeListFile();

                    _mainForm.Invoke(new MethodInvoker(delegate()
                    {
                        NewVersionAlert nva = new NewVersionAlert(currentVersion, version, changes);
                        nva.ShowDialog(_mainForm);
                    }));

                    _showingDialog = false;

                    // stop if needed
                    AutoUpdateManager.Run(_mainForm);
                }
            }
        }

        private string ReadChangeListFile()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ChangesFile);
                request.Method = "GET";

                using (WebResponse response = request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(responseStream))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                return "Couldn't read the changelog file. See what's new here: https://sourceforge.net/projects/dualmonitortb/files/";
            }
        }

        private Version GetLatestOnlineVersion()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(VersionFile);
                request.Method = "GET";

                using (WebResponse response = request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        XmlDocument sr = new XmlDocument();
                        sr.Load(responseStream);

                        XmlNode versionNode = sr.SelectSingleNode("app/version");
                        string[] tokens = versionNode.InnerText.Split('.');

                        if (tokens.Length != 4)
                        {
                            return null;
                        }

                        return new Version(int.Parse(tokens[0]), int.Parse(tokens[1]),
                            int.Parse(tokens[2]), int.Parse(tokens[3]));                        
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        #endregion

        public static void Run(Form form)
        {
            if (TaskbarPropertiesManager.Instance.Properties.CheckForUpdates)
            {
                if (Instance == null)
                {
                    Instance = new AutoUpdateManager(form);
                }
            }
            else
            {
                if (Instance != null)
                {
                    Instance.Dispose();
                    Instance = null;
                }
            }
        }
    }
}
