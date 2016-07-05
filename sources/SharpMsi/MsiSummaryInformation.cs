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
        public const uint PID_SUBJECT = 3;
        public const uint PID_AUTHOR = 4;
        public const uint PID_KEYWORDS = 5;
        public const uint PID_COMMENTS = 6;
        public const uint PID_TEMPLATE = 7;
        public const uint PID_LASTAUTHOR = 8;
        public const uint PID_REVNUMBER = 9;
        public const uint PID_LASTPRINTED = 11;
        public const uint PID_CREATE_DTM = 12;
        public const uint PID_LASTSAVE_DTM = 13;
        public const uint PID_PAGECOUNT = 14;
        public const uint PID_WORDCOUNT = 15;
        public const uint PID_CHARCOUNT = 16;
        public const uint PID_APPNAME = 18;
        public const uint PID_SECURITY = 19;

        // Fields...
        private readonly uint _UpdateCount;

        public int CodePage
        {
            get
            {
                return Convert.ToInt32(GetProperty(PID_CODEPAGE));
            }
        }

        /// <summary>
        /// A description of this file as an installation package. The description should include the phrase "Installation Database." <see cref="MSDN"/>
        /// </summary>
        public string Title
        {
            get
            {
                return Convert.ToString(GetProperty(PID_TITLE));
            }
        }

        /// <summary>
        /// The name of the product installed by this package. This should be the same name as in the ProductName property. <see cref="MSDN"/>
        /// </summary>
        public string Subject
        {
            get
            {
                return Convert.ToString(GetProperty(PID_SUBJECT));
            }
        }

        /// <summary>
        /// The name of the manufacturer of this product. This should be the same name as in the Manufacturer property. <see cref="MSDN"/>
        /// </summary>
        public string Author
        {
            get
            {
                return Convert.ToString(GetProperty(PID_AUTHOR));
            }
        }

        /// <summary>
        /// The time and date when this installer database was created.
        /// </summary>
        public DateTime CreateTime
        {
            get
            {
                return Convert.ToDateTime(GetProperty(PID_CREATE_DTM));
            }
        }

        /// <summary>
        /// Specifies the maximum number of updated values.
        /// </summary>
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
