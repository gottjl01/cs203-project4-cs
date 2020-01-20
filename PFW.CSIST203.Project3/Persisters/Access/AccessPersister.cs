using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

namespace PFW.CSIST203.Project3.Persisters.Access
{
    /// <summary>
    /// Access persister that interacts with an Access database
    /// </summary>
    public class AccessPersister : IPersistData
    {
        internal readonly string accessFile = null;
        internal bool noDatabase = false;
        internal bool IsDisposed = false;

        /// <summary>
        /// Creates a persister that acts like no data exists
        /// </summary>
        public AccessPersister()
        {
            // create an access persister that does nothing
            noDatabase = true;
        }

        /// <summary>
        /// Creates a persister that use the supplied access database file as its source
        /// </summary>
        /// <param name="accessFile">The access database file to read</param>
        public AccessPersister(string accessFile)
        {
            if (!System.IO.File.Exists(accessFile))
                throw new System.IO.FileNotFoundException("Access Database not found", accessFile);
            this.accessFile = accessFile;
        }

        /// <summary>
        /// The filter used by the open dialog to find files that this persister will handle
        /// </summary>
        /// <returns></returns>
        public string FileFilter
        {
            get
            {
                return "Access Database Files|*.mdb;*.accdb";
            }
        }

        /// <summary>
        /// Retrieves a row from the access database using the specific ID
        /// </summary>
        /// <param name="id">The ID of the row to retrieve</param>
        /// <returns>A DataRow representing the retrieved data, or null (Nothing) it is not found</returns>
        public DataRow GetRow(int id)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("Persister");

            if (noDatabase)
                return null;
            DataTable dt = new DataTable("tblEmployees");
            using (System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(Util.GetAccessConnectionString(accessFile)))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM [tblEmployees] where [ID] = @ID";

                    // create the single parameter object
                    var par = cmd.CreateParameter();
                    par.ParameterName = "@ID";
                    par.DbType = DbType.Int32;
                    par.Value = id;
                    cmd.Parameters.Add(par);

                    using (var dr = cmd.ExecuteReader())
                    {
                        dt.Load(dr);
                    }
                }
            }

            System.Data.DataRow row = null;

            // the single row that was returned if exactly one match was found
            if (dt.Rows.Count == 1)
                row = dt.Rows[0];

            return row;
        }

        /// <summary>
        /// Counts the number of rows present in the access database
        /// </summary>
        /// <returns>The number of rows present in the database</returns>
        public int CountRows()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("Persister");

            if (noDatabase)
                return 0;

            using (System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(Util.GetAccessConnectionString(accessFile)))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM [tblEmployees]";
                    return int.Parse(cmd.ExecuteScalar().ToString());
                }
            }
        }

        /// <summary>
        /// Disposes of the object and any managed resources
        /// </summary>
        public void Dispose()
        {
            IsDisposed = true;
        }

        /// <summary>
        /// Retrieves all data present in the access database as a single operation
        /// </summary>
        /// <returns>A datatable representing all data present in the access database</returns>
        public DataTable GetData()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("Persister");

            DataTable dt = new DataTable("tblEmployees");
            if (noDatabase)
                return dt;

            using (System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(Util.GetAccessConnectionString(accessFile)))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM [tblEmployees]";

                    using (var dr = cmd.ExecuteReader())
                    {
                        dt.Load(dr);
                    }
                }
            }
            return dt;
        }

        public void StoreRow(DataRow row)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public DataRow CreateRow(string tableName)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

    }

}
