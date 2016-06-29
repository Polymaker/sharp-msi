using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpMsi.Native
{
    public static class NativeUtils
    {
        public static void PrintHexPointer(IntPtr objectPtr, int ptrSize)
        {
            byte[] arrayData = new byte[ptrSize];
            Marshal.Copy(objectPtr, arrayData, 0, (int)ptrSize);

            for (int i = 0; i < (int)Math.Ceiling(ptrSize / 16d); i++)
            {
                Trace.WriteLine(arrayData.Skip(i * 16).Take(16).Select(x => x.ToString("X2")).Aggregate((a, b) => a + " " + b));
            }
        }
    }
}
