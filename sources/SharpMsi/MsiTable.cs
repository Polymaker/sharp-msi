using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SharpMsi.Native;

namespace SharpMsi
{
    [DebuggerDisplay("{Name}")]
    public class MsiTable : IMsiDbObject
    {
        // Fields...
        //private bool _IsTemporary;
        private readonly MsiDatabase _Database;
        private readonly string _Name;
        private MsiTableColumn[] _Columns;

        public MsiDatabase Database
        {
            get { return _Database; }
        }

        public string Name
        {
            get { return _Name; }
        }

        //public bool IsTemporary
        //{
        //    get { return _IsTemporary; }
        //}

        public MsiTableColumn[] Columns
        {
            get 
            {
                if (_Columns.Length == 0)
                    _Columns = GetTableColumns(Database, Name);
                return _Columns; 
            }
        }

        internal MsiTable(MsiDatabase database, string name)
        {
            _Database = database;
            _Name = name;
            _Columns = new MsiTableColumn[0];
        }

        internal MsiTable(MsiDatabase database, string name, MsiTableColumn[] columns)
        {
            _Database = database;
            _Name = name;
            _Columns = columns;
        }

        public IEnumerable<MsiViewRecord> Query()
        {
            return Database.Query(string.Format("SELECT * FROM `{0}`", Name));
        }

        internal static MsiTableColumn[] GetTableColumns(MsiDatabase database, string tablename)
        {
            var pkColumnNames = GetPrimaryKeys(database, tablename);
            var columns = new MsiTableColumn[0];

            using (var tmpView = database.OpenView(string.Format("SELECT * FROM `{0}`", tablename)))
            {
                columns = new MsiTableColumn[tmpView.Columns.Length];
                for (int i = 0; i < columns.Length; i++)
                {
                    columns[i] = new MsiTableColumn(
                        tmpView.Columns[i],
                        pkColumnNames.Contains(tmpView.Columns[i].Name));
                }
            }
            return columns;
        }

        internal static string[] GetPrimaryKeys(MsiDatabase database, string tablename)
        {
            var pkInfoPtr = IntPtr.Zero;
            var res = (MsiResult)MsiAPI.MsiDatabaseGetPrimaryKeys(database.Handle, tablename, out pkInfoPtr);
            if (res != MsiResult.Success)
                return new string[0];
            var pkInfoRecord = new MsiRecord(pkInfoPtr);
            var colNames = new string[pkInfoRecord.FieldCount];
            for (int i = 0; i < pkInfoRecord.FieldCount; i++)
                colNames[i] = pkInfoRecord.GetString(i + 1);
            pkInfoRecord.Dispose();
            return colNames;
        }
    }
}
