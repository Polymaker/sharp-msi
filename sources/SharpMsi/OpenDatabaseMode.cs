using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi
{
    public enum OpenDatabaseMode : int
    {
        /// <summary>
        /// Open a database read-only, no persistent changes.
        /// </summary>
        ReadOnly = 0,
        /// <summary>
        /// Open a database read/write in transaction mode.
        /// </summary>
        Transcation = 1,
        /// <summary>
        /// Open a database direct read/write without transaction.
        /// </summary>
        Direct = 2,
        /// <summary>
        /// Create a new database, transact mode read/write.
        /// </summary>
        Create = 3,
        /// <summary>
        /// Create a new database, direct mode read/write.
        /// </summary>
        CreateDirect = 4
    }
}
