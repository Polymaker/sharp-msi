using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SharpMsi.Native
{
    public static class CabinetAPI
    {
        
        [DllImport("cabinet.dll")]
        public static extern IntPtr FDICreate(IntPtr pfnalloc, IntPtr pfnfree, IntPtr pfnopen, IntPtr pfnread, IntPtr pfnwrite, IntPtr pfnclose, IntPtr pfnseek, int cpuType, [In, Out] IntPtr perf);

        [DllImport("cabinet.dll")]
        public static extern bool FDIDestroy(IntPtr hfdi);

        [StructLayout(LayoutKind.Sequential)]
        public struct ERF
        {
            public int erfOper;
            public int erfType;
            public bool fError;
        }

        public delegate IntPtr FNALLOC(uint cb);

        public delegate void FNFREE(IntPtr pv);

        public static IntPtr FDICreate()
        {
            //Marshal.GetFunctionPointerForDelegate
            return IntPtr.Zero;
        }


    }
}
