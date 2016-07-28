using SharpMsi.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SharpMsi.Tables
{
    public abstract class TableRecord : IDisposable
    {
        private bool _IsDeleted;
        private bool _IsNew;
        private MsiViewRecord record;

        #region Properties

        public bool IsNew
        {
            get { return _IsNew; }
        }

        public bool IsDeleted
        {
            get { return _IsDeleted; }
        }

        protected TableTypeMapping Mapping
        {
            get
            {
                if (!ClassInfos.ContainsKey(GetType()))
                {
                    return null;
                }
                return ClassInfos[GetType()];
            }
        }

        #endregion

        public TableRecord()
        {
            _IsNew = true;
            _IsDeleted = false;
            record = null;
            if (GetType() != typeof(TableRecord))
            {

            }
        }

        #region CRUD

        public void Save()
        {
            if (IsDeleted || record == null)
                return;

            if (IsNew)
            {
                if (MsiViewModify(MSIMODIFY.INSERT))
                    _IsNew = false;
            }
            else
            {
                MsiViewModify(MSIMODIFY.UPDATE);
            }
        }

        public void Delete()
        {
            if (IsDeleted || record == null)
                return;

            if (IsNew)
            {
                _IsDeleted = true;
                return;
            }

            if (MsiViewModify(MSIMODIFY.DELETE))
                _IsDeleted = true;
        }

        public void Refresh()
        {
            if (IsNew || IsDeleted || record == null)
                return;
            MsiAPI.MsiViewModify(record.View.Handle, (int)MSIMODIFY.REFRESH, record.Handle);
        }

        #endregion

        private bool MsiViewModify(MSIMODIFY action)
        {
            try
            {
                MsiAPI.MsiViewModify(record.View.Handle, (int)action, record.Handle);
                return true;
            }
            catch { throw; }
        }

        public void Dispose()
        {
            if (record != null)
            {
                record.Dispose();
                record = null;
            }
        }

        protected static Dictionary<Type, TableTypeMapping> ClassInfos;

        static TableRecord()
        {
            ClassInfos = new Dictionary<Type, TableTypeMapping>();
        }

        protected class TableTypeMapping
        {
            public string TableName { get; set; }
            public Type ClassType { get; set; }
            public List<ColumnMemberMapping> Fields { get; set; }
        }

        protected class ColumnMemberMapping
        {
            public MemberInfo ClassMember { get; set; }
            public int FieldIndex { get; set; }
            public MsiTableColumn ColumnInfo { get; set; }
        }

        private TableTypeMapping GetTableMapping()
        {
            var myType = GetType();
            if (ClassInfos.ContainsKey(myType))
            {
                return ClassInfos[myType];
            }
            return null;
        }
    }
}
