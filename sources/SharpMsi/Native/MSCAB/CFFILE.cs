using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SharpMsi.Native.MSCAB
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CFFILE
    {
        /// <summary> uncompressed size of this file in bytes </summary>
        public uint cbFile;
        /// <summary> uncompressed offset of this file in the folder </summary>
        public uint uoffFolderStart;
    }
}
