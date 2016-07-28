using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi
{
    public enum MsiViewType
    {
        Unknown = -1,
        Query,
        Insert,
        Update,
        Delete,
        Create,
        Drop,
        Alter
    }
}
