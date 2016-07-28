﻿using System;
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
        /// Must first call MsiViewFetch with the same record. 
        /// Fails for a deleted row. 
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
        /// <summary>
        /// Updates an existing record. Nonprimary keys only. 
        /// Must first call MsiViewFetch. 
        /// Fails with a deleted record. Works only with read-write records.
        /// </summary>
        UPDATE,
        /// <summary>
        /// Writes current data in the cursor to a table row. 
        /// Updates record if the primary keys match an existing row and inserts if they do not match. 
        /// Fails with a read-only database. 
        /// This mode cannot be used with a view containing joins.
        /// </summary>
        ASSIGN,
        /// <summary>
        /// Updates or deletes and inserts a record into a table. 
        /// Must first call MsiViewFetch with the same record. 
        /// Updates record if the primary keys are unchanged. 
        /// Deletes old row and inserts new if primary keys have changed. 
        /// Fails with a read-only database. 
        /// This mode cannot be used with a view containing joins.
        /// </summary>
        REPLACE,
        MERGE,
        /// <summary>
        /// Remove a row from the table. 
        /// You must first call the MsiViewFetch function with the same record. 
        /// Fails if the row has been deleted. 
        /// Works only with read-write records. 
        /// This mode cannot be used with a view containing joins.
        /// </summary>
        DELETE,
        INSERT_TEMPORARY,
        VALIDATE,
        VALIDATE_NEW,
        VALIDATE_FIELD,
        VALIDATE_DELETE
    }
}
