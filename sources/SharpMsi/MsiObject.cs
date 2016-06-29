using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpMsi.Native;

namespace SharpMsi
{
    public class MsiObject : IDisposable
    {
        private IntPtr _Handle;

        public IntPtr Handle
        {
            get { return _Handle; }
            protected set { _Handle = value; }
        }

        public MsiObject(IntPtr handle)
        {
            _Handle = handle;
        }

        ~MsiObject()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                MsiAPI.MsiCloseHandle(Handle);
                Handle = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }
}
