using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi.Tables
{
    [MsiTable("Property")]
    public class Property : TableRecord
    {
        // Fields...
        private string _Value;
        private string _Name;

        [MsiColumn("Property", MsiColumnType.String, IsPrimary = true)]
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
            }
        }

        [MsiColumn("Value", MsiColumnType.LocalizableString)]
        public string Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
            }
        }
    }
}
