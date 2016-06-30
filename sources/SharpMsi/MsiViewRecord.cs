using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi
{
    /// <summary>
    /// Does nothing more thant MsiRecord, but keeps a reference to its view to prevent GC from disposing the view while the record is still used.
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
    }
}
