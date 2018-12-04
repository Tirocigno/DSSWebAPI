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
                    string dbPath = @"C:\Users\feden\Documents\Visual Studio 2015\Projects\DSS2018WFA\testDb.sqlite";
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
    }
}