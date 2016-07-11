using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpMsi.Native;

namespace SharpMsi
{
    public class MsiSummaryInformation : MsiObject, IMsiDbObject
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
        private readonly MsiDatabase _Database;

        /// <summary>
        /// The numeric value of the ANSI code page used to display the Summary Information. This property must be set before any string properties are set in the summary information.
        /// </summary>
        public short CodePage
        {
            get
            {
                return Convert.ToInt16(GetProperty(PID_CODEPAGE));
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
            set
            {
                SetProperty(PID_SUBJECT, value);
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
            set
            {
                SetProperty(PID_AUTHOR, value);
            }
        }

        /// <summary>
        /// A list of keywords that may be used by file browsers to do keyword searches for a file. The keywords should include "Installer" as well as product-specific keywords.
        /// </summary>
        public string Keywords
        {
            get
            {
                return Convert.ToString(GetProperty(PID_KEYWORDS));
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

        public MsiDatabase Database
        {
            get { return _Database; }
        }

        public MsiSummaryInformation(IntPtr handle, MsiDatabase database, uint updateCount)
            : base(handle)
        {
            _Database = database;
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

        #region Set property value

        private void SetProperty(uint pid, object value)
        {
            var valueType = value.GetType();
            uint dataType = 0;

            int intValue = 0;
            string stringValue = null;
            var timeValue = default(System.Runtime.InteropServices.ComTypes.FILETIME);

            if (valueType == typeof(short))
            {
                intValue = (short)value;
                dataType = (uint)VARENUM.VT_I2;
            }
            else if (valueType == typeof(int))
            {
                intValue = (int)value;
                dataType = (uint)VARENUM.VT_I4;
            }
            else if (valueType == typeof(string))
            {
                stringValue = (string)value;
                dataType = (uint)VARENUM.VT_LPSTR;
            }
            else if (valueType == typeof(DateTime))
            {
                timeValue = ((DateTime)value).DateTimeToFiletime();
                dataType = (uint)VARENUM.VT_FILETIME;
            }

            var res = (MsiResult)MsiAPI.MsiSummaryInfoSetProperty(Handle, pid, dataType, intValue, timeValue, stringValue);
            if (res != MsiResult.Success)
                throw MsiAPI.GetMsiResultException(res);
        }

        #endregion

        public void Commit()
        {
            MsiAPI.MsiSummaryInfoPersist(Handle);
            //after commiting, MsiSummaryInfoGetProperty always return VT_EMPTY and MsiSummaryInfoSetProperty returns ERROR_FUNCTION_FAILED
            //so we reload the summary
            RecreateHandle();
        }

        private void RecreateHandle()
        {
            if(Handle != IntPtr.Zero)
                MsiAPI.MsiCloseHandle(Handle);

            IntPtr summaryPtr;
            MsiAPI.MsiGetSummaryInformation(Database.Handle, null, UpdateCount, out summaryPtr);
            Handle = summaryPtr;
        }

        public static MsiSummaryInformation GetSummaryInformation(MsiDatabase database)
        {
            IntPtr summaryPtr;
            var res = (MsiResult)MsiAPI.MsiGetSummaryInformation(database.Handle, null, 0, out summaryPtr);
            if (res == MsiResult.Success && summaryPtr != IntPtr.Zero)
            {
                var updateCount = 0u;
                if (!database.ReadOnly)
                {
                    //set the update count to the number of properties. I don't know what is the purpose of limiting the number of updates.
                    //We could also just put a large number.
                    MsiAPI.MsiSummaryInfoGetPropertyCount(summaryPtr, out updateCount);
                    var sumInfo = new MsiSummaryInformation(summaryPtr, database, updateCount);
                    sumInfo.RecreateHandle();//recreate summary handle with obtained updateCount value
                    return sumInfo;
                }
                return new MsiSummaryInformation(summaryPtr, database, updateCount);
            }
            return null;
        }

        public static MsiSummaryInformation GetSummaryInformation(MsiDatabase database, uint updateCount)
        {
            IntPtr summaryPtr;
            var res = (MsiResult)MsiAPI.MsiGetSummaryInformation(database.Handle, null, updateCount, out summaryPtr);
            if (res == MsiResult.Success && summaryPtr != IntPtr.Zero)
                return new MsiSummaryInformation(summaryPtr, database, updateCount);
            return null;
        }

        ~MsiSummaryInformation()
        {
            Dispose();
        }

        public override void Dispose()
        {
            //MSDN:
            //If a value of uiUpdateCount greater than 0 is used to open an existing summary information stream, MsiSummaryInfoPersist must be called before closing the phSummaryInfo handle. 
            //Failing to do this will lose the existing stream information.
            if (UpdateCount > 0 && Handle != IntPtr.Zero)
            {
                MsiAPI.MsiSummaryInfoPersist(Handle);
            }
            base.Dispose();
        }
    }
}
