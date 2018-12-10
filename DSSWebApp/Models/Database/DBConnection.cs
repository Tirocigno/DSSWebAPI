using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace DSSWebApp.Models.Database
{
    public class DBConnection
    {
        public string connString, factory;

        public DBConnection()
        {
            string sdb = ConfigurationManager.AppSettings["dbServer"];
            switch (sdb)
            {
                case "SQLiteConn":

                    connString = ConfigurationManager.ConnectionStrings["SQLiteConn"].ConnectionString;
                    factory = ConfigurationManager.ConnectionStrings["SQLiteConn"].ProviderName;
                    string dbPath =  (string)AppDomain.CurrentDomain.GetData("DataDirectory") + "\\testDb.sqlite";
                    connString = connString.Replace("DBFILE", dbPath);
                    break;
                case "LocalSqlServConn":
                    connString =
                    ConfigurationManager.ConnectionStrings["LocalDbConn"].ConnectionString;
                    factory = "System.Data.SqlClient";
                    break;

                case "RemoteSqlServConn":
                    connString = ConfigurationManager.ConnectionStrings["RemoteSqlServConn"].ConnectionString;
                    factory = "System.Data.SqlClient";
                    break;

                default:
                    connString =
                    ConfigurationManager.ConnectionStrings["LocalDbConn"].ConnectionString;
                    factory = "System.Data.SqlClient";
                    break;
            }
        }

        public string launchSimpleQuery(string query, string tableName, string field)
        {
            string result = "";
            DataSet ds = new DataSet();
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(factory);
            using (DbConnection conn = dbFactory.CreateConnection())
            {
                try
                {
                    conn.ConnectionString = connString;
                    conn.Open();
                    DbDataAdapter dbAdapter = dbFactory.CreateDataAdapter();
                    DbCommand dbCommand = conn.CreateCommand();
                    dbCommand.CommandText = query;
                    DbParameter param = dbCommand.CreateParameter();
                    dbAdapter.SelectCommand = dbCommand;
                    dbAdapter.Fill(ds);
                    ds.Tables[0].TableName = tableName;
                    foreach (DataRow dr in ds.Tables[tableName].Rows)
                        result += dr[field].ToString();
                }
                catch (Exception ex)
                {
                    result = "Exception thrown: " + ex.ToString();
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                   
                }
                return result;
            }
        }

        public List<clienti> readClientiFromDB()
        {
            customRemoteDbConnectionString connection = new customRemoteDbConnectionString();
            return connection.clienti.ToList<clienti>();
        }

        public List<ordini> readOrdiniFromDB()
        {
            customRemoteDbConnectionString connection = new customRemoteDbConnectionString();
            return connection.ordini.ToList<ordini>();
        }

        public List<serie> readSerieFromDB()
        {
            customRemoteDbConnectionString connection = new customRemoteDbConnectionString();
            return connection.serie.ToList<serie>();
        }




        #region funzioni accesso generiche

        /**
         * Read a table and convert it to JSon
         */
        public string readTableViaFactory(string queryText)
        {
            int i, numcol;
            string res = "[";
            List<string> columns = new List<string>();
            DbProviderFactory dbFactory = null;

            Console.WriteLine("readTable raggiunto");

            dbFactory = DbProviderFactories.GetFactory(this.factory);

            using (DbConnection conn = dbFactory.CreateConnection())
            {
                try
                {
                    conn.ConnectionString = this.connString;
                    conn.Open();
                    IDbCommand com = conn.CreateCommand();
                    com.CommandText = queryText;
                    IDataReader reader = com.ExecuteReader();

                    numcol = reader.FieldCount;
                    for (i = 0; i < numcol; i++)
                        columns.Add(reader.GetName(i));

                    while (reader.Read())
                    {
                        res += "{";
                        for (i = 0; i < numcol; i++)
                        {
                            res += "\"" + columns[i] + "\":\"" + reader[i] + "\",";
                        }
                        res += "},";
                        res = res.Replace(",}", "}");
                    }
                    reader.Close();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    res = "[ERROR] " + ex.Message;
                    goto end;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
            res = (res + "]").Replace(",]", "]");
            end:
            return res;
        }

        public int execNonQueryViaFactory(string connString, string queryText, string factory)
        {
            int numRows = 0;
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(factory);

            using (DbConnection connection = dbFactory.CreateConnection())
            {
                try
                {
                    connection.ConnectionString = connString;
                    connection.Open();
                    IDbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = queryText;

                    numRows = cmd.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
                return numRows;
            }
        }

        public object execScalarViaFactory(string connString, string queryText, string factory)
        {
            object retObj = null;
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(factory);

            using (DbConnection conn = dbFactory.CreateConnection())
            {
                try
                {
                    conn.ConnectionString = connString;
                    conn.Open();
                    IDbCommand com = conn.CreateCommand();
                    com.CommandText = queryText;

                    retObj = com.ExecuteScalar();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
                return retObj;
            }
        }

        #endregion
    }
}