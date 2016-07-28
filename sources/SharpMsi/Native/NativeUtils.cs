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

        [DllImport("Shell32.dll")]
        internal static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);

        private static string[] KnownFolderGuids = new string[]
        {
            "{56784854-C6CB-462B-8169-88E350ACB882}", // Contacts
            "{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", // Desktop
            "{FDD39AD0-238F-46AF-ADB4-6C85480369C7}", // Documents
            "{374DE290-123F-4565-9164-39C4925E467B}", // Downloads
            "{1777F761-68AD-4D8A-87BD-30B759FA33DD}", // Favorites
            "{BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968}", // Links
            "{4BD8D571-6D19-48D3-BE97-422220080E43}", // Music
            "{33E28130-4E1E-4676-835A-98395C3BC3BB}", // Pictures
            "{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}", // SavedGames
            "{7D1D3A04-DEBB-4115-95CF-2F29DA2920DA}", // SavedSearches
            "{18989B1D-99B5-455B-841C-AB7C74E4DDFC}", // Videos
        };

        public enum KnownFolder
        {
            Contacts,
            Desktop,
            Documents,
            Downloads,
            Favorites,
            Links,
            Music,
            Pictures,
            SavedGames,
            SavedSearches,
            Videos
        }

        public static string GetKnownFolder(KnownFolder knownFolder)
        {
            IntPtr outPath;
            int result = SHGetKnownFolderPath(new Guid(KnownFolderGuids[(int)knownFolder]), 0x004000, IntPtr.Zero, out outPath);
            if (result >= 0)
            {
                return Marshal.PtrToStringUni(outPath);
            }
            else
            {
                throw new ExternalException("Unable to retrieve the known folder path. It may not "
                    + "be available on this system.", result);
            }
        }
    }
}
