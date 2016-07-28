using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SharpMsi.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestOpenClose()
        {
            MsiDatabase msiDB = GetDatabase(OpenDatabaseMode.ReadOnly);
            if (msiDB == null)
                return;

            try
            {
                msiDB.Close();
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Exception while closing msi (\"{0}\").\r\nException:\r\n{1}", msiDB.Filepath, ex));
                return;
            }

            msiDB = null;
        }

        [TestMethod]
        public void TestReadOnly()
        {
            MsiDatabase msiDB = GetDatabase(OpenDatabaseMode.ReadOnly);

            if (msiDB == null)
                return;

            try
            {
                msiDB.ExecuteNonQuery("INSERT INTO Property (Property, Value) VALUES (?, ?)", "MYPROP", "MYVALUE");
                Assert.Fail("Should not be able to write");
            }
            catch (MsiException ex)
            {
                Trace.WriteLine("Exception " + ex.ErrorCode);
                Trace.WriteLine(ex.Message);
            }
            finally
            {
                msiDB.Close();
                msiDB = null;
            }
        }

        [TestMethod]
        public void TestQuery()
        {
            MsiDatabase msiDB = GetDatabase(OpenDatabaseMode.ReadOnly);

            if (msiDB == null)
                return;

            try
            {
                using (var viewQuery = msiDB.OpenView("SELECT `File`.`File`, `File`.`FileName`, `Component`.`ComponentId` FROM `File`, `Component` WHERE `Component`.`Component` = `File`.`Component_`"))
                {
                    Trace.WriteLine("View type: " + viewQuery.ViewType);
                    if(viewQuery.Tables.Length == 1)
                        Trace.WriteLine("Table: " + viewQuery.Tables[0]);
                    else if (viewQuery.Tables.Length > 1)
                        Trace.WriteLine("Table: " + viewQuery.Tables.Aggregate((i,j) => i + ", " + j));
                    Trace.WriteLine(string.Format("Query has {0} columns:", viewQuery.Columns.Length));
                    foreach (var tableCol in viewQuery.Columns)
                        Trace.WriteLine(string.Format("  Column '{0}' type {1}", tableCol.Name, tableCol.GetSqlSyntax()));

                    foreach (var record in viewQuery.ExecuteQuery())
                    {
                        Trace.WriteLine("Record found:\r\n\t" + record.Trace());
                        break;
                    }
                }
                    
            }
            catch (MsiException ex)
            {
                Assert.Fail(string.Format("Exception while querying msi.\r\nException {0}:\r\n{1}", ex.ErrorCode, ex));
            }
            finally
            {
                msiDB.Close();
                msiDB = null;
            }
        }

        private static MsiDatabase GetDatabase(OpenDatabaseMode mode)
        {
            string msiPath;
            if (!FindRandomMsi(out msiPath))
            {
                Assert.Fail("Could not find any msi to open.");
                return null;
            }
            MsiDatabase msiDB;

            try
            {
                Trace.WriteLine(string.Format("Opening msi database \"{0}\"...", msiPath));
                msiDB = MsiDatabase.Open(msiPath, mode);
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Could not open msi (\"{0}\").\r\nException:\r\n{1}", msiPath, ex));
                return null;
            }

            return msiDB;
        }

        private static bool FindRandomMsi(out string filename)
        {
            string downloadFolder = Native.NativeUtils.GetKnownFolder(Native.NativeUtils.KnownFolder.Downloads);
            string[] msiFiles = Directory.GetFiles(downloadFolder, "*.msi");
            var rng = new Random();
            if (msiFiles.Length > 0)
            {
                filename = msiFiles[rng.Next(0, msiFiles.Length)];
                return true;
            }
            filename = string.Empty;
            return false;
        }
    }
}
