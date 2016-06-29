using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpMsi.Native;

namespace SharpMsi
{
    public abstract class MsiObject : IDisposable
    {
        private bool _IsDisposed;
        private IntPtr _Handle;

        public IntPtr Handle
        {
            get { return _Handle; }
            protected set { _Handle = value; }
        }

        public bool IsDisposed
        {
            get { return _IsDisposed; }
        }

        public MsiObject(IntPtr handle)
        {
            _Handle = handle;
        }

        public virtual void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                MsiAPI.MsiCloseHandle(Handle);
                Handle = IntPtr.Zero;
            }
            _IsDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
