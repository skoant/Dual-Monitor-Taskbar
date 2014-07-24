using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DualMonitor.Win32;

namespace DualMonitor.Forms
{
    public partial class SelectProgram : Form
    {
        private List<IntPtr> _parentHandles;

        public class ProgramData
        {
            public IntPtr HWND;
            public string Caption;
            public string FileName;
            public Icon Icon;
        }

        public ProgramData SelectedWindow
        {
            get
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    return listView1.SelectedItems[0].Tag as ProgramData;
                }
                return null;
            }
        }

        public SelectProgram(List<IntPtr> parentHandles)
        {
            _parentHandles = parentHandles;
            InitializeComponent();
        }

        private void SelectProgram_Load(object sender, EventArgs e)
        {
            listView1.SmallImageList = new ImageList();

            Native.EnumWindows(new Native.EnumDelegate(delegate(IntPtr hwnd, int lParam)
            {
                string fileName;
                if (Native.IsAltTabVisible(hwnd)
                    && Native.IsWindowValidForFinder(ref hwnd, _parentHandles, out fileName))
                {
                    
                    ProgramData pd = new ProgramData()
                    {
                        FileName = fileName,
                        Caption = Native.GetWindowText(hwnd),
                        HWND = hwnd,
                        Icon = Native.GetIcon(hwnd)
                    };

                    ListViewItem lvi = new ListViewItem(new string[] { "", pd.Caption, pd.FileName });
                    lvi.Tag = pd;

                    if (pd.Icon != null)
                    {
                        listView1.SmallImageList.Images.Add(pd.Icon.ToBitmap());
                        lvi.ImageIndex = listView1.SmallImageList.Images.Count - 1;
                    }

                    listView1.Items.Add(lvi);
                }
                return true;
            }), IntPtr.Zero);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelect.Enabled = listView1.SelectedItems.Count > 0;
        }

    }
}
