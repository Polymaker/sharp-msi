using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpMsi.Native;

namespace SharpMsi
{
    public class MsiSummaryInformation : MsiObject
    {
        public const uint PID_CODEPAGE = 1;
        public const uint PID_TITLE = 2;

        // Fields...
        private readonly uint _UpdateCount;

        public string Title
        {
            get
            {
                return Convert.ToString(GetProperty(PID_TITLE));
            }
        }

        public uint UpdateCount
        {
            get { return _UpdateCount; }
        }

        public MsiSummaryInformation(IntPtr handle, uint updateCount)
            : base(handle)
        {
            _UpdateCount = updateCount;
        }

        #region Get property value

        public object GetProperty(uint pid)
        {
            uint dataTypeNum;

            int intValue;
            System.Runtime.InteropServices.ComTypes.FILETIME timeValue;
            StringBuilder stringValue = new StringBuilder();
            int stringSize = 0;

            var funcResult = (MsiResult)MsiAPI.MsiSummaryInfoGetProperty(Handle, pid, out dataTypeNum, out intValue, out timeValue, stringValue, ref stringSize);

            var dataType = (VARENUM)dataTypeNum;
            if (dataType == VARENUM.VT_I2 || dataType == VARENUM.VT_I4)
            {
                if (funcResult == MsiResult.Success)
                    return intValue;
            }
            else if (dataType == VARENUM.VT_LPSTR || dataType == VARENUM.VT_LPWSTR)
            {
                if (funcResult == MsiResult.MoreData)
                {
                    stringValue = new StringBuilder(++stringSize);// returned size does not include null terminator.
                    funcResult = (MsiResult)MsiAPI.MsiSummaryInfoGetProperty(Handle, pid, out dataTypeNum, out intValue, out timeValue, stringValue, ref stringSize);
                }

                if (funcResult == MsiResult.Success)
                    return stringValue.ToString();
            }
            else if (dataType == VARENUM.VT_FILETIME)
            {
                if (funcResult == MsiResult.Success)
                    return timeValue.FiletimeToDateTime();
            }
            
            return null;
        }

        public bool GetProperty<T>(uint pid, out T value)
        {
            var result = GetProperty(pid);
            if (result == null)
            {
                value = default(T);
                return false;
            }
            else
            {
                value = (T)result;
                return true;
            }
        }

        #endregion

        ~MsiSummaryInformation()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (UpdateCount > 0 && Handle != IntPtr.Zero)
            {
                MsiAPI.MsiSummaryInfoPersist(Handle);
            }
            base.Dispose();
        }
    }
}
