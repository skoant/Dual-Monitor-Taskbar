using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DualMonitor.Entities
{
    [Serializable]
    public class Rule
    {
        public const string MOVE_MONITOR_WITH_CURSOR = "Monitor with mouse cursor";

        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public string Class { get; set; }

        public string Program { get; set; }

        public string Icon { get; set; }

        public bool UseCaption { get; set; }

        public bool UseProgram { get; set; }

        public bool UseClass { get; set; }

        public string Caption { get; set; }

        public string MoveAction { get; set; }

        public Guid Id { get; set; }

        internal Rule Clone()
        {
            return (Rule)this.MemberwiseClone();
        }
    }

    public enum RuleActionType
    {
        Move
    }
}
