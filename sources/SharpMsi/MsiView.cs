using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SharpMsi.Native;
using System.Text.RegularExpressions;

namespace SharpMsi
{
    public class MsiView : MsiObject, IMsiDbObject
    {
        // Fields...
        private MsiViewType _ViewType;
        private string[] _Tables;
        private List<MsiViewRecord> _Records;
        private readonly MsiDatabase _Database;
        private string _Query;
        private MsiColumnInfo[] _Columns;
        private bool columnInfoFetched;

        public MsiDatabase Database
        {
            get { return _Database; }
        }

        public string Query
        {
            get { return _Query; }
        }

        public MsiViewType ViewType
        {
            get { return _ViewType; }
        }

        public string[] Tables
        {
            get { return _Tables; }
        }

        public MsiColumnInfo[] Columns
        {
            get
            {
                if (!columnInfoFetched)
                {
                    GetColumnsInfo();
                    columnInfoFetched = true;
                }
                return _Columns;
            }
        }

        public IList<MsiViewRecord> Records
        {
            get { return _Records.AsReadOnly(); }
        }

        private MsiView(IntPtr handle, MsiDatabase database, string query)
            : base(handle)
        {
            _Database = database;
            _Query = query;
            _Records = new List<MsiViewRecord>();
            _Columns = new MsiColumnInfo[0];
            _Tables = new string[0];
            _ViewType = MsiViewType.Query;
            ParseViewTypeAndTables();
        }

        private void ParseViewTypeAndTables()
        {
            var cleanQuery = Regex.Replace(Query.Trim(), @"\s?([\(\)\,\*]|<=|>=|>|<|=)\s?", " $1 ");
            string[] words = cleanQuery.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            _ViewType = ParseViewType(words[0]);

            int curIndex = 1;

            if (ViewType != MsiViewType.Update)
            {
                for (; curIndex < words.Length; curIndex++)
                {
                    if (words[curIndex].Equals("into", StringComparison.OrdinalIgnoreCase) ||
                        words[curIndex].Equals("from", StringComparison.OrdinalIgnoreCase) ||
                        words[curIndex].Equals("table", StringComparison.OrdinalIgnoreCase))
                    {
                        curIndex++;
                        break;
                    }
                }
            }

            var tables = new List<string>();
            bool foundTable = true;
            for (; curIndex < words.Length; curIndex++)
            {
                if (foundTable)
                {
                    tables.Add(words[curIndex].Trim('`'));
                    foundTable = false;
                }
                else if (words[curIndex] == ",")
                    foundTable = true;
                else
                    break;
            }
            _Tables = tables.ToArray();
        }

        private static MsiViewType ParseViewType(string sql)
        {
            if (sql.Equals("select", StringComparison.OrdinalIgnoreCase))
                return MsiViewType.Query;
            else if (sql.Equals("insert", StringComparison.OrdinalIgnoreCase))
                return MsiViewType.Insert;
            else if (sql.Equals("update", StringComparison.OrdinalIgnoreCase))
                return MsiViewType.Update;
            else if (sql.Equals("delete", StringComparison.OrdinalIgnoreCase))
                return MsiViewType.Delete;
            else if (sql.Equals("create", StringComparison.OrdinalIgnoreCase))
                return MsiViewType.Create;
            else if (sql.Equals("drop", StringComparison.OrdinalIgnoreCase))
                return MsiViewType.Drop;
            else if (sql.Equals("alter", StringComparison.OrdinalIgnoreCase))
                return MsiViewType.Alter;

            return (MsiViewType)(-1);
        }

        public static MsiView Open(MsiDatabase database, string query)
        {
            IntPtr viewHandle = IntPtr.Zero;
            var res = (MsiResult)MsiAPI.MsiDatabaseOpenView(database.Handle, query, out viewHandle);
            if (res != MsiResult.Success)
                throw MsiAPI.GetMsiResultException(res);
            return new MsiView(viewHandle, database, query);
        }

        public void Execute()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("MsiView");
            DisposeRecords();
            var res = (MsiResult)MsiAPI.MsiViewExecute(Handle, IntPtr.Zero);
            if (res != MsiResult.Success)
                throw MsiAPI.GetMsiResultException(res);
        }

        public void Execute(params object[] parameters)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("MsiView");
            DisposeRecords();
            MsiRecord inputRecord = null;
            try
            {
                inputRecord = MsiRecord.Create(parameters.Length);
                for (int i = 0; i < parameters.Length; i++)
                    inputRecord.SetValue(i + 1, parameters[i]);

                var res = (MsiResult)MsiAPI.MsiViewExecute(Handle, inputRecord.Handle);
                if (res != MsiResult.Success)
                    throw MsiAPI.GetMsiResultException(res);
            }
            finally
            {
                if (inputRecord != null)
                    inputRecord.Dispose();
            }
        }

        #region Execute Query

        public IEnumerable<MsiViewRecord> ExecuteQuery()
        {
            Execute();
            return FetchQuery();
        }

        public IEnumerable<MsiViewRecord> ExecuteQuery(params object[] parameters)
        {
            Execute(parameters);
            return FetchQuery();
        }

        private IEnumerable<MsiViewRecord> FetchQuery()
        {
            var res = MsiResult.Success;
            while (res == MsiResult.Success)
            {
                IntPtr recordPtr = IntPtr.Zero;
                res = (MsiResult)MsiAPI.MsiViewFetch(Handle, out recordPtr);

                if (res == MsiResult.Success)
                {
                    var recordObj = new MsiViewRecord(recordPtr, this);
                    _Records.Add(recordObj);
                    yield return recordObj;
                }
                else if (recordPtr != IntPtr.Zero)
                    MsiAPI.MsiCloseHandle(recordPtr);
            }
        }

        #endregion

        #region Columns info

        private void GetColumnsInfo()
        {
            MsiRecord namesInfo = null, typesInfo = null;
            try
            {
                namesInfo = GetColumnsInfo(MSICOLINFO.Names);
                typesInfo = GetColumnsInfo(MSICOLINFO.Types);

                _Columns = new MsiColumnInfo[namesInfo.FieldCount];
                for (int i = 0; i < namesInfo.FieldCount; i++)
                {
                    _Columns[i] = MsiColumnInfo.ParseColumnInfo(
                        namesInfo.GetString(i + 1),
                        typesInfo.GetString(i + 1));
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (namesInfo != null)
                    namesInfo.Dispose();
                if (typesInfo != null)
                    typesInfo.Dispose();
            }
        }

        private MsiRecord GetColumnsInfo(MSICOLINFO info)
        {
            var infoPtr = IntPtr.Zero;
            var res = (MsiResult)MsiAPI.MsiViewGetColumnInfo(Handle, (int)info, out infoPtr);
            if (res != MsiResult.Success)
                throw MsiAPI.GetMsiResultException(res);

            try
            {
                return new MsiRecord(infoPtr);
            }
            catch
            {
                if (infoPtr != IntPtr.Zero)
                    MsiAPI.MsiCloseHandle(infoPtr);
                throw;
            }
        }

        #endregion

        ~MsiView()
        {
            Dispose();
        }

        public override void Dispose()
        {
            DisposeRecords();
            base.Dispose();
        }

        private void DisposeRecords()
        {
            if (Records.Count > 0)
            {
                _Records.ForEach(r => r.Dispose());
                _Records.Clear();
                MsiAPI.MsiViewClose(Handle);
            }
        }
    }
}
