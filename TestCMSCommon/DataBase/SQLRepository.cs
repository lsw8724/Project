using System.Data.SqlClient;
using TestCMSCommon.Properties;
using System.Data;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TestCMSCommon.DataBase
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
        public static TrendDataTable TrendData { get { return new TrendDataTable(DBConnection); } }
        public static MeasurementTable Measurement {get{ return new MeasurementTable(DBConnection); }}
        public static ReceiverTable Receiver { get { return new ReceiverTable(DBConnection); }}

        public static Dictionary<int, Measurement> MeasurementCache = new Dictionary<int, Measurement>();
        public static Dictionary<int, Receiver> ReceiverCache = new Dictionary<int, Receiver>();

        public static void Init()
        {
            try
            {
                DBConnection = new SqlConnection(
                  "server=" + Settings.Default.DBIp + ";" +
                  "uid=" + Settings.Default.DBAccount + ";" +
                  "pwd=" + Settings.Default.DBPassword + ";" +
                  "database=master;");
                DBConnection.InfoMessage += OnSQLInfoMessage;
                if (!CheckExistDatabase())
                {
                    CreateDatabase();
                    CreateMeasurementTable();
                    CreateReceiverTable();
                    CreateTrendDataTable();
                }
                else
                {
                    if (!CheckExistTable("Measurement")) throw new Exception("Measurement Table Not Exists!");
                    if (!CheckExistTable("Receiver")) throw new Exception("Receiver Table Not Exists!"); ;
                    if (!CheckExistTable("TrendData")) throw new Exception("TrendData Table Not Exists!"); ;
                }
                MeasurementCache = Measurement.GetAllMeasure().ToDictionary(x => x.Idx);
                ReceiverCache = Receiver.GetAllReceiver().ToDictionary(x => x.Idx);
            }
            catch (SqlException sqlEx)
            {
                //TODO Write Log
            }
        }

        public static void Close()
        {
            DBConnection.Close();
            DBConnection.Dispose();
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

                if (DBConnection.State != ConnectionState.Open)
                    DBConnection.Open();

                cmd.ExecuteNonQuery();

                if (DBConnection.State != ConnectionState.Closed)
                    DBConnection.Close();
            }
            if (SqlInfoMessage.Equals(SQLMessageType.DB_Exist.ToString())) return true;
            else return false;            
        }

        private static void CreateDatabase()
        {
            using (SqlCommand cmd = new SqlCommand("CREATE DATABASE ["+Settings.Default.DBName+"]", DBConnection))
            {
                if (DBConnection.State != ConnectionState.Open)
                    DBConnection.Open();

                cmd.ExecuteNonQuery();

                if (DBConnection.State != ConnectionState.Closed)
                    DBConnection.Close();
            }
        }

        public static bool CheckExistTable(string tableName)
        {
            using (SqlCommand cmd = new SqlCommand("IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TABLE) PRINT @MESSAGE", DBConnection))
            {
                cmd.Parameters.AddWithValue("@TABLE", tableName);
                cmd.Parameters.AddWithValue("@MESSAGE", SQLMessageType.Table_Exist.ToString());
                if (DBConnection.State != ConnectionState.Open)
                    DBConnection.Open();

                DBConnection.ChangeDatabase(Settings.Default.DBName);
                cmd.ExecuteNonQuery();

                if (DBConnection.State != ConnectionState.Closed)
                    DBConnection.Close();
            }
            if (SqlInfoMessage.Equals(SQLMessageType.Table_Exist.ToString())) return true;
            else return false;           
        }

        public static void CreateMeasurementTable()
        {
            string query =  "BEGIN TRAN CREATE TABLE [dbo].[Measurement]( " +
                            "[Idx] [int] NOT NULL, " +
                            "[MeasureType] [TINYINT] NOT NULL, " +
                            "[ChannelId] [int] NOT NULL, " +
                            "[LowFreq] [int] NULL, " +
                            "[HighFreq] [int] NULL, " +
                            "[Interval1] [int] NULL, " +
                            "[Interval2] [int] NULL, " +
                            "[Interval3] [int] NULL, " +
                            "CONSTRAINT [PK_dbo.Measurement] PRIMARY KEY CLUSTERED([Idx] ASC)) ON[PRIMARY] COMMIT TRAN";
            using (SqlCommand cmd = new SqlCommand(query, DBConnection))
            {
                if (DBConnection.State != ConnectionState.Open)
                    DBConnection.Open();

                DBConnection.ChangeDatabase(Settings.Default.DBName);
                cmd.ExecuteNonQuery();

                if (DBConnection.State != ConnectionState.Closed)
                    DBConnection.Close();
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
                if (DBConnection.State != ConnectionState.Open)
                    DBConnection.Open();

                DBConnection.ChangeDatabase(Settings.Default.DBName);
                cmd.ExecuteNonQuery();

                if (DBConnection.State != ConnectionState.Closed)
                    DBConnection.Close();
            }
        }

        public static void CreateReceiverTable()
        {
            string query = "BEGIN TRAN CREATE TABLE [dbo].[Receiver]( " +
                            "[Idx] [int] IDENTITY(1,1) NOT NULL, " +
                            "[ReceiverType] [TINYINT] NOT NULL, " +
                            "[Ip] [nvarchar](20) NULL, " +
                            "[FilePath] [nvarchar](200) NULL, " +
                            "[Port] [int] NOT NULL, " +
                            "[SerializerType] [TINYINT] NOT NULL, " +
                            "CONSTRAINT [PK_dbo.Receiver] PRIMARY KEY CLUSTERED([Idx] ASC)) ON[PRIMARY] COMMIT TRAN";
            using (SqlCommand cmd = new SqlCommand(query, DBConnection))
            {
                if (DBConnection.State != ConnectionState.Open)
                    DBConnection.Open();

                DBConnection.ChangeDatabase(Settings.Default.DBName);
                cmd.ExecuteNonQuery();

                if (DBConnection.State != ConnectionState.Closed)
                    DBConnection.Close();
            }
        }
    }
}
