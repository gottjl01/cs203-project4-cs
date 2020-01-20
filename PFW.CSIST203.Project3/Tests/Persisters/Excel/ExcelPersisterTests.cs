using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PFW.CSIST203.Project3.Tests.Persisters.Excel
{
    /// <summary>
    /// ExcelDataPersister testing harness
    /// </summary>
    public abstract class ExcelDataPersisterTests : TestBase
    {
        /// <summary>
        /// Constructor testing harness
        /// </summary>
        [TestClass]
        public class _Constructor : ExcelDataPersisterTests
        {

            /// <summary>
            /// The parameterless constructor
            /// </summary>
            [TestMethod]
            public void ParameterlessConstructor()
            {
                PFW.CSIST203.Project3.Persisters.Excel.ExcelPersister obj = null;
                AssertDelegateSuccess(() => obj = new Project3.Persisters.Excel.ExcelPersister(),
                    "Instantiation of the parameterless constructor should not throw an exception");

                Assert.IsNotNull(obj.Data, "The datatable should not be null immediately after instantiation");
                Assert.IsNotNull(obj.Data.Rows, "The data row property should be non-null");
                Assert.AreEqual(0, obj.Data.Rows.Count, "The number of data rows should be exactly zero when using the parameterless constructor");
            }

            /// <summary>
            /// Single parameter pointing to a file that does not exist
            /// </summary>
            [TestMethod]
            public void ExcelFiledoesNotExist()
            {
                var tmpExcelFile = System.IO.Path.Combine(GetMethodSpecificWorkingDirectory(), System.Guid.NewGuid().ToString() + ".xlsx");
                PFW.CSIST203.Project3.Persisters.Excel.ExcelPersister obj = null;
                AssertDelegateFailure(() => obj = new Project3.Persisters.Excel.ExcelPersister(tmpExcelFile),
                    typeof(System.IO.FileNotFoundException),
                    "A non-existent excel field should throw an immediate exception");
            }

            /// <summary>
            /// Single parameter pointing to a file that does exist
            /// </summary>
            [TestMethod]
            public void ExcelFileWasReadCorrectly()
            {
                var directory = GetMethodSpecificWorkingDirectory();
                var tmpExcelFile = System.IO.Path.Combine(directory, "data001.xlsx");
                CopyEmbeddedResourceBaseToDirectory("PFW.CSIST203.Project3.Tests.Resources.Persisters.Excel.ExcelFileWasReadCorrectly", directory);
                Assert.IsTrue(System.IO.File.Exists(tmpExcelFile), "The sample excel data was not properly extracted from the executing assembly");

                PFW.CSIST203.Project3.Persisters.Excel.ExcelPersister obj = null;
                AssertDelegateSuccess(() => obj = new Project3.Persisters.Excel.ExcelPersister(tmpExcelFile),
                    "The excel document should have been read into memory without issue");
                Assert.IsNotNull(obj.Data, "The datatable should not be null after reading in the test data excel file");
                Assert.AreEqual(9, obj.Data.Rows.Count, "The expected number of rows were not read from the test excel file");
                AssertDelegateSuccess(() => obj.Dispose(), "Disposing of the object should not throw any exception");
            }
        }

        /// <summary>
        /// GetRows() method testing harness
        /// </summary>
        [TestClass]
        public class GetRowsMethod : ExcelDataPersisterTests
        {

            /// <summary>
            /// 
            /// </summary>
            [TestMethod]
            public void ReturnsNullForNonExistentRow()
            {
                var directory = GetMethodSpecificWorkingDirectory();
                var tmpExcelFile = System.IO.Path.Combine(directory, "data002.xlsx");
                CopyEmbeddedResourceBaseToDirectory("PFW.CSIST203.Project3.Tests.Resources.Persisters.Excel.ReturnsNullForNonExistentRow", directory);
                Assert.IsTrue(System.IO.File.Exists(tmpExcelFile), "The sample excel data was not properly extracted from the executing assembly");

                PFW.CSIST203.Project3.Persisters.Excel.ExcelPersister obj = null;
                AssertDelegateSuccess(() => obj = new Project3.Persisters.Excel.ExcelPersister(tmpExcelFile),
                    "The excel document should have been read into memory without issue");
                Assert.AreEqual(9, obj.Data.Rows.Count, "The expected number of rows were not read from the test excel file");

                System.Data.DataRow row = null;
                AssertDelegateSuccess(() => row = obj.GetRow(0), "Retrieving row zero should not throw an exception");

                Assert.IsNotNull(row, "row should be non-null in the test file");
                AssertDelegateSuccess(() => row = obj.GetRow(5), "Retrieving a non-zero row should not throw an exception");
                Assert.IsNotNull(row, "row should be non-null in the test file");
                AssertDelegateSuccess(() => row = obj.GetRow(100), "Retrieving a row beyond the rows available should not throw an exception");
                Assert.IsNull(row, "row should have been null");
                AssertDelegateSuccess(() => row = obj.GetRow(-1), "Retrieving a row beyond the rows available should not throw an exception");
                Assert.IsNull(row, "row should have been null");
            }

            [TestMethod]
            public void RetrievesCorrectRow()
            {
                var directory = GetMethodSpecificWorkingDirectory();
                var tmpExcelFile = System.IO.Path.Combine(directory, "data003.xlsx");
                CopyEmbeddedResourceBaseToDirectory("PFW.CSIST203.Project3.Tests.Resources.Persisters.Excel.RetrievesCorrectRow", directory);
                Assert.IsTrue(System.IO.File.Exists(tmpExcelFile), "The sample excel data was not properly extracted from the executing assembly");

                PFW.CSIST203.Project3.Persisters.Excel.ExcelPersister obj = null;
                AssertDelegateSuccess(() => obj = new Project3.Persisters.Excel.ExcelPersister(tmpExcelFile),
                    "The excel document should have been read into memory without issue");
                Assert.AreEqual(9, obj.Data.Rows.Count, "The expected number of rows were not read from the test excel file");

                var row5 = obj.GetRow(3);
                Assert.IsNotNull(row5, "row 4 should be present in the sample excel file");
                Assert.AreEqual("Anne", row5["First Name"], "Row 4 did not have the expected first name");
                Assert.AreEqual("Hellung-Larsen", row5["Last Name"], "Row 4 did not have the expected last name");
                Assert.AreEqual("anne@northwindtraders.com", row5["E-mail Address"], "Row 4 did not have the expected email address");
                Assert.AreEqual("(123)555-0104", row5["Business Phone"], "Row 4 did not have the expected business phone");
                Assert.AreEqual("Northwind Traders", row5["Company"], "Row 4 did not have the expected company name");
                Assert.AreEqual("Sales Representative", row5["Job Title"], "Row 4 did not have the expected sales representative");
            }

            [TestMethod]
            public void ThrowsObjectDisposedExceptionAfterBeingDisposed()
            {
                PFW.CSIST203.Project3.Persisters.Excel.ExcelPersister obj = null;
                AssertDelegateSuccess(() => obj = new Project3.Persisters.Excel.ExcelPersister(),
                    "Instantiation of the parameterless constructor should not throw an exception");
                AssertDelegateSuccess(() => obj.Dispose(), "Disposing of the object should not throw an exception");
                AssertDelegateFailure(() => obj.GetRow(0), typeof(ObjectDisposedException),
                    "The object should have thrown an exception when this method was called because it was previously disposed");
            }
        }

        [TestClass]
        public class CountRowsMethod : ExcelDataPersisterTests
        {
            [TestMethod]
            public void ReturnsZeroWithParameterlessConstructor()
            {
                PFW.CSIST203.Project3.Persisters.Excel.ExcelPersister obj = null;
                AssertDelegateSuccess(() => obj = new Project3.Persisters.Excel.ExcelPersister(),
                    "Instantiation of the parameterless constructor should not throw an exception");
                Assert.AreEqual(0, obj.CountRows(), "The number of rows returned should be zero");
                AssertDelegateSuccess(() => obj.Dispose(), "Object disposal should not throw an exception");
            }

            [TestMethod]
            public void ReturnsCorrectCountFromExcelFile()
            {
                var directory = GetMethodSpecificWorkingDirectory();
                var tmpExcelFile = System.IO.Path.Combine(directory, "data004.xlsx");
                CopyEmbeddedResourceBaseToDirectory("PFW.CSIST203.Project3.Tests.Resources.Persisters.Excel.ReturnsCorrectCountFromExcelFile", directory);
                Assert.IsTrue(System.IO.File.Exists(tmpExcelFile), "The sample excel data was not properly extracted from the executing assembly");

                PFW.CSIST203.Project3.Persisters.Excel.ExcelPersister obj = null;
                AssertDelegateSuccess(() => obj = new Project3.Persisters.Excel.ExcelPersister(tmpExcelFile),
                    "reading the test data should not cause an exception");

                Assert.AreEqual(9, obj.CountRows(), "The expected number of rows from the excel file was not found");
                AssertDelegateSuccess(() => obj.Dispose(), "Object disposal should not throw an exception");
            }

            [TestMethod]
            public void ThrowsObjectDisposedExceptionAfterBeingDisposed()
            {
                PFW.CSIST203.Project3.Persisters.Excel.ExcelPersister obj = null;
                AssertDelegateSuccess(() => obj = new Project3.Persisters.Excel.ExcelPersister(),
                    "Instantiation of the parameterless constructor should not throw an exception");
                AssertDelegateSuccess(() => obj.Dispose(), "Disposing of the object should not throw an exception");

                AssertDelegateFailure(() => obj.CountRows(), typeof(ObjectDisposedException), "The object should have thrown an exception when this method was called because it was previously disposed");
            }
        }

        [TestClass]
        public class DisposeMethod : ExcelDataPersisterTests
        {
            [TestMethod]
            public void DisposeNullsOutDataTable()
            {
                PFW.CSIST203.Project3.Persisters.Excel.ExcelPersister obj = null;
                AssertDelegateSuccess(() => obj = new Project3.Persisters.Excel.ExcelPersister(),
                    "Instantiation of the parameterless constructor should not throw an exception");
                Assert.IsNotNull(obj.Data, "The datatable should be non-null immediately upon instantiation");

                AssertDelegateSuccess(() => obj.Dispose(), "Disposing of the object should not throw an exception");
                Assert.IsNull(obj.Data, "The datatable should have been nulled out when the Dispose() method was called");
            }

            [TestMethod]
            public void DisposeSetsIsDisposedToFalse()
            {
                PFW.CSIST203.Project3.Persisters.Excel.ExcelPersister obj = null;
                AssertDelegateSuccess(() => obj = new Project3.Persisters.Excel.ExcelPersister(),
                    "Instantiation of the parameterless constructor should not throw an exception");
                Assert.IsNotNull(obj.Data, "The datatable should be non-null immediately upon instantiation");

                AssertDelegateSuccess(() => obj.Dispose(), "Disposing of the object should not throw an exception");
                Assert.IsTrue(obj.isDisposed, "The IsDisposed property should be set to True after a called to Dispose()");
            }

            [TestMethod]
            public void MultipleDisposeCallsDoNotThrowAnException()
            {
                PFW.CSIST203.Project3.Persisters.Excel.ExcelPersister obj = null;
                AssertDelegateSuccess(() => obj = new Project3.Persisters.Excel.ExcelPersister(),
                    "Instantiation of the parameterless constructor should not throw an exception");
                Assert.IsNotNull(obj.Data, "The datatable should be non-null immediately upon instantiation");

                AssertDelegateSuccess(() => obj.Dispose(), "Disposing of the object should not throw an exception");
                AssertDelegateSuccess(() => obj.Dispose(), "Additional calls to the Dispose() method should complete without error, but do nothing");
                AssertDelegateSuccess(() => obj.Dispose(), "Additional calls to the Dispose() method should complete without error, but do nothing");
            }
        }
    }

}
