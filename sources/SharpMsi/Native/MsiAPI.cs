using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

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

        [DllImport("msi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
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
            return new Exception("Msi exception : " + Enum.GetName(typeof(MsiResult), res));
        }
    }
}
