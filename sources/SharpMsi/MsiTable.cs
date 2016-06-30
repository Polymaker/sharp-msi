using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpMsi.Native;

namespace SharpMsi
{
    public class MsiTable
    {
        // Fields...
        private readonly MsiDatabase _Database;
        private string _Name;
        private MsiTableColumn[] _Columns;

        public MsiDatabase Database
        {
            get { return _Database; }
        }

        public string Name
        {
            get { return _Name; }
        }

        public MsiTableColumn[] Columns
        {
            get 
            {
                if (_Columns.Length == 0)
                    LoadColumnsInfo();
                return _Columns; 
            }
        }

        internal MsiTable(MsiDatabase database, string name)
        {
            _Database = database;
            _Name = name;
            _Columns = new MsiTableColumn[0];
        }

        private void LoadColumnsInfo()
        {

            var pkColumnNames = GetPrimaryKeyColumnNames();

            using (var tmpView = Database.OpenView("SELECT * FROM " + Name))
            {
                _Columns = new MsiTableColumn[tmpView.Columns.Length];
                for (int i = 0; i < _Columns.Length; i++)
                {
                    _Columns[i] = new MsiTableColumn(
                        tmpView.Columns[i], 
                        pkColumnNames.Contains(tmpView.Columns[i].Name));
                }
            }
        }

        private string[] GetPrimaryKeyColumnNames()
        {
            var pkInfoPtr = IntPtr.Zero;
            var res = (MsiResult)MsiAPI.MsiDatabaseGetPrimaryKeys(Database.Handle, Name, out pkInfoPtr);
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
