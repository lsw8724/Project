using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using TestCms1.Properties;

namespace TestCms1.DataBase
{
    public class TrendDataRow
    {
        public int Idx;
        public DateTime TimeStamp;
        public int MeasureId;
        public float Scalar;

        public TrendDataRow(object idx, object timeStamp, object measureId, object scalar)
        {
            Idx = (int)idx;
            TimeStamp = (DateTime)timeStamp;
            MeasureId =(int)measureId;
            Scalar = Convert.ToSingle(scalar);
        }
    }

    public class TrendData
    {
        private SqlConnection DBConnection;
        private const string BaseSelectQuery = "SELECT * FROM [dbo].[TrendData] ";
      
        public TrendData(SqlConnection conn)
        {
            DBConnection = conn;
            if (!SQLRepository.CheckExistTable("TrendData"))
                SQLRepository.CreateTrendDataTable();
        }

        public Dictionary<int, TrendDataRow> GetData(int measureId)
        {
            using (SqlCommand cmd = new SqlCommand(BaseSelectQuery+ "WHERE [MeasureId] = @ID", DBConnection))
            {
                cmd.Parameters.AddWithValue("@ID", measureId);
                return GetResult(cmd);
            }           
        }

        public Dictionary<int, TrendDataRow> GetDataRange(DateTime start, DateTime end)
        {
            using (SqlCommand cmd = new SqlCommand(BaseSelectQuery + "WHERE [TimeStamp] >= @START AND [TimeStamp] < @END", DBConnection))
            {
                cmd.Parameters.AddWithValue("@START", new SqlDateTime(start));
                cmd.Parameters.AddWithValue("@END", new SqlDateTime(end));
                return GetResult(cmd);
            }
        }

        public void InsertData(DateTime timeStamp, int measureId=-1, float scalar = 0)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[TrendData] (TimeStamp,MeasureId,Scalar) VALUES(@TIMESTAMP, @MEASUREID, @SCALAR)", DBConnection))
            {
                cmd.Parameters.AddWithValue("@TIMESTAMP", new SqlDateTime(timeStamp));
                cmd.Parameters.AddWithValue("@MEASUREID", measureId);
                cmd.Parameters.AddWithValue("@SCALAR", scalar);
                if (DBConnection.State != ConnectionState.Open) DBConnection.Open();
                DBConnection.ChangeDatabase(Settings.Default.DBName);
                cmd.ExecuteNonQuery();
                if (DBConnection.State != ConnectionState.Closed) DBConnection.Close();
            };
        }

        public void DeleteData(int idx)
        {
            using (SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[TrendData] WHERE Idx = @ID", DBConnection))
            {
                cmd.Parameters.AddWithValue("@ID", idx);
                if (DBConnection.State != ConnectionState.Open) DBConnection.Open();
                DBConnection.ChangeDatabase(Settings.Default.DBName);
                cmd.ExecuteNonQuery();
                if (DBConnection.State != ConnectionState.Closed) DBConnection.Close();
            } 
        }

        private Dictionary<int, TrendDataRow> GetResult(SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            if (DBConnection.State != ConnectionState.Open) DBConnection.Open();
            DBConnection.ChangeDatabase(Settings.Default.DBName);
            using (var da = new SqlDataAdapter(cmd.CommandText,DBConnection)) da.Fill(dt);
            if (DBConnection.State != ConnectionState.Closed) DBConnection.Close();
            Dictionary<int, TrendDataRow> result = new Dictionary<int, TrendDataRow>();
            foreach (DataRow row in dt.Rows)
                result.Add((int)row.ItemArray[0], new TrendDataRow(row.ItemArray[0], row.ItemArray[1], row.ItemArray[2], row.ItemArray[3]));
            return result;
        }
    }
}
