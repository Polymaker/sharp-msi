using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi
{
    public interface IMsiDbObject
    {
        MsiDatabase Database { get; }
    }
}
