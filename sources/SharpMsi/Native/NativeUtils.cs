using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace SharpMsi.Native
{
    public static class NativeUtils
    {
        public static void PrintHexPointer(IntPtr objectPtr, int ptrSize)
        {
            byte[] arrayData = new byte[ptrSize];
            Marshal.Copy(objectPtr, arrayData, 0, (int)ptrSize);
            PrintByteArray(arrayData);
            //for (int i = 0; i < (int)Math.Ceiling(ptrSize / 16d); i++)
            //{
            //    Trace.WriteLine(arrayData.Skip(i * 16).Take(16).Select(x => x.ToString("X2")).Aggregate((a, b) => a + " " + b));
            //}
        }

        public static void PrintByteArray(byte[] arrayData)
        {
            for (int i = 0; i < (int)Math.Ceiling(arrayData.Length / 16d); i++)
            {
                Trace.WriteLine(arrayData.Skip(i * 16).Take(16).Select(x => x.ToString("X2")).Aggregate((a, b) => a + " " + b));
            }
        }

        public static DateTime FiletimeToDateTime(this FILETIME fileTime)
        {
            long hFT2 = (((long)fileTime.dwHighDateTime) << 32) | ((uint)fileTime.dwLowDateTime);
            return DateTime.FromFileTime(hFT2);
        }

        public static FILETIME DateTimeToFiletime(this DateTime time)
        {
            FILETIME ft;
            long hFT1 = time.ToFileTime();
            ft.dwLowDateTime = (int)(hFT1 & 0xFFFFFFFF);
            ft.dwHighDateTime = (int)(hFT1 >> 32);
            return ft;
        }
    }
}
