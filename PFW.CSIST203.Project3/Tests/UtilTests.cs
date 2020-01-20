using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PFW.CSIST203.Project3.Tests
{
    /// <summary>
    /// Testing harness for the Util class
    /// </summary>
    public abstract class UtilTests : TestBase
    {

        /// <summary>
        /// GetExcelConnectionString() method testing harness
        /// </summary>
        [TestClass]
        public class GetExcelConnectionStringMethod : UtilTests
        {

            /// <summary>
            /// Verify a correct connection string for 2007 on onward file format with header option specified as yes
            /// </summary>
            [TestMethod]
            public void Excel2007FilenameWithHeader()
            {
                var connectionString = Util.GetExcelConnectionString("bogus.xlsx", true);
                Assert.IsTrue(connectionString.IndexOf("HDR=Yes", StringComparison.OrdinalIgnoreCase) >= 0, "Header option not specified in excel connection string");
            }

            /// <summary>
            /// Verify a correct connection string for pre-2007 excel file format with header option specified as no
            /// </summary>
            [TestMethod]
            public void Excel2007FilenameWithoutHeader()
            {
                var connectionString = Util.GetExcelConnectionString("bogus.xlsx", false);
                Assert.IsTrue(connectionString.IndexOf("HDR=No", StringComparison.OrdinalIgnoreCase) >= 0, "Header option not specified in excel connection string");
            }

            /// <summary>
            /// Utility method that verifies the current machine can actually use the Microsoft Excel provider specified in the connection string
            /// </summary>
            [TestMethod]
            public void ExcelOleDb12ProviderIsRegistereOnLocalMachine()
            {
                var directory = GetMethodSpecificWorkingDirectory();
                var tmpExcelFile = System.IO.Path.Combine(directory, "data005.xlsx");
                CopyEmbeddedResourceBaseToDirectory("PFW.CSIST203.Project3.Tests.Resources.Util.ExcelOleDb12ProviderIsRegistereOnLocalMachine", directory);
                Assert.IsTrue(System.IO.File.Exists(tmpExcelFile), "Unable to extract testing excel file from the embedded assembly");

                using (var table = new System.Data.DataTable("Sheet1"))
                {
                    try
                    {
                        using (System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(Util.GetExcelConnectionString(tmpExcelFile, true)))
                        {
                            using (System.Data.IDbCommand cmd = connection.CreateCommand())
                            {
                                cmd.CommandText = "SELECT * FROM [Sheet1$]";
                                connection.Open();
                                using (System.Data.IDataReader dr = cmd.ExecuteReader())
                                {
                                    table.Load(dr);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if ((ex.Message.IndexOf("Microsoft.ACE.OLEDB.12.0' provider is not registered on the local machine", StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            logger.Error("Please install the Microsoft Access Database Engine 2016 Redistributable: https://www.microsoft.com/en-us/download/details.aspx?id=54920", ex);
                            throw new System.Exception("Please install the Microsoft Access Database Engine 2016 Redistributable: https://www.microsoft.com/en-us/download/details.aspx?id=54920");
                        }

                        logger.Error("Problem reading excel file: " + tmpExcelFile, ex);
                        throw;
                    }
                }
            }
        }

        [TestClass]
        public class GetAccessConnectionString : UtilTests
        {

            /// <summary>
            /// Utility method that verifies the current machine can actually use the Microsoft Access provider specified in the connection string
            /// </summary>
            [TestMethod]
            public void AccessOleDb12ProviderIsRegistereOnLocalMachine()
            {
                var directory = GetMethodSpecificWorkingDirectory();
                var tmpAccessDatabase = System.IO.Path.Combine(directory, "sample-data.accdb");
                CopyEmbeddedResourceBaseToDirectory("PFW.CSIST203.Project3.Tests.Resources.Data", directory);
                Assert.IsTrue(System.IO.File.Exists(tmpAccessDatabase), "Unable to extract testing access database file from the embedded assembly");

                using (var table = new System.Data.DataTable("tblEmployees"))
                {
                    try
                    {
                        using (System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(Util.GetAccessConnectionString(tmpAccessDatabase)))
                        {
                            using (System.Data.IDbCommand cmd = connection.CreateCommand())
                            {
                                cmd.CommandText = "SELECT * FROM [tblEmployees]";
                                connection.Open();
                                using (System.Data.IDataReader dr = cmd.ExecuteReader())
                                {
                                    table.Load(dr);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if ((ex.Message.IndexOf("Microsoft.ACE.OLEDB.12.0' provider is not registered on the local machine", StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            logger.Error("Please install the Microsoft Access Database Engine 2016 Redistributable: https://www.microsoft.com/en-us/download/details.aspx?id=54920", ex);
                            throw new System.Exception("Please install the Microsoft Access Database Engine 2016 Redistributable: https://www.microsoft.com/en-us/download/details.aspx?id=54920");
                        }

                        logger.Error("Problem reading access database file: " + tmpAccessDatabase, ex);
                        throw;
                    }
                }
            }
        }
    }

}
