using System.Data.Common;
using legoog.Services;
using legoog.Models;
using Microsoft.Data.SqlClient;

namespace legoog.Services.DB;

public class DBService 
{
    private Data? data;

    // later safe it in a ressource file
    string connectionString = @"";

    private SqlConnection sqlConn()
    {
        try
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

    }


    public void insertNewSearchResult()
    {
        // write DB Connection and insert all data    
        var dbCon = sqlConn();
        dbCon.CreateCommand();
        dbCon.Close();
    }


    // public Data getSearch()
    // {
    //     return null; 
    // }

}