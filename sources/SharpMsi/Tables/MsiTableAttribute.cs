using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi.Tables
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MsiTableAttribute : Attribute
    {
        // Fields...
        private string _TableName;

        public string TableName
        {
            get { return _TableName; }
        }
        
        public bool Temporary { get; set; }
        public bool SystemTable { get; set; }

        public MsiTableAttribute(string tableName)
        {
            _TableName = tableName;
            Temporary = false;
            SystemTable = false;
        }
    }
}
