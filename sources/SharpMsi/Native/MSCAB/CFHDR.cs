using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi.Native.MSCAB
{
    [Flags]
    public enum CFHDR : ushort
    {
        /// <summary>
        /// Flag PREV_CABINET is set if this cabinet file is not the first in a set of cabinet files. When this bit is set, the szCabinetPrev and szDiskPrev fields are present in this CFHEADER.
        /// </summary>
        PREV_CABINET = 0x0001,
        /// <summary>
        /// Flag NEXT_CABINET is set if this cabinet file is not the last in a set of cabinet files. When this bit is set, the szCabinetNext and szDiskNext fields are present in this CFHEADER.
        /// </summary>
        NEXT_CABINET = 0x0002,
        /// <summary>
        /// Flag RESERVE_PRESENT is set if this cabinet file contains any reserved fields. When this bit is set, the cbCFHeader, cbCFFolder, and cbCFData fields are present in this CFHEADER.
        /// </summary>
        RESERVE_PRESENT = 0x0004
    }
}
