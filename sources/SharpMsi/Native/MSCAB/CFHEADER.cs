using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SharpMsi.Native.MSCAB
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CFHEADER
    {
        /// <summary> file signature </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] signature;
        /// <summary> reserved </summary>
        public uint reserved1;
        /// <summary> size of this cabinet file in bytes </summary>
        public uint cbCabinet;
        /// <summary> reserved </summary>
        public uint reserved2;
        /// <summary> offset of the first CFFILE entry </summary>
        public uint coffFiles;
        /// <summary> reserved </summary>
        public uint reserved3;
        /// <summary> cabinet file format version, minor </summary>
        public byte versionMinor;
        /// <summary> cabinet file format version, major </summary>
        public byte versionMajor;
        /// <summary> number of CFFOLDER entries in this cabinet</summary>
        public ushort cFolders;
        /// <summary> number of CFFILE entries in this cabinet </summary>
        public ushort cFiles;
        /// <summary> cabinet file option indicators </summary>
        [MarshalAs(UnmanagedType.U2)]
        public CFHDR flags;
        /// <summary> must be the same for all cabinets in a set</summary>
        public ushort setID;
        /// <summary> number of this cabinet file in a set </summary>
        public ushort iCabinet;
        /*
        /// <summary> (optional, requires flag RESERVE_PRESENT) size of per-cabinet reserved area</summary>
        public ushort cbCFHeader;
        /// <summary> (optional, requires flag RESERVE_PRESENT) size of per-folder reserved area</summary>
        public byte cbCFFolder;
        /// <summary> (optional, requires flag RESERVE_PRESENT) size of per-datablock reserved area</summary>
        public byte cbCFData;
        /// <summary> (optional, requires flag RESERVE_PRESENT) per-cabinet reserved area. Size of cbCFHeader</summary>
        public byte[] abReserve;
        /// <summary> (optional, requires flag PREV_CABINET) name of previous cabinet file </summary>
        public byte[] szCabinetPrev;
        /// <summary> (optional, requires flag PREV_CABINET) name of previous disk </summary>
        public byte[] szDiskPrev;
        /// <summary> (optional, requires flag NEXT_CABINET) name of next cabinet file </summary>
        public byte[] szCabinetNext;
        /// <summary> (optional, requires flag NEXT_CABINET) name of next disk </summary>
        public byte[] szDiskNext;
        */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CFHEADER_RESERVE
    {
        /// <summary> size of per-cabinet reserved area</summary>
        public ushort cbCFHeader;
        /// <summary> size of per-folder reserved area</summary>
        public byte cbCFFolder;
        /// <summary> size of per-datablock reserved area</summary>
        public byte cbCFData;
        /// <summary> per-cabinet reserved area. Length of cbCFHeader</summary>
        public byte[] abReserve;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CFHEADER_PREV
    {
        /// <summary> name of previous cabinet file </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string szCabinetPrev;
        /// <summary> name of previous disk </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string szDiskPrev;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CFHEADER_NEXT
    {
        /// <summary> name of next cabinet file </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string szCabinetNext;
        /// <summary> name of next disk </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string szDiskNext;
    }
}
