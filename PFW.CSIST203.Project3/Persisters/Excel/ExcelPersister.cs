using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

namespace PFW.CSIST203.Project3.Persisters.Excel
{
    /// <summary>
    /// Excel Persister that interacts with data in an xls or xlsx file
    /// </summary>
    public class ExcelPersister : IPersistData
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ExcelPersister));

        private System.Data.DataTable _Data = null;
        private string _ExcelFile = null;

        /// <summary>
        /// This data table must be populated with all data contained in the specified excel file
        /// </summary>
        internal System.Data.DataTable Data
        {
            get
            {
                return _Data;
            }
            private set
            {
                _Data = value;
            }
        }

        private bool _isDisposed = false;

        /// <summary>
        /// Get a value indicating whether or not the object has been disposed
        /// </summary>
        internal bool isDisposed
        {
            get
            {
                return _isDisposed;
            }
            private set
            {
                _isDisposed = value;
            }
        }

        public string FileFilter
        {
            get
            {
                return "Excel Files|*.xls;*.xlsx";
            }
        }

        /// <summary>
        /// This contructor creates a persister that contains no data
        /// </summary>
        public ExcelPersister()
        {
            Data = new DataTable("Sheet1");
            Data.Columns.AddRange(new DataColumn[] {
                new DataColumn("First Name", typeof(string)),
                new DataColumn("Last Name", typeof(string)),
                new DataColumn("E-mail Address", typeof(string)),
                new DataColumn("Business Phone", typeof(string)),
                new DataColumn("Company", typeof(string)),
                new DataColumn("Job Title", typeof(string))
            });
        }

        /// <summary>
        /// Persists data to and from the specified excel file
        /// </summary>
        /// <param name="excelFilepath">The excel file that should be read into memory</param>
        public ExcelPersister(string excelFilepath)
        {
            if (!System.IO.File.Exists(excelFilepath))
                throw new System.IO.FileNotFoundException(excelFilepath);

            // keep a pointer to the excel file
            _ExcelFile = excelFilepath;

            var table = new System.Data.DataTable("Sheet1");
            try
            {
                using (System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(Util.GetExcelConnectionString(excelFilepath, true)))
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

                logger.Error("Problem reading excel file: " + excelFilepath, ex);
                throw;
            }

            // assign the data table before exiting the constructor
            this.Data = table;
        }

        /// <summary>
        /// This method must retrieve data from the data table at the specified row
        /// </summary>
        /// <param name="rowNumber">The row you would like to request from the excel data (zero based)</param>
        /// <returns>The row requested or nothing if the row number specified is non-positive or beyond the end of the available data</returns>
        public System.Data.DataRow GetRow(int rowNumber)
        {
            if (this.isDisposed)
                throw new ObjectDisposedException("Data (DataTable)");
            if (rowNumber < 0 || rowNumber >= this.Data.Rows.Count)
                return null;
            return this.Data.Rows[rowNumber];
        }

        /// <summary>
        /// Retrieves the number of rows contained in the excel data
        /// </summary>
        public int CountRows()
        {
            if (this.isDisposed)
                throw new ObjectDisposedException("Data (DataTable)");
            return this.Data.Rows.Count;
        }

        /// <summary>
        /// Cleans up any managed resources used by the DataTable
        /// </summary>
        public void Dispose()
        {
            isDisposed = true;
            if (null != Data)
            {
                Data.Dispose();
                Data = null;
            }
        }

        public DataTable GetData()
        {
            return Data;
        }

        public void StoreRow(DataRow row)
        {
            throw new NotImplementedException();
        }

        public DataRow CreateRow(string tableName)
        {
            throw new NotImplementedException();
        }
    }


}
