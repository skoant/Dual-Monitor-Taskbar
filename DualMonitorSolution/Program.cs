using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using DualMonitor.Forms;
using System.IO;
using DualMonitor.VisualStyle;
using System.Reflection;
using System.Drawing;

namespace DualMonitor
{
    static class Program
    {
        private static NotifyIcon _icon;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Hook fatal abort handler
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);            
            
            for (int retry = 0; retry < 2; retry++)
            {
                // single instance allowed
                bool createdNew = true;
                using (Mutex mutex = new Mutex(true, "DualMonitor", out createdNew))
                {
                    if (createdNew)
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(true);

                        if (Screen.AllScreens.Length == 1)
                        {
                            TaskbarPropertiesManager.Instance.Load();
                            ShowOneScreenWarning();

                            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged_Restart);
                            Application.Run();
                        }
                        else
                        {
                            TaskbarPropertiesManager.Instance.Load();
                            AeroDecorator.Instance.Init();
                            PinnedManager.Instance.Load();

                            Microsoft.Win32.SystemEvents.DisplaySettingsChanging += new EventHandler(SystemEvents_DisplaySettingsChanging);
                            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

                            MultiMonitorManager.Start();

                            WindowManager.Instance.MoveProgramsToTaskbars();
                            
                            Application.Run(new DualMonitorForm());
                        }

                        break;
                    }
                    else
                    {
                        // after Application.Restart, we should give the system some time to delete the mutex...
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            }
        }

        #region System events
        static void SystemEvents_DisplaySettingsChanging(object sender, EventArgs e)
        {
            ProcessMonitor.Instance.PauseMonitoring();
        }

        static void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            if (Screen.AllScreens.Length == 1)
            {
                Application.Restart();
            }
            else
            {
                WindowManager.Instance.MoveProgramsToTaskbars();
                ProcessMonitor.Instance.ResumeMonitoring();
            }
        }
        #endregion

        private static void ShowOneScreenWarning()
        {
            _icon = new NotifyIcon();
            _icon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            _icon.Visible = true;
            ContextMenuStrip menu = new ContextMenuStrip();

            _icon.DoubleClick += new EventHandler(menu_DoubleClick);

            ToolStripMenuItem properties = new ToolStripMenuItem();
            properties.Text = "Properties";
            properties.Click += new EventHandler(properties_Click);
            properties.Font = new Font(properties.Font, FontStyle.Bold);
            menu.Items.Add(properties);

            ToolStripMenuItem exit = new ToolStripMenuItem();
            exit.Text = "Exit";
            exit.Click += new EventHandler(exit_Click);
            menu.Items.Add(exit);

            _icon.ContextMenuStrip = menu;

            _icon.ShowBalloonTip(30000, "Dual Monitor Taskbar",
                "Can not detect a second monitor. When it becomes available, the second taskbar will appear.", ToolTipIcon.Info);
        }

        static void menu_DoubleClick(object sender, EventArgs e)
        {
            properties_Click(sender, e);
        }

        static void properties_Click(object sender, EventArgs e)
        {
            var propertiesWindow = new PropertiesWindow(null);
            propertiesWindow.StartPosition = FormStartPosition.CenterScreen;
            propertiesWindow.InitializeWithProperties();
            propertiesWindow.BringToFront();
            propertiesWindow.ShowDialog();
        }

        static void exit_Click(object sender, EventArgs e)
        {
            HideIcon();            
            Application.Exit();
        }

        static void HideIcon()
        {
            _icon.Visible = false;
            _icon.Dispose();
        }

        static void SystemEvents_DisplaySettingsChanged_Restart(object sender, EventArgs e)
        {
            if (Screen.AllScreens.Length != 1)
            {
                HideIcon();                
                Application.Restart();
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {            
            string dump = string.Format("DualMonitor-{0:yyMMddHH}.log", DateTime.Now);
            string path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), dump);
            Version curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;  

            using (var s = new FileStream(path, FileMode.Append))
            {
                using (var sw = new StreamWriter(s))
                {
                    sw.WriteLine("[{0:yyyy-MM-dd HH:mm:ss}] Dual Monitor Crash Dump {1}", DateTime.Now, curVersion.ToString());
                    sw.WriteLine();
                    sw.WriteLine("Last exception:");
                    sw.WriteLine(e.ExceptionObject.ToString());
                    sw.WriteLine();
                    sw.WriteLine("OS: {0}", Environment.OSVersion.ToString());
                    sw.WriteLine(".NET: {0}", Environment.Version.ToString());
                    sw.WriteLine("Aero: {0}", AeroDecorator.Instance.IsDwmCompositionEnabled);
                    sw.WriteLine("Launch command: {0}", Environment.CommandLine);
                    sw.WriteLine("UTC time: {0} {1}", DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToShortTimeString());
                }
            }
        }
    }
}
