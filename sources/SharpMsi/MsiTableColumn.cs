using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi
{
    public class MsiTableColumn : MsiColumnInfo
    {
        private readonly bool _IsPrimaryKey;

        public bool IsPrimaryKey
        {
            get { return _IsPrimaryKey; }
        }

        public MsiTableColumn(string name, MsiColumnType type, int length, bool nullable)
            : base(name, type, length, nullable)
        {
            _IsPrimaryKey = false;
        }

        public MsiTableColumn(MsiColumnInfo colInfo, bool isPrimaryKey)
            : base(colInfo.Name, colInfo.Type, colInfo.Length, colInfo.Nullable)
        {
            _IsPrimaryKey = isPrimaryKey;
        }

        public MsiTableColumn(string name, MsiColumnType type, int length, bool nullable, bool isPrimaryKey)
            : base(name, type, length, nullable)
        {
            _IsPrimaryKey = isPrimaryKey;
        }
    }
}
