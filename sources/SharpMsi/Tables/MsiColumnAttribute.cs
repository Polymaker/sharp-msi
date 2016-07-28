using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi.Tables
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class MsiColumnAttribute : Attribute
    {
        // Fields...
        private string _MapTo;

        public string MapTo
        {
            get { return _MapTo; }
        }

        public MsiColumnType ColumnType { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsNullable { get; set; }

        public MsiColumnAttribute(string mapTo)
        {
            _MapTo = mapTo;
        }

        public MsiColumnAttribute(string mapTo, MsiColumnType columnType)
        {
            _MapTo = mapTo;
            ColumnType = columnType;
            IsPrimary = false;
            IsNullable = false;
        }

        public MsiColumnAttribute(string mapTo, MsiColumnType columnType, bool isNullable)
        {
            _MapTo = mapTo;
            ColumnType = columnType;
            IsNullable = isNullable;
            IsPrimary = false;
        }

        public MsiColumnAttribute(string mapTo, MsiColumnType columnType, bool isPrimary, bool isNullable)
        {
            _MapTo = mapTo;
            ColumnType = columnType;
            IsPrimary = isPrimary;
            IsNullable = isNullable;
        }
    }
}
