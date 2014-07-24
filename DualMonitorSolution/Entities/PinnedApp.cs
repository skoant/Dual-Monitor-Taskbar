using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DualMonitor.Entities
{
    [Serializable]
    public class PinnedApp
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Arguments { get; set; }

        public System.Drawing.Bitmap Icon { get; set; }

        public string Shortcut { get; set; }
    }
}
