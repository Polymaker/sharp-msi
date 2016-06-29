using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace SharpMsi
{
    public class MsiError
    {
        // Fields...
        private readonly int _Code;
        private readonly string _Message;
        private readonly string _FormatedMessage;

        public int Code
        {
            get { return _Code; }
        }

        public string Message
        {
            get { return _Message; }
        }

        public string FormatedMessage
        {
            get { return _FormatedMessage ?? _Message; }
        }

        public MsiError(int code, string message)
        {
            _Code = code;
            _Message = message;
            _FormatedMessage = null;
        }

        public MsiError(int code, string message, string formatedMessage)
        {
            _Code = code;
            _Message = message;
            _FormatedMessage = formatedMessage;
        }

        #region Error formating

        private static XDocument ErrorList;

        public static string GetErrorMessage(int errorCode)
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

        #endregion
    }
}
