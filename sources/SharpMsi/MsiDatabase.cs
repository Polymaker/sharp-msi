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
        private readonly OpenDatabaseMode _AccessMode;
        private readonly string _Filepath;
        private Dictionary<string, string> _Properties;
        private List<MsiTable> _Tables;

        public string Filepath
        {
            get { return _Filepath; }
        }

        public Dictionary<string, string> Properties
        {
            get { return _Properties; }
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
            _Properties = new Dictionary<string, string>();
            _Tables = new List<MsiTable>();
            LoadProperties();
        }

        public void LoadProperties()
        {
            _Properties.Clear();
            using (var view = OpenView("SELECT * FROM `Property`"))
            {
                foreach (var record in view.ExecuteQuery())
                    _Properties.Add(record.GetString(1), record.GetString(2));
            }
            //foreach (var record in Query("SELECT * FROM Property"))
            //{
            //    _Properties.Add(record.GetString(1), record.GetString(2));
            //}
        }

        private void GetTableList()
        {
            _Tables.Clear();
            using (var view = OpenView("SELECT * FROM `_Tables`"))
            {
                foreach (var record in view.ExecuteQuery())
                    _Tables.Add(new MsiTable(this, record.GetString(1)));
            }
            //foreach (var record in Query("SELECT * FROM _Tables"))
            //{
            //    _Tables.Add(new MsiTable(this, record.GetString(1)));
            //}
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

        public MsiView OpenView(string query)
        {
            return MsiView.Open(this, query);
        }

        public IEnumerable<MsiViewRecord> Query(string query)
        {
            var view = MsiView.Open(this, query);
            foreach (var record in view.ExecuteQuery())
                yield return record;
            //yield break;
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

        ~MsiDatabase()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (AccessMode == OpenDatabaseMode.CreateDirect || AccessMode == OpenDatabaseMode.Direct)
            {
                MsiAPI.MsiDatabaseCommit(Handle);
            }
            base.Dispose();
        }
    }
}
