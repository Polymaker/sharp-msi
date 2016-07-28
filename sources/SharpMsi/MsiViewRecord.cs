using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi
{
    /// <summary>
    /// Does nothing more thant MsiRecord, but keeps a reference to its view to prevent GC from disposing the view while the record is still used.
    /// It also allows to access the view's columns and therefore we can have detailed information about the record's fields/columns.
    /// </summary>
    public class MsiViewRecord : MsiRecord
    {
        private readonly MsiView _View;

        public MsiView View
        {
            get { return _View; }
        }
        
        internal MsiViewRecord(IntPtr handle, MsiView view)
            : base(handle)
        {
            _View = view;
        }

        ~MsiViewRecord()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldIndex">Index of the field. Start at 1 (not 0 like the programming convention).</param>
        /// <returns></returns>
        public object GetValue(int fieldIndex)
        {
            return GetValue(fieldIndex, View.Columns[fieldIndex - 1]);
        }

        public object GetValue(MsiColumnInfo column)
        {
            return GetValue(View.Columns.IndexOf(column) + 1, column);
        }

        private object GetValue(int index, MsiColumnInfo column)
        {
            switch (column.Type)
            {
                case MsiColumnType.String:
                case MsiColumnType.LocalizableString:
                    return GetString(index);
                case MsiColumnType.Number:
                    return GetInteger(index);
                case MsiColumnType.Object:
                    return GetStream(index);
            }
            return null;
        }

        public string Trace()
        {
            string outputStr = View.Tables.Length == 1 ? View.Tables[0] : "Record";
            outputStr += string.Format("({0}) ", View.Columns.Select(c => c.Name).Aggregate((i, j) => i + ", " + j));
            outputStr += string.Format("Values({0}) ", View.Columns
                .Select(c => c.Type == MsiColumnType.Object ? "Stream" : GetValue(c).ToString())
                .Aggregate((i, j) => i + ", " + j));
            return outputStr;
        }
    }
}
