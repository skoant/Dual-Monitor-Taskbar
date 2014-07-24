using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace DualMonitor.Win32
{
    /// <summary>
    /// APPDOCLISTTYPE.  ADLT_*.
    /// </summary>
    public enum ADLT
    {
        RECENT = 0,   // The recently used documents list
        FREQUENT,     // The frequently used documents list
    }

    /// <summary>
    /// Allows an application to retrieve the most recent and frequent documents opened in that app, as reported via SHAddToRecentDocs
    /// </summary>
    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid("3c594f9f-9f30-47a1-979a-c9e83d3d0a06")
    ]
    public interface IApplicationDocumentLists
    {
        /// <summary>
        /// Set the App User Model ID for the application retrieving this list.  If an AppID is not provided via this method,
        /// the system will use a heuristically determined ID.  This method must be called before GetList. 
        /// </summary>
        /// <param name="pszAppID">App Id.</param>
        void SetAppID([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);

        /// <summary>
        /// Retrieve an IEnumObjects or IObjectArray for IShellItems and/or IShellLinks. 
        /// Items may appear in both the frequent and recent lists.  
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object GetList([In] ADLT listtype, [In] uint cItemsDesired, [In] ref Guid riid);
    }

    [GuidAttribute("86bec222-30f2-47e0-9f25-60d11cd75c28")]
    [ClassInterfaceAttribute(ClassInterfaceType.None)]
    [ComImportAttribute()]
    public class ApplicationDocumentLists { }
}
