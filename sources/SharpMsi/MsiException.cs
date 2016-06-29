using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi
{
    public class MsiException : Exception
    {
        private readonly MsiResult _Result;
        private readonly int _ErrorCode;

        public int ErrorCode
        {
            get { return _ErrorCode; }
        }

        public MsiResult Result
        {
            get { return _Result; }
        }

        public MsiException(int errorCode, string message, MsiResult result)
            : base(message)
        {
            _ErrorCode = errorCode;
            _Result = result;
        }
    }
}
