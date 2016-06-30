using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMsi.Native;

namespace SharpMsi.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var msiFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.msi");
            if (msiFiles.Length == 0)
            {
                Trace.WriteLine("No .msi file found");
                return;
            }

            var msiInstaller = MsiDatabase.Open(msiFiles[0], OpenDatabaseMode.Direct);
            //var propVal = msiInstaller.Summary.GetProperty(13);
            //var testTable = msiInstaller.GetTableInfo(MsiDatabaseTables._Streams);
            //var records = testTable.Query().ToList();

            //var testEntry = records.FirstOrDefault(x => x.GetString(1).Contains("SummaryInformation"));
            //if (testEntry != null)
            //{
            //    var entryData = testEntry.GetStream(2);
            //    var dataBytes = entryData.ToArray();
            //    NativeUtils.PrintByteArray(dataBytes);
            //}


            //foreach (var table in msiInstaller.Tables)
            //{
            //    Trace.WriteLine(string.Format("Table '{0}' has {1} columns:", table.Name, table.Columns.Length));
            //    //foreach (var tableCol in table.Columns)
            //    //    Trace.WriteLine(string.Format("  Column '{0}' type {1}", tableCol.Name, tableCol.GetSqlSyntax()));
            //}
            //var view = msiInstaller.OpenView("SELECT * FROM Property");

            //view.Dispose();

            msiInstaller.Dispose();
        }


    }
}
