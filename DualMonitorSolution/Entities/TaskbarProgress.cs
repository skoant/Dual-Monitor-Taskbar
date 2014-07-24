using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DualMonitor.Entities
{
    public class TaskbarProgress
    {
        public TaskbarProgressState State { get; set; }
        public int Value { get; set; }

        public TaskbarProgress()
        {
            State = TaskbarProgressState.NoProgress;
            Value = -1;
        }
    }

    public enum TaskbarProgressState
    {
        // Summary:
        //     No progress is displayed.
        NoProgress = 0,
        //
        // Summary:
        //     The progress is indeterminate (marquee).
        Indeterminate = 1,
        //
        // Summary:
        //     Normal progress is displayed.
        Normal = 2,
        //
        // Summary:
        //     An error occurred (red).
        Error = 4,
        //
        // Summary:
        //     The operation is paused (yellow).
        Paused = 8,
    }
}
