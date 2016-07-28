using SharpMsi.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi.Tables
{
    public class TableRecord : IDisposable
    {
        private bool _IsDeleted;
        private bool _IsNew;
        private MsiViewRecord record;

        public bool IsNew
        {
            get { return _IsNew; }
        }

        public bool IsDeleted
        {
            get { return _IsDeleted; }
        }

        public TableRecord()
        {
            _IsNew = true;
            _IsDeleted = false;
            record = null;
        }

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

            if(MsiViewModify(MSIMODIFY.DELETE))
                _IsDeleted = true;
        }

        public void Refresh()
        {
            if (IsNew || IsDeleted || record == null)
                return;
            MsiAPI.MsiViewModify(record.View.Handle, (int)MSIMODIFY.REFRESH, record.Handle);
        }

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
    }
}
