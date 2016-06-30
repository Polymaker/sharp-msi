using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi
{
    public class MsiColumnInfo
    {
        private readonly string _Name;
        private readonly MsiColumnType _Type;
        private readonly int _Length;
        private readonly bool _Nullable;

        public string Name
        {
            get { return _Name; }
        }

        public MsiColumnType Type
        {
            get { return _Type; }
        }
        
        public int Length
        {
            get { return _Length; }
        }

        public bool Nullable
        {
            get { return _Nullable; }
        }

        private MsiColumnInfo()
        {
            _Name = String.Empty;
            _Type = MsiColumnType.String;
            _Length = 0;
            _Nullable = true;
        }

        public MsiColumnInfo(string name, MsiColumnType type, int length, bool nullable)
        {
            _Name = name;
            _Type = type;
            _Length = length;
            _Nullable = nullable;
        }

        public string GetSqlSyntax()
        {
            string sqlDef = string.Empty;
            switch (Type)
            {
                case MsiColumnType.String:
                    sqlDef = Length > 0 ? "CHAR(#)" : "LONGCHAR";
                    break;
                case MsiColumnType.LocalizableString:
                    sqlDef = Length > 0 ? "CHAR(#) LOCALIZABLE" : "LONGCHAR LOCALIZABLE";
                    break;
                case MsiColumnType.Number:
                    sqlDef = Length == 2 ? "INT" : "LONG";
                    break;
                case MsiColumnType.Object:
                    sqlDef = "OBJECT";
                    break;
            }
            if (sqlDef.Contains("#"))
            {
                sqlDef = sqlDef.Replace("#", Length.ToString());
            }
            if (!Nullable)
                sqlDef += " NOT NULL";
            return sqlDef;
        }

        public string GetMsiDefinition()
        {
            string msiDef = string.Empty;

            switch (Type)
            {
                case MsiColumnType.String:
                    msiDef = "s";
                    break;
                case MsiColumnType.LocalizableString:
                    msiDef = "l";
                    break;
                case MsiColumnType.Number:
                    msiDef = "i";
                    break;
                case MsiColumnType.Object:
                    msiDef = "v";
                    break;
            }

            if (Nullable)
                msiDef = msiDef.ToUpper();

            return msiDef + Length;
        }

        internal static MsiColumnInfo ParseColumnInfo(string name, string columnDescriptor)
        {
            var colTypeInfo = ParseDescriptor(columnDescriptor);
            return new MsiColumnInfo(name, colTypeInfo.Type, colTypeInfo.Size, colTypeInfo.Nullable);
        }

        private static ColTypeInfo ParseDescriptor(string typeStr)
        {
            MsiColumnType colType;
            switch (typeStr.ToLower()[0])
            {
                case 's':
                    colType = MsiColumnType.String;
                    break;
                case 'l':
                    colType = MsiColumnType.LocalizableString;
                    break;
                case 'i':
                    colType = MsiColumnType.Number;
                    break;
                case 'v':
                    colType = MsiColumnType.Object;
                    break;
                default:
                    throw new NotImplementedException("Column type descriptor : " + typeStr);
            }
            return new ColTypeInfo()
            {
                Type = colType,
                Size = int.Parse(typeStr.Substring(1)),
                Nullable = Char.IsUpper(typeStr[0])
            };
        }

        class ColTypeInfo
        {
            public MsiColumnType Type { get; set; }
            public int Size { get; set; }
            public bool Nullable { get; set; }
        }
    }
}
