using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PFW.CSIST203.Project3.Tests.Persisters.Access
{
    /// <summary>
    /// Root testing harness for the AccessPersister
    /// </summary>
    public abstract class AccessPersisterTests : TestBase
    {

        /// <summary>
        /// Refactored method for creating a persister that uses the parameterless constructor
        /// </summary>
        /// <returns>A persister that should contain no data</returns>
        protected PFW.CSIST203.Project3.Persisters.Access.AccessPersister CreatePersister()
        {
            PFW.CSIST203.Project3.Persisters.Access.AccessPersister obj = null;

            AssertDelegateSuccess(() =>
            {
                obj = new Project3.Persisters.Access.AccessPersister();
            }, "Instantiation of the default constructor should not throw an exception");

            return obj;
        }

        /// <summary>
        /// Refactored method for creating a persister that uses the sample access database embedded into the project
        /// </summary>
        /// <param name="workingDirectory">The method specific working directory</param>
        /// <returns>A persister instantiated using the sample access database</returns>
        protected PFW.CSIST203.Project3.Persisters.Access.AccessPersister CreatePersister(string workingDirectory)
        {
            PFW.CSIST203.Project3.Persisters.Access.AccessPersister obj = null;
            var tmpAccessDatabaseFile = System.IO.Path.Combine(workingDirectory, "sample-data.accdb");
            CopyEmbeddedResourceBaseToDirectory("PFW.CSIST203.Project3.Tests.Resources.Data", workingDirectory);
            Assert.IsTrue(System.IO.File.Exists(tmpAccessDatabaseFile), "The sample data access database file was not found");

            AssertDelegateSuccess(() =>
            {
                obj = new Project3.Persisters.Access.AccessPersister(tmpAccessDatabaseFile);
            }, "Instantiation pointing to a file should not immediately throw an exception");

            return obj;
        }

        /// <summary>
        /// Constructor testing harness
        /// </summary>
        [TestClass]
        public class _Constructor : AccessPersisterTests
        {

            /// <summary>
            /// Default constructor testing harness
            /// </summary>
            [TestMethod]
            public void DefaultConstructor()
            {
                PFW.CSIST203.Project3.Persisters.Access.AccessPersister obj = null;

                AssertDelegateSuccess(() =>
                {
                    obj = new Project3.Persisters.Access.AccessPersister();
                }, "Instantiation using the empty constructor should not throw an exception");

                Assert.IsTrue(string.IsNullOrWhiteSpace(obj.accessFile), "The access file variable should not be set when the parameterless constructor is used");
                Assert.IsTrue(obj.noDatabase, "The boolean value indicating no backing database is available should return true");
            }

            /// <summary>
            /// Constructor passed with a file that does not exist
            /// </summary>
            [TestMethod]
            public void ConstructorWithMissingFile()
            {
                PFW.CSIST203.Project3.Persisters.Access.AccessPersister obj = null;
                var tmp = System.IO.Path.Combine(GetMethodSpecificWorkingDirectory(), System.Guid.NewGuid().ToString() + ".accdb");

                AssertDelegateFailure(() =>
                {
                    obj = new Project3.Persisters.Access.AccessPersister(tmp);
                }, typeof(System.IO.FileNotFoundException), "An invalid file was supplied into the constructor and should have thrown a System.IO.FileNotFoundException");

                Assert.IsNull(obj, "The variable should still be null because it could not be instantiated");
            }

            /// <summary>
            /// Constructor passed a path to an access database file that does exist
            /// </summary>
            [TestMethod]
            public void ConstructorWithValidFile()
            {
                PFW.CSIST203.Project3.Persisters.Access.AccessPersister obj = null;
                var directory = GetMethodSpecificWorkingDirectory();
                var tmpAccessDatabaseFile = System.IO.Path.Combine(directory, "sample-data.accdb");
                CopyEmbeddedResourceBaseToDirectory("PFW.CSIST203.Project3.Tests.Resources.Data", directory);
                Assert.IsTrue(System.IO.File.Exists(tmpAccessDatabaseFile), "The sample data access database file was not found");

                AssertDelegateSuccess(() =>
                {
                    obj = new Project3.Persisters.Access.AccessPersister(tmpAccessDatabaseFile);
                }, "Instantiation pointing to a file should not immediately throw an exception");

                Assert.AreEqual(tmpAccessDatabaseFile, obj.accessFile, "The local variable should be set to the supplied file in the constructor");
                Assert.IsFalse(obj.noDatabase, "A database file was supplied, so the boolean value should be set to false");
            }
        }

        /// <summary>
        /// CountRows() method testing harness
        /// </summary>
        [TestClass]
        public class CountRowsMethod : AccessPersisterTests
        {

            /// <summary>
            /// The parameterless constructor should return zero when this method is called
            /// </summary>
            [TestMethod]
            public void DefaultConstructedObjectReturnsZero()
            {
                var obj = CreatePersister();
                var result = 0;

                AssertDelegateSuccess(() =>
                {
                    result = obj.CountRows();
                }, "A persister created with the default constructor should not throw an exception when the CountRows() method is called");

                Assert.AreEqual(0, result, "A persister created with the default constructor should have no data");
            }

            /// <summary>
            /// The method should return the current count of entries in the access database
            /// </summary>
            [TestMethod]
            public void RowCountIsCorrect()
            {
                var obj = CreatePersister(GetMethodSpecificWorkingDirectory());
                var result = 0;

                AssertDelegateSuccess(() =>
                {
                    result = obj.CountRows();
                }, "A call to the CountRows() method should not throw an exception when a valid database file was specified");

                Assert.AreEqual(9, result, "The sample data did not return the expected count");
            }

            /// <summary>
            /// Calling this method after the Dispose() call should throw an exception
            /// </summary>
            [TestMethod]
            public void ThrowsObjectDisposedAfterDisposeCalled()
            {
                var obj = CreatePersister();
                AssertDelegateSuccess(() =>
                {
                    obj.Dispose();
                }, "The Dispose() method should not throw an exception");

                AssertDelegateFailure(() =>
                {
                    obj.CountRows();
                }, typeof(System.ObjectDisposedException), "A disposed object should throw an exception when the method is called");
            }
        }

        /// <summary>
        /// Dispose() method testing harness
        /// </summary>
        [TestClass]
        public class DisposeMethod : AccessPersisterTests
        {

            /// <summary>
            /// Calling this method should set the IsDisposed variable to True
            /// </summary>
            [TestMethod]
            public void DisposeSetsIsDisposedVariable()
            {
                var obj = CreatePersister();
                AssertDelegateSuccess(() =>
                {
                    obj.Dispose();
                }, "The Dispose() method should not throw an exception");

                Assert.IsTrue(obj.IsDisposed, "Once Dispose() is called, the local IsDisposed variable should be true");
            }

            /// <summary>
            /// Calling the Dispose() method multiple times should not throw an exception
            /// </summary>
            [TestMethod]
            public void MultipleDisposeCallsDoNotThrowException()
            {
                var obj = CreatePersister();
                AssertDelegateSuccess(() =>
                {
                    obj.Dispose();
                }, "The Dispose() method should not throw an exception");
                Assert.IsTrue(obj.IsDisposed, "Once Dispose() is called, the local IsDisposed variable should be true");

                AssertDelegateSuccess(() =>
                {
                    obj.Dispose();
                }, "The Dispose() method should not throw an exception");
                Assert.IsTrue(obj.IsDisposed, "Once Dispose() is called, the local IsDisposed variable should be true");

                AssertDelegateSuccess(() =>
                {
                    obj.Dispose();
                }, "The Dispose() method should not throw an exception");
                Assert.IsTrue(obj.IsDisposed, "Once Dispose() is called, the local IsDisposed variable should be true");
            }
        }

        /// <summary>
        /// GetRow() testing harness
        /// </summary>
        [TestClass]
        public class GetRowMethod : AccessPersisterTests
        {

            /// <summary>
            /// Ensure the DataRow returned from the persister contains the correct data
            /// </summary>
            [TestMethod]
            public void ReadsDatabaseFileCorrectly()
            {
                var obj = CreatePersister(GetMethodSpecificWorkingDirectory());
                DataRow row = null;

                AssertDelegateSuccess(() =>
                {
                    row = obj.GetRow(4);
                }, "The GetRow() method should not have thrown an exception");

                Assert.AreEqual(4, row["ID"], "The database value in the sample file did not match the expected result");
                Assert.AreEqual("Northwind Traders", row["Company"], "The database value in the sample file did not match the expected result");
                Assert.AreEqual("Sergienko", row["Last Name"], "The database value in the sample file did not match the expected result");
                Assert.AreEqual("Mariya", row["First Name"], "The database value in the sample file did not match the expected result");
                Assert.AreEqual("mariya@northwindtraders.com", row["E-mail Address"], "The database value in the sample file did not match the expected result");
                Assert.AreEqual("Sales Representative", row["Job Title"], "The database value in the sample file did not match the expected result");
                Assert.AreEqual("(123)555-0104", row["Business Phone"], "The database value in the sample file did not match the expected result");
            }

            /// <summary>
            /// Requesting data for an ID that does not exist should return a null (Nothing) DataRow
            /// </summary>
            [TestMethod]
            public void ReturnsNullIfNoData()
            {
                var obj = CreatePersister();
                DataRow row = null;

                AssertDelegateSuccess(() =>
                {
                    row = obj.GetRow(123);
                }, "A parameterless constructed persister should not throw an exception when calling GetRow()");

                Assert.IsNull(row, "Row object should have been null");
            }

            /// <summary>
            /// If the Access Database is modified by another application the persister should pick up that change when it reloads the data
            /// </summary>
            [TestMethod]
            public void PicksUpExternalDatabaseChanges()
            {
                var obj = CreatePersister(GetMethodSpecificWorkingDirectory());
                DataRow row = null;

                AssertDelegateSuccess(() =>
                {
                    row = obj.GetRow(2);
                }, "Retrieval of a specific record in the access database should not throw an exception");

                // ensure the state of the database
                Assert.AreEqual(2, row["ID"], "The database value in the sample file did not match the expected result");
                Assert.AreEqual("Northwind Traders", row["Company"], "The database value in the sample file did not match the expected result");
                Assert.AreEqual("Cencini", row["Last Name"], "The database value in the sample file did not match the expected result");
                Assert.AreEqual("Andrew", row["First Name"], "The database value in the sample file did not match the expected result");
                Assert.AreEqual("andrew@northwindtraders.com", row["E-mail Address"], "The database value in the sample file did not match the expected result");
                Assert.AreEqual("Vice President, Sales", row["Job Title"], "The database value in the sample file did not match the expected result");
                Assert.AreEqual("(123)555-0102", row["Business Phone"], "The database value in the sample file did not match the expected result");

                // change a few values in the DataRow before persisting these changes back to the database
                row["Company"] = "Acme Products, LLC";
                row["Business Phone"] = "(260)123-4567";

                // perform a manual modification to the record above that does not utilize the UpdateRow() method
                using (System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(Util.GetAccessConnectionString(obj.accessFile)))
                {
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE [tblEmployees] SET [Company] = @Company,[Last Name] = @LastName,[First Name] = @FirstName,[E-mail Address] = @EmailAddress,[Job Title] = @JobTitle,[Business Phone] = @BusinessPhone WHERE [ID] = @ID";

                        // company parameter
                        var par = cmd.CreateParameter();
                        par.ParameterName = "@Company";
                        par.DbType = DbType.String;
                        par.Value = row["Company"];
                        cmd.Parameters.Add(par);

                        // last name
                        par = cmd.CreateParameter();
                        par.ParameterName = "@LastName";
                        par.DbType = DbType.String;
                        par.Value = row["Last Name"];
                        cmd.Parameters.Add(par);

                        // first name
                        par = cmd.CreateParameter();
                        par.ParameterName = "@FirstName";
                        par.DbType = DbType.String;
                        par.Value = row["First Name"];
                        cmd.Parameters.Add(par);

                        // e-mail address
                        par = cmd.CreateParameter();
                        par.ParameterName = "@EmailAddress";
                        par.DbType = DbType.String;
                        par.Value = row["E-mail Address"];
                        cmd.Parameters.Add(par);

                        // job title
                        par = cmd.CreateParameter();
                        par.ParameterName = "@JobTitle";
                        par.DbType = DbType.String;
                        par.Value = row["Job Title"];
                        cmd.Parameters.Add(par);

                        // business phone
                        par = cmd.CreateParameter();
                        par.ParameterName = "@BusinessPhone";
                        par.DbType = DbType.String;
                        par.Value = row["Business Phone"];
                        cmd.Parameters.Add(par);

                        par = cmd.CreateParameter();
                        par.ParameterName = "@ID";
                        par.DbType = DbType.Int32;
                        par.Value = row["ID"];
                        cmd.Parameters.Add(par);

                        var results = cmd.ExecuteNonQuery();

                        Assert.AreEqual(1, results, "The number of expected results from the database update was not returned");
                    }
                }

                // Read the modified row using the persister
                AssertDelegateSuccess(() =>
                {
                    row = obj.GetRow(2);
                }, "Retrieval of a specific record in the access database should not throw an exception");

                Assert.AreEqual("Acme Products, LLC", row["Company"], "The database value in the sample file did not match the expected result after the external modification");
                Assert.AreEqual("(260)123-4567", row["Business Phone"], "The database value in the sample file did not match the expected result after the external modification");
            }

            /// <summary>
            /// Calling this method should throw an ObjectDisposedException if the Dispose() method has been previously called
            /// </summary>
            [TestMethod]
            public void ThrowsObjectDisposedAfterDisposeCalled()
            {
                var obj = CreatePersister();
                AssertDelegateSuccess(() =>
                {
                    obj.Dispose();
                }, "The Dispose() method should not throw an exception");

                AssertDelegateFailure(() =>
                {
                    obj.GetRow(0);
                }, typeof(System.ObjectDisposedException), "A disposed object should throw an exception when the method is called");
            }
        }

        /// <summary>
        /// GetData() method testing harness
        /// </summary>
        [TestClass]
        public class GetDataMethod : AccessPersisterTests
        {

            /// <summary>
            /// The parameterless constructor should cause this method to return an empty data table
            /// </summary>
            [TestMethod]
            public void ParameterlessConstructorReturnsEmptyDataTable()
            {
                var obj = CreatePersister();
                System.Data.DataTable data = null;

                AssertDelegateSuccess(() =>
                {
                    data = obj.GetData();
                }, "Requesting all data contained in the underlying database file should not throw an exception");

                Assert.IsNotNull(data, "The DataTable returned by the method should be non-null");
                Assert.AreEqual(0, data.Rows.Count, "The number of data rows contained in a parameterless constructed persister should be zero");
            }

            /// <summary>
            /// A call to this method should return a DataTable that contains all data from the Access Database table
            /// </summary>
            [TestMethod]
            public void AccessDatabaseFileReturnsAllResults()
            {
                var obj = CreatePersister(GetMethodSpecificWorkingDirectory());
                System.Data.DataTable data = null;

                AssertDelegateSuccess(() =>
                {
                    data = obj.GetData();
                }, "Requesting all data contained in the underlying database file should not throw an exception");

                Assert.IsNotNull(data, "The DataTable returned by the method should be non-null");

                // perform a raw check of the testing database to ensure the persister isn't faking the results
                // NOTE: This section may also fail if the persister does not Dispose() of any open OleDbConnection objects properly
                // with a Using statement (as below) or by simply calling Dispose() on the object once finished
                var rawCount = 0;
                using (System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(Util.GetAccessConnectionString(obj.accessFile)))
                {
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM [tblEmployees]";
                        rawCount = int.Parse(cmd.ExecuteScalar().ToString().Trim());
                    }
                }

                Assert.AreEqual(rawCount, data.Rows.Count, "The persister and raw OleDb command check did not return identical results");
            }

            /// <summary>
            /// Calling this method should throw an ObjectDisposedException if the Dispose() method has been previously called
            /// </summary>
            [TestMethod]
            public void ThrowsObjectDisposedAfterDisposeCalled()
            {
                var obj = CreatePersister();
                AssertDelegateSuccess(() =>
                {
                    obj.Dispose();
                }, "The Dispose() method should not throw an exception");

                AssertDelegateFailure(() =>
                {
                    obj.GetData();
                }, typeof(System.ObjectDisposedException), "A call into this method should throw a System.ObjectDisposedException");
            }
        }

        /// <summary>
        /// UpdateRow() method testing harness
        /// </summary>
        [TestClass]
        public class StoreRowMethod : AccessPersisterTests
        {

            /// <summary>
            /// Retrieval of an existing record, changing it and then updating it should return that value during subsequent loads
            /// </summary>
            [TestMethod]
            public void UpdateExistingRecordWorksWithPersister()
            {
                var obj = CreatePersister(GetMethodSpecificWorkingDirectory());
                System.Data.DataRow row = null;

                AssertDelegateSuccess(() =>
                {
                    row = obj.GetRow(6);
                }, "Retrieval of a specific record in the access database should not throw an exception");

                Assert.AreEqual("Sales Representative", row["Job Title"], "The expected value was not read from the sample access database");

                // change their job title
                row["Job Title"] = "Sales Manager";
                row["Business Phone"] = "(123)555-5119";

                // send the row to the access database to be updated
                AssertDelegateSuccess(() =>
                {
                    obj.StoreRow(row);
                }, "Updating a row should not throw an exception");

                // ask the persister for the row again and ensure it matches the expected value
                AssertDelegateSuccess(() =>
                {
                    row = obj.GetRow(6);
                }, "Retrieval of a specific record in the access database should not throw an exception");

                Assert.AreEqual("Sales Manager", row["Job Title"], "The expected value was not read from the sample access database");
                Assert.AreEqual("(123)555-5119", row["Business Phone"], "The expected value was not read from the sample access database");
            }

            /// <summary>
            /// Once the persister has updated a row, connect to the database manually and verify the write occurred as expected
            /// </summary>
            [TestMethod]
            public void UpdateExistingRecordWorksWithOleDb()
            {
                var obj = CreatePersister(GetMethodSpecificWorkingDirectory());
                System.Data.DataRow row = null;

                AssertDelegateSuccess(() =>
                {
                    row = obj.GetRow(6);
                }, "Retrieval of a specific record in the access database should not throw an exception");

                Assert.AreEqual("Sales Representative", row["Job Title"], "The expected value was not read from the sample access database");

                // change their job title
                row["Job Title"] = "Sales Supervisor";
                row["Business Phone"] = "(123)555-6119";

                // send the row to the access database to be updated
                AssertDelegateSuccess(() =>
                {
                    obj.StoreRow(row);
                }, "Updating a row should not throw an exception");

                // retrieve the row in question using a raw OleDbConnection
                System.Data.DataTable dt = new System.Data.DataTable();
                using (System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(Util.GetAccessConnectionString(obj.accessFile)))
                {
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM [tblEmployees] WHERE [ID] = @ID";

                        // create the single parameter that selects the specific row we are interested in
                        var par = cmd.CreateParameter();
                        par.ParameterName = "@ID";
                        par.DbType = DbType.Int32;
                        par.Value = 6;
                        cmd.Parameters.Add(par);

                        using (var dr = cmd.ExecuteReader())
                        {
                            dt.Load(dr);
                        }
                    }
                }

                Assert.AreEqual(1, dt.Rows.Count, "The sample data should only have returned a single result");
                Assert.AreEqual(row.Table.Columns.Count, dt.Columns.Count, "The column count from the two different retrieval methods did not match exactly");

                // iterate over all of the columns owned by each of the data sources
                var OleDbMethodRow = dt.Rows[0];
                foreach (System.Data.DataColumn column in row.Table.Columns)
                    Assert.AreEqual(row[column.ColumnName], OleDbMethodRow[column.ColumnName], "The column value updated did not match the value read from the access database for column: " + column.ColumnName);
            }

            /// <summary>
            /// If an invalid ID is specified during the update operation an exception should be thrown indicating it failed
            /// </summary>
            [TestMethod]
            public void UpdateMissingRecordThrowsException()
            {
                var obj = CreatePersister(GetMethodSpecificWorkingDirectory());
                System.Data.DataRow row = null;

                AssertDelegateSuccess(() =>
                {
                    row = obj.GetRow(6);
                }, "Retrieval of a specific record in the access database should not throw an exception");

                // change the ID (unique identifier) to an invalid value
                row["ID"] = -25;

                // Try to update the value
                AssertDelegateFailure(() =>
                {
                    obj.StoreRow(row);
                }, typeof(System.ArgumentException), "Updating an invalid row should throw a System.ArgumentException");
            }

            /// <summary>
            /// Calling this method should throw an ObjectDisposedException if the Dispose() method has been previously called
            /// </summary>
            [TestMethod]
            public void ThrowsObjectDisposedAfterDisposeCalled()
            {
                var obj = CreatePersister(GetMethodSpecificWorkingDirectory());
                System.Data.DataRow row = null;

                AssertDelegateSuccess(() =>
                {
                    row = obj.GetRow(7);
                }, "Retrieval of a specific record in the access database should not throw an exception");

                AssertDelegateSuccess(() =>
                {
                    obj.Dispose();
                }, "The Dispose() method should not throw an exception");

                AssertDelegateFailure(() =>
                {
                    obj.StoreRow(row);
                }, typeof(System.ObjectDisposedException), "A call into this method should throw a System.ObjectDisposedException");
            }

            /// <summary>
            /// Verify that the persister can store new rows correctly in the access database
            /// </summary>
            [TestMethod]
            public void StoreNewRecordWorksCorrectly()
            {
                var obj = CreatePersister(GetMethodSpecificWorkingDirectory());
                System.Data.DataRow row = null;

                // query the database and determine the number of rows present in the database
                var currentcount = obj.CountRows();

                AssertDelegateSuccess(() =>
                {
                    row = obj.CreateRow("tblEmployees");
                }, "Creating a new row object should not throw an exception");
                Assert.IsNotNull(row, "The row object cannot be null");

                // make sure not new entry was created by simply calling the CreateRow() method
                Assert.AreEqual(currentcount, obj.CountRows(), "Simply calling the CreateRow() method should NOT have created an entry in the database");

                // assign the values for the new row
                // Company
                row["Company"] = "Acme, Inc";
                // Last Name
                row["Last Name"] = "Smith";
                // First Name
                row["First Name"] = "Joe";
                // E-mail Address
                row["E-mail Address"] = "jsmith@acme.org";
                // Job Title
                row["Job Title"] = "Senior Marketing Specialist";
                // Business Phone
                row["Business Phone"] = "(742)555-5555";

                AssertDelegateSuccess(() =>
                {
                    obj.StoreRow(row);
                }, "Storing a new entry into the database should not fail");

                Assert.AreEqual(currentcount + 1, obj.CountRows(), "A new row was not created after the successful save operation");

                // make sure the 'SELECT @@IDENTITY' initialized the ID column
                Assert.IsNotNull(row["ID"], "The ID value was not retrieved propertly from the database after the store operation");
                Assert.AreNotEqual(row["ID"], 0, "The ID must be a positive integer if set propertly by the SELECT @@IDENTITY sql query");
                Assert.IsFalse(row.IsNull("ID"), "The ID value was not retrieved propertly from the database after the store operation");

                System.Data.DataTable dt = new System.Data.DataTable();
                using (System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(Util.GetAccessConnectionString(obj.accessFile)))
                {
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM [tblEmployees] WHERE [ID] = @ID";

                        // create the single parameter that selects the specific row we are interested in
                        var par = cmd.CreateParameter();
                        par.ParameterName = "@ID";
                        par.DbType = DbType.Int32;
                        par.Value = row["ID"];
                        cmd.Parameters.Add(par);

                        using (var dr = cmd.ExecuteReader())
                        {
                            dt.Load(dr);
                        }
                    }
                }

                Assert.AreEqual(1, dt.Rows.Count, "The raw SQL query should have returned exactly one row");

                // make sure all values that were saved into the database match excactly with the values that were found using raw SQL queries
                var newRow = (DataRow)dt.Rows[0];
                foreach (DataColumn column in newRow.Table.Columns)
                    Assert.AreEqual(
                        row[column.ColumnName], newRow[column.ColumnName],
                        string.Format("The column ('{0}') value from the database ('{1}') did not match the value that was supplied during the store operation ('{2}')",
                        column.ColumnName,
                        newRow[column.ColumnName],
                        row[column.ColumnName]));
            }
        }

        [TestClass]
        public class CreateRowMethod : AccessPersisterTests
        {
            [TestMethod]
            public void SupportsEmployeeTable()
            {
                var obj = CreatePersister(GetMethodSpecificWorkingDirectory());
                System.Data.DataRow row = null;

                AssertDelegateSuccess(() =>
                {
                    row = obj.CreateRow("tblEmployees");
                }, "Creation of a row for the tblEmployees table should always work");

                Assert.IsNotNull(row, "The row object returned should be non-null");

                AssertDelegateFailure(() =>
                {
                    row = obj.CreateRow(System.Guid.NewGuid().ToString());
                }, typeof(ArgumentException), "The persister should throw an exception when an invalid table is specified");
            }

            [TestMethod]
            public void NewRowHasCorrectColumns()
            {
                var obj = CreatePersister(GetMethodSpecificWorkingDirectory());
                System.Data.DataRow row = null;

                AssertDelegateSuccess(() =>
                {
                    row = obj.CreateRow("tblEmployees");
                }, "Creation of a row for the tblEmployees table should always work");

                var columns = row.Table.Columns.Cast<DataColumn>();
                List<string> expectedColumnNames = new List<string>() { "ID", "Company", "Last Name", "First Name", "E-mail Address", "Job Title", "Business Phone" };
                List<Type> expectedColumnTypes = new List<Type>() { typeof(int), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };

                for (int i = 0; i <= expectedColumnNames.Count - 1; i++)
                {
                    var columnName = expectedColumnNames[i];
                    var column = columns.FirstOrDefault(c => string.Equals(c.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));
                    Assert.IsNotNull(column, string.Format("Column '{0}' missing from the newly created data row", columnName));
                    Assert.AreEqual(expectedColumnTypes[i], column.DataType, "The data type of the column ({0}) did not match the expected type ({1})", expectedColumnTypes[i], column.DataType);
                }
            }

            [TestMethod]
            public void ThrowsObjectDisposedAfterDisposeCalled()
            {
                var obj = CreatePersister(GetMethodSpecificWorkingDirectory());
                System.Data.DataRow row = null;

                AssertDelegateSuccess(() =>
                {
                    row = obj.CreateRow("tblEmployees");
                }, "Creating a new row object should not throw an exception");

                AssertDelegateSuccess(() =>
                {
                    obj.Dispose();
                }, "The Dispose() method should not throw an exception");

                AssertDelegateFailure(() =>
                {
                    row = obj.CreateRow("tblEmployees");
                }, typeof(System.ObjectDisposedException), "A call into this method should throw a System.ObjectDisposedException");
            }
        }

    }
}
