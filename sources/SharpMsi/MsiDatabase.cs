using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpMsi.Native;

namespace SharpMsi
{
    public class MsiDatabase : MsiObject
    {
        // Fields...
        private MsiSummaryInformation _Summary;
        private readonly OpenDatabaseMode _AccessMode;
        private readonly string _Filepath;
        private List<MsiTable> _Tables;

        public string Filepath
        {
            get { return _Filepath; }
        }

        public MsiSummaryInformation Summary
        {
            get { return _Summary; }
        }

        public MsiTable[] Tables
        {
            get
            {
                if (_Tables.Count == 0)
                    GetTableList();
                return _Tables.ToArray();
            }
        }

        public OpenDatabaseMode AccessMode
        {
            get { return _AccessMode; }
        }

        private MsiDatabase(IntPtr handle, string path, OpenDatabaseMode accessMode)
            : base(handle)
        {
            _Filepath = path;
            _AccessMode = accessMode;
            _Tables = new List<MsiTable>();
            IntPtr summaryPtr;
            MsiAPI.MsiGetSummaryInformation(handle, null, 0, out summaryPtr);
            _Summary = new MsiSummaryInformation(summaryPtr, 0);
        }

        private void GetTableList()
        {
            _Tables.Clear();

            foreach (var record in Query("SELECT * FROM _Tables"))
            {
                _Tables.Add(new MsiTable(this, record.GetString(1)));
            }
        }

        public static MsiDatabase Open(string filepath, OpenDatabaseMode mode)
        {
            filepath = Path.GetFullPath(filepath);
            IntPtr msiHandle = IntPtr.Zero;
            var res = (MsiResult)MsiAPI.MsiOpenDatabase(filepath, (IntPtr)mode, out msiHandle);
            if (res != MsiResult.Success)
                throw MsiAPI.GetMsiResultException(res);
            return new MsiDatabase(msiHandle, filepath, mode);
        }

        /// <summary>
        /// Commits changes to a database.
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            var res = (MsiResult)MsiAPI.MsiDatabaseCommit(Handle);
            return res == MsiResult.Success;
        }

        #region Views & queries

        public MsiView OpenView(string query)
        {
            return MsiView.Open(this, query);
        }

        public IEnumerable<MsiViewRecord> Query(string query)
        {
            var view = MsiView.Open(this, query);
            foreach (var record in view.ExecuteQuery())
                yield return record;
        }

        public IEnumerable<MsiViewRecord> Query(string query, params object[] parameters)
        {
            var view = MsiView.Open(this, query);
            foreach (var record in view.ExecuteQuery(parameters))
                yield return record;
        }

        public void ExecuteNonQuery(string query)
        {
            using (var view = OpenView(query))
            {
                view.Execute();
            }
        }

        public void ExecuteNonQuery(string query, params object[] parameters)
        {
            using (var view = OpenView(query))
            {
                view.Execute(parameters);
            }
        }

        #endregion

        public MsiTable GetTableInfo(MsiDatabaseTables table)
        {
            string tableName = table.ToString();
            if (Tables.Any(t => t.Name == tableName))
                return Tables.First(t => t.Name == tableName);
            var tableInfo = new MsiTable(this, tableName);

            return tableInfo;
        }

        ~MsiDatabase()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (_Summary != null)
            {
                _Summary.Dispose();
                _Summary = null;
            }
            if (AccessMode == OpenDatabaseMode.CreateDirect || 
                AccessMode == OpenDatabaseMode.Direct)
            {
                MsiAPI.MsiDatabaseCommit(Handle);
            }
            base.Dispose();
        }
    }
}
