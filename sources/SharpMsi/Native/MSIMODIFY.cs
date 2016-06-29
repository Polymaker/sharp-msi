using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi.Native
{
    public enum MSIMODIFY : int
    {
        SEEK = -1,
        REFRESH = 0,
        INSERT = 1,
        UPDATE,
        ASSIGN,
        REPLACE,
        MERGE,
        DELETE,
    }
}
