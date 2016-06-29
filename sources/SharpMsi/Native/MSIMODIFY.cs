using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi.Native
{
    public enum MSIMODIFY : int
    {
        /// <summary>
        /// Refreshes the information in the supplied record without changing the position in the result set and without affecting subsequent fetch operations. 
        /// The record may then be used for subsequent Update, Delete, and Refresh. 
        /// All primary key columns of the table must be in the query and the record must have at least as many fields as the query. 
        /// Seek cannot be used with multi-table queries. 
        /// This mode cannot be used with a view containing joins.
        /// </summary>
        SEEK = -1,
        /// <summary>
        /// Refreshes the information in the record. 
        /// Must first call MsiViewFetch with the same record. Fails for a deleted row. 
        /// Works with read-write and read-only records.
        /// </summary>
        REFRESH = 0,
        /// <summary>
        /// Inserts a record. 
        /// Fails if a row with the same primary keys exists. 
        /// Fails with a read-only database. 
        /// This mode cannot be used with a view containing joins.
        /// </summary>
        INSERT = 1,
        UPDATE,
        ASSIGN,
        REPLACE,
        MERGE,
        DELETE,
        INSERT_TEMPORARY,
        VALIDATE,
        VALIDATE_NEW,
        VALIDATE_FIELD,
        VALIDATE_DELETE
    }
}
