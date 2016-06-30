using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpMsi.Native;

namespace SharpMsi
{
    public class MsiRecord : MsiObject
    {
        // Fields...
        private int _FieldCount;

        public int FieldCount
        {
            get { return _FieldCount; }
        }

        internal MsiRecord(IntPtr handle)
            : base(handle)
        {
            _FieldCount = MsiAPI.MsiRecordGetFieldCount(Handle);
        }

        public int GetFieldSize(int fieldIndex)
        {
            if (fieldIndex <= 0 || fieldIndex > FieldCount)
                throw new IndexOutOfRangeException("fieldIndex");
            return MsiAPI.MsiRecordDataSize(Handle, (uint)fieldIndex);
        }

        public bool IsNull(int fieldIndex)
        {
            if (fieldIndex <= 0 || fieldIndex > FieldCount)
                throw new IndexOutOfRangeException("fieldIndex");
            return MsiAPI.MsiRecordIsNull(Handle, (uint)fieldIndex);
        }

        #region Get values

        public int GetInteger(int fieldIndex)
        {
            if (fieldIndex <= 0 || fieldIndex > FieldCount)
                throw new IndexOutOfRangeException("fieldIndex");

            return MsiAPI.MsiRecordGetInteger(Handle, (uint)fieldIndex);
        }

        public string GetString(int fieldIndex)
        {
            if (fieldIndex <= 0 || fieldIndex > FieldCount)
                throw new IndexOutOfRangeException("fieldIndex");

            return GetString(fieldIndex, GetFieldSize(fieldIndex));
        }

        public string GetString(int fieldIndex, int length)
        {
            if (fieldIndex <= 0 || fieldIndex > FieldCount)
                throw new IndexOutOfRangeException("fieldIndex");

            var builder = new StringBuilder(length + 1);
            var count = builder.Capacity;

            var res = (MsiResult)MsiAPI.MsiRecordGetString(Handle, (uint)fieldIndex, builder, ref count);
            if (res != MsiResult.Success)
                throw MsiAPI.GetMsiResultException(res);

            return builder.ToString();
        }

        public int ReadStream(int fieldIndex, byte[] buffer, int length)
        {
            if (fieldIndex <= 0 || fieldIndex > FieldCount)
                throw new IndexOutOfRangeException("fieldIndex");

            int byteRead = length;
            var res = (MsiResult)MsiAPI.MsiRecordReadStream(Handle, (uint)fieldIndex, buffer, ref byteRead);
            if (!(res == MsiResult.Success || res == MsiResult.MoreData))
                throw MsiAPI.GetMsiResultException(res);
            return byteRead;
        }

        public Stream GetStream(int fieldIndex)
        {
            if (fieldIndex <= 0 || fieldIndex > FieldCount)
                throw new IndexOutOfRangeException("fieldIndex");

            byte[] buffer = new byte[256];
            var ms = new MemoryStream();
            int byteRead = buffer.Length;
            do
            {
                //byteRead = buffer.Length;
                var res = (MsiResult)MsiAPI.MsiRecordReadStream(Handle, (uint)fieldIndex, buffer, ref byteRead);
                if (!(res == MsiResult.Success || res == MsiResult.MoreData))
                    throw MsiAPI.GetMsiResultException(res);
                ms.Write(buffer, 0, byteRead);
                if (res == MsiResult.Success)
                    break;
            }
            while (true);

            return ms;
        }

        #endregion

        #region Set values

        public void SetInteger(int fieldIndex, int value)
        {
            if (fieldIndex <= 0 || fieldIndex > FieldCount)
                throw new IndexOutOfRangeException("fieldIndex");
            MsiAPI.MsiRecordSetInteger(Handle, (uint)fieldIndex, value);
        }

        public void SetString(int fieldIndex, string value)
        {
            if (fieldIndex <= 0 || fieldIndex > FieldCount)
                throw new IndexOutOfRangeException("fieldIndex");
            MsiAPI.MsiRecordSetString(Handle, (uint)fieldIndex, value);
        }

        public void SetStream(int fieldIndex, Stream value)
        {
            if (fieldIndex <= 0 || fieldIndex > FieldCount)
                throw new IndexOutOfRangeException("fieldIndex");

            var tmpFilepath = Path.GetTempFileName();

            value.Seek(0, SeekOrigin.Begin);
            using (var fs = File.OpenWrite(tmpFilepath))
                value.CopyTo(fs);

            MsiAPI.MsiRecordSetStream(Handle, (uint)fieldIndex, tmpFilepath);
            File.Delete(tmpFilepath);
        }

        public void SetValue(int fieldIndex, object value)
        {
            var valueType = value.GetType();
            switch (valueType.Name)
            {
                default:
                    throw new NotSupportedException("Value type.");
                case "Byte":
                case "Int16":
                case "Int32":
                case "Int64":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                    SetInteger(fieldIndex, Convert.ToInt32(value));
                    break;
                case "Char":
                case "String":
                    SetString(fieldIndex, Convert.ToString(value));
                    break;
            }
        }

        #endregion

        public static MsiRecord Create(int fieldCount)
        {
            return new MsiRecord(MsiAPI.MsiCreateRecord((uint)fieldCount));
        }

        ~MsiRecord()
        {
            Dispose();
        }
    }
}
