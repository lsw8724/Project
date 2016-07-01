using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using TestCMSCommon.Properties;
using System.Linq;

namespace TestCMSCommon.ADODotNET
{
    public class TrendData
    {
        public int Idx;
        public DateTime TimeStamp;
        public int MeasureId;
        public float Scalar;

        public TrendData(int idx, DateTime timeStamp, int measureId, float scalar)
        {
            Idx = idx;
            TimeStamp = timeStamp;
            MeasureId =measureId;
            Scalar = scalar;
        }

        public TrendData(DateTime timeStamp, int measureId, float scalar)
        {
            TimeStamp = timeStamp;
            MeasureId =measureId;
            Scalar = scalar;
        }
    }

    public class TrendDataTable
    {
        private SqlConnection DBConnection;
      
        public TrendDataTable(SqlConnection conn)
        {
            DBConnection = conn;
        }

        public TrendData[] GetData(int measureId)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[TrendData] WHERE [MeasureId] = @ID", DBConnection))
            {
                cmd.Parameters.AddWithValue("@ID", measureId);
                return GetResult(cmd);
            }           
        }

        public TrendData[] GetDataRange(DateTime start, DateTime end)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[TrendData] WHERE [TimeStamp] >= @START AND [TimeStamp] < @END", DBConnection))
            {
                cmd.Parameters.AddWithValue("@START", new SqlDateTime(start));
                cmd.Parameters.AddWithValue("@END", new SqlDateTime(end));
                return GetResult(cmd);
            }
        }

        public void InsertData(TrendData row)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[TrendData] (TimeStamp,MeasureId,Scalar) VALUES(@TIMESTAMP, @MEASUREID, @SCALAR)", DBConnection))
            {
                cmd.Parameters.AddWithValue("@TIMESTAMP", new SqlDateTime(row.TimeStamp));
                cmd.Parameters.AddWithValue("@MEASUREID", row.MeasureId);
                cmd.Parameters.AddWithValue("@SCALAR", row.Scalar);
                if (DBConnection.State != ConnectionState.Open)
                    DBConnection.Open();

                DBConnection.ChangeDatabase(Settings.Default.DBName);
                cmd.ExecuteNonQuery();

                if (DBConnection.State != ConnectionState.Closed)
                    DBConnection.Close();
            };
        }

        public void DeleteData(int idx)
        {
            using (SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[TrendData] WHERE Idx = @ID", DBConnection))
            {
                cmd.Parameters.AddWithValue("@ID", idx);
                if (DBConnection.State != ConnectionState.Open)
                    DBConnection.Open();

                DBConnection.ChangeDatabase(Settings.Default.DBName);
                cmd.ExecuteNonQuery();

                if (DBConnection.State != ConnectionState.Closed)
                    DBConnection.Close();
            } 
        }

        private TrendData[] GetResult(SqlCommand cmd)
        {
            var dt = new DataTable();
            if (DBConnection.State != ConnectionState.Open)
                DBConnection.Open();

            DBConnection.ChangeDatabase(Settings.Default.DBName);

            using (var da = new SqlDataAdapter(cmd))
                da.Fill(dt);

            if (DBConnection.State != ConnectionState.Closed)
                DBConnection.Close();

            return dt.Select().Select(row => new TrendData((int)row.ItemArray[0], (DateTime)row.ItemArray[1], (int)row.ItemArray[2], Convert.ToSingle(row.ItemArray[3]))).ToArray();
        }
    }
}
