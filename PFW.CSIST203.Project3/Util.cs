using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFW.CSIST203.Project3
{
    public class Util
    {

        /// <summary>
        /// Returns an excel file connection string suitable for use by an OleDbConnection
        /// </summary>
        /// <param name="excelFile">Path or filename of an excel document on disk</param>
        /// <returns>A connection string that is suitable for selecting all non-header content from the excel file</returns>
        public static string GetExcelConnectionString(string excelFile, bool hasHeaderRow)
        {

            // retrieve the extension and initialize connection string builder
            var extension = System.IO.Path.GetExtension(excelFile);
            System.Data.OleDb.OleDbConnectionStringBuilder builder = new System.Data.OleDb.OleDbConnectionStringBuilder();
            string header = hasHeaderRow ? "Yes" : "No";

            // if we are using Office 2000-era excel files, use the 4.0 provider
            if (string.Equals(extension, ".xls", StringComparison.OrdinalIgnoreCase) && !System.Environment.Is64BitOperatingSystem)
            {
                builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                builder.Add("Extended Properties", string.Format("Excel 8.0;IMEX=1;HDR={0};", header));
            }
            else if (string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                builder.Add("Extended Properties", string.Format("Excel 12.0;IMEX=1;HDR={0};", header));
            }
            else
                // The provider cannot be determined and an exception must be thrown
                throw new NotSupportedException(string.Format("Excel connection string for files with extension '{0}' are not supported by the operating system", extension));
            builder.DataSource = excelFile;
            return builder.ConnectionString;
        }

        /// <summary>
        /// Returns an access database connection string suitable for use by an OleDbConnection
        /// </summary>
        /// <param name="accessDatabaseFile">The physical access database for which a connection string is being requested</param>
        /// <returns>A connection string that is suitable for connecting to the provided access database</returns>
        public static string GetAccessConnectionString(string accessDatabaseFile)
        {
            System.Data.OleDb.OleDbConnectionStringBuilder builder = new System.Data.OleDb.OleDbConnectionStringBuilder();
            var extension = System.IO.Path.GetExtension(accessDatabaseFile);

            if (string.Equals(extension, ".mdb", StringComparison.OrdinalIgnoreCase) && !System.Environment.Is64BitOperatingSystem)
            {
                builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                builder.Add("User Id", "admin");
                builder.Add("Password", string.Empty);
            }
            else if (string.Equals(extension, ".accdb", StringComparison.OrdinalIgnoreCase))
            {
                builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                builder.Add("Persist Security Info", "False");
            }
            else
                // The provider cannot be determined and an exception must be thrown
                throw new NotSupportedException(string.Format("Access connection string for files with extension '{0}' are not supported by this utility method", extension));

            builder.DataSource = accessDatabaseFile;
            return builder.ConnectionString;
        }
    }


}
