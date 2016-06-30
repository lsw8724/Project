using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TestCms1.Properties;
using System.Windows.Forms;
using System.Data;

namespace TestCms1.DataBase
{
    public enum SQLMessageType : int
    {
        DB_Exist,
        Table_Exist,
    }

    public static class SQLRepository
    {
        private static string SqlInfoMessage = string.Empty;
        private static SqlConnection DBConnection;
        public static TrendData TrendData { get { return new TrendData(DBConnection); } }
        public static Measurement Measurement {get{ return new Measurement(DBConnection); }}

        public static void Init()
        {
            try
            {
                DBConnection = new SqlConnection(
                  "server=" + Settings.Default.DBServerIp + ";" +
                  "uid=" + Settings.Default.DBAccount + ";" +
                  "pwd=" + Settings.Default.DBPassword + ";" +
                  "database=master;");
                DBConnection.InfoMessage += OnSQLInfoMessage;
                if (!CheckExistDatabase()) CreateDatabase();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show(sqlEx.Message);
            }
        }

        private static void OnSQLInfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            SqlInfoMessage = e.Message;
        }

        private static bool CheckExistDatabase()
        {
            using (SqlCommand cmd = new SqlCommand("IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = @DBNAME) PRINT @MESSAGE", DBConnection))
            {
                cmd.Parameters.AddWithValue("@DBNAME", Settings.Default.DBName);
                cmd.Parameters.AddWithValue("@MESSAGE", SQLMessageType.DB_Exist.ToString());
                if (DBConnection.State != ConnectionState.Open) DBConnection.Open();
                cmd.ExecuteNonQuery();
                if (DBConnection.State != ConnectionState.Closed) DBConnection.Close();
            }
            if (SqlInfoMessage.Equals(SQLMessageType.DB_Exist.ToString())) return true;
            else return false;            
        }

        private static void CreateDatabase()
        {
            using (SqlCommand cmd = new SqlCommand("CREATE DATABASE ["+Settings.Default.DBName+"]", DBConnection))
            {
                if (DBConnection.State != ConnectionState.Open) DBConnection.Open();
                cmd.ExecuteNonQuery();
                if (DBConnection.State != ConnectionState.Closed) DBConnection.Close();
            }
        }

        public static bool CheckExistTable(string tableName)
        {
            using (SqlCommand cmd = new SqlCommand("IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TABLE) PRINT @MESSAGE", DBConnection))
            {
                cmd.Parameters.AddWithValue("@TABLE", tableName);
                cmd.Parameters.AddWithValue("@MESSAGE", SQLMessageType.Table_Exist.ToString());
                if (DBConnection.State != ConnectionState.Open) DBConnection.Open();
                DBConnection.ChangeDatabase(Settings.Default.DBName);
                cmd.ExecuteNonQuery();
                if (DBConnection.State != ConnectionState.Closed) DBConnection.Close();
            }
            if (SqlInfoMessage.Equals(SQLMessageType.Table_Exist.ToString())) return true;
            else return false;           
        }

        public static void CreateMeasurementTable()
        {
            string query =  "BEGIN TRAN CREATE TABLE [dbo].[Measurement]( " +
                            "[Idx] [int] NOT NULL, " +
                            "[MeasureType] [int] NOT NULL, " +
                            "[ChannelId] [int] NOT NULL, " +
                            "[LowFreq] [int] NULL, " +
                            "[HighFreq] [int] NULL, " +
                            "[Interval1] [int] NULL, " +
                            "[Interval2] [int] NULL, " +
                            "[Interval3] [int] NULL, " +
                            "CONSTRAINT [PK_dbo.Measurement] PRIMARY KEY CLUSTERED([Idx] ASC)) ON[PRIMARY] COMMIT TRAN";
            using (SqlCommand cmd = new SqlCommand(query, DBConnection))
            {
                if (DBConnection.State != ConnectionState.Open) DBConnection.Open();
                DBConnection.ChangeDatabase(Settings.Default.DBName);
                cmd.ExecuteNonQuery();
                if (DBConnection.State != ConnectionState.Closed) DBConnection.Close();
            }
        }

        public static void CreateTrendDataTable()
        {
            string query =  "BEGIN TRAN CREATE TABLE [dbo].[TrendData]( " +
                            "[Idx] [int] IDENTITY(1,1) NOT NULL, " +
                            "[TimeStamp] [datetime] NOT NULL, " +
                            "[MeasureId] [int] NOT NULL, " +
                            "[Scalar] [float] NOT NULL, " +
                            "CONSTRAINT [PK_dbo.TrendData] PRIMARY KEY CLUSTERED([Idx] ASC)) ON[PRIMARY] COMMIT TRAN";
            using (SqlCommand cmd = new SqlCommand(query, DBConnection))
            {
                if (DBConnection.State != ConnectionState.Open) DBConnection.Open();
                DBConnection.ChangeDatabase(Settings.Default.DBName);
                cmd.ExecuteNonQuery();
                if (DBConnection.State != ConnectionState.Closed) DBConnection.Close();
            }
        }
    }
}
