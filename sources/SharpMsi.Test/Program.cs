using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var msiInstaller = MsiDatabase.Open(@"blender-2.77a-windows64.msi", OpenDatabaseMode.Direct);
            try
            {
                var test = msiInstaller.Query("select * from MyAss").First();
            }
            catch { }
            //foreach (var table in msiInstaller.Tables)
            //{
            //    Trace.WriteLine(string.Format("Table '{0}' has {1} columns:", table.Name, table.Columns.Length));
            //    foreach (var tableCol in table.Columns)
            //        Trace.WriteLine(string.Format("  Column '{0}' type {1}", tableCol.Name, tableCol.GetSqlSyntax()));
            //}
            //var view = msiInstaller.OpenView("SELECT * FROM Property");
 
            //view.Dispose();

            msiInstaller.Dispose();
        }


    }
}
