using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace SharpMsi.Native
{
    public static class MsiAPI
    {
        [DllImport("msi.dll", ExactSpelling = true)]
        public static extern uint MsiCloseHandle(IntPtr hAny);

        [DllImport("msi.dll", ExactSpelling = true)]
        public static extern IntPtr MsiCreateRecord(uint cParams);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern int MsiDatabaseCommit(IntPtr hDatabase);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern uint MsiDatabaseGetPrimaryKeys(IntPtr hDatabase, string szTableName, out IntPtr phRecord);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern int MsiDatabaseOpenView(IntPtr hDatabase, string szQuery, out IntPtr phView);

        [DllImport("msi.dll", SetLastError = true)]
        public static extern int MsiEnumComponents(int iComponentIndex, StringBuilder lpProductBuf);

        [DllImport("msi.dll", SetLastError = true)]
        public static extern int MsiEnumProducts(int iProductIndex, StringBuilder lpProductBuf);

        [DllImport("msi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern UInt32 MsiEnumRelatedProducts(string strUpgradeCode, int reserved, int iIndex, StringBuilder sbProductCode);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern int MsiFormatRecord(IntPtr hInstall, IntPtr hRecord, [Out] StringBuilder szResultBuf, ref int pcchResultBuf);

        [DllImport("msi.dll", ExactSpelling = true)]
        public static extern IntPtr MsiGetLastErrorRecord();

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern uint MsiOpenDatabase(string szDatabasePath, IntPtr phPersist, out IntPtr phDatabase);

        [DllImport("msi.dll", ExactSpelling = true)]
        public static extern int MsiRecordDataSize(IntPtr hRecord, uint iField);

        [DllImport("msi.dll", ExactSpelling = true)]
        public static extern int MsiRecordGetFieldCount(IntPtr hRecord);

        [DllImport("msi.dll", ExactSpelling = true)]
        public static extern int MsiRecordGetInteger(IntPtr hRecord, uint iField);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern int MsiRecordGetString(IntPtr hRecord, uint iField, StringBuilder szValueBuf, ref int pcchValueBuf);

        [DllImport("msi.dll", ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MsiRecordIsNull(IntPtr hRecord, uint iField);

        [DllImport("msi.dll", ExactSpelling = true)]
        public static extern uint MsiRecordReadStream(IntPtr hRecord, uint iField, [Out] byte[] szDataBuf, ref int pcbDataBuf);

        [DllImport("msi.dll", ExactSpelling = true)]
        public static extern int MsiRecordSetInteger(IntPtr hRecord, uint iField, int iValue);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern int MsiRecordSetStream(IntPtr hRecord, uint iField, string szFilePath);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern int MsiRecordSetString(IntPtr hRecord, uint iField, string szValue);

        [DllImport("msi.dll")]
        public static extern int MsiViewClose(IntPtr viewhandle);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern int MsiViewExecute(IntPtr hView, IntPtr hRecord);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern uint MsiViewFetch(IntPtr hView, out IntPtr hRecord);

        [DllImport("msi.dll", ExactSpelling = true)]
        public static extern uint MsiViewGetColumnInfo(IntPtr hView, int eColumnInfo, out IntPtr hRecord);

        [DllImport("msi.dll")]
        public static extern uint MsiViewModify(IntPtr hView, int eModifyMode, IntPtr hRecord);

        public static Exception GetMsiResultException(MsiResult res)
        {
            var msiError = GetLastError();

            if(msiError == null)
                return new Exception("Msi function failed with result : " + Enum.GetName(typeof(MsiResult), res));

            return new MsiException(msiError.Code, msiError.FormatedMessage, res);
        }

        private static MsiError _LastError = null;

        public static MsiError GetLastError()
        {
            var errorRecordPtr = MsiGetLastErrorRecord();
            if (errorRecordPtr == IntPtr.Zero)
                return _LastError;

            MsiViewRecord errorRecord = null;

            try
            {
                //int textSize = 0;
                //StringBuilder errorText = new StringBuilder();
                //var res = (MsiResult)MsiFormatRecord(IntPtr.Zero, errorRecordPtr, errorText, ref textSize);
                //if (res == MsiResult.MoreData)
                //{
                //    textSize++;// returned size does not include null terminator.
                //    errorText = new StringBuilder(textSize);
                //    res = (MsiResult)MsiFormatRecord(IntPtr.Zero, errorRecordPtr, errorText, ref textSize);
                //    if (res == MsiResult.Success)
                //    {

                //    }
                //}

                errorRecord = new MsiViewRecord(errorRecordPtr);

                var errorCode = errorRecord.GetInteger(1);
                var errorMessage = GetErrorMessage(errorCode);
                var errorMessageArgs = new string[errorRecord.FieldCount - 1];

                for (int i = 2; i <= errorRecord.FieldCount; i++)
                    errorMessageArgs[i - 2] = errorRecord.GetString(i);
                _LastError = new MsiError(
                    errorCode, 
                    errorMessage, 
                    TryFormatErrorMessage(errorMessage, errorMessageArgs)
                    );
            }
            finally
            {
                //MsiCloseHandle(errorRecordPtr);
                if (errorRecord != null)
                    errorRecord.Dispose();
            }
            return _LastError;
        }

        private static XDocument ErrorList;

        private static string GetErrorMessage(int errorCode)
        {
            if (ErrorList == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("SharpMsi.MsiErrors.xml"))
                    ErrorList = XDocument.Load(stream);
            }
            var errorElem = ErrorList.Descendants("Error").FirstOrDefault(e => e.Attribute("code").Value == errorCode.ToString());
            if (errorElem != null)
                return errorElem.Attribute("message").Value;
            return string.Empty;
        }

        private static string TryFormatErrorMessage(string message, string[] messageArgs)
        {
            if (!(message.Contains("{") && messageArgs.Length > 0))
                return message;
            try
            {
                return string.Format(message, messageArgs);
            }
            catch
            {
                return message;
            }
        }
    }
}
