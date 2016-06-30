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
    public class MeasurementRow
    {
        public int Idx;
        public int MeasureType;
        public int ChannelId;
        public int LowFreq;
        public int HighFreq;
        public int Interval1;
        public int Interval2;
        public int Interval3;

        public MeasurementRow(object id, object type, object chid, object l, object h, object i1, object i2, object i3)
        {
            Idx=(int)id;
            MeasureType = (int)type;
            ChannelId = (int)chid;
            LowFreq= (int)l;
            HighFreq = (int)h;
            Interval1= (int)i1;
            Interval2= (int)i2;
            Interval3= (int)i3;
        }
    }

    public class Measurement
    {
        private SqlConnection DBConnection;
        private const string BaseSelectQuery = "SELECT * FROM [dbo].[Measurement] ";

        public Measurement(SqlConnection conn)
        {
            DBConnection = conn;
            if (!SQLRepository.CheckExistTable("Measurement"))
                SQLRepository.CreateMeasurementTable();
        }

        public Dictionary<int, MeasurementRow> GetAllData()
        {
            using (SqlCommand cmd = new SqlCommand(BaseSelectQuery, DBConnection))
                return GetResult(cmd);
        }

        public Dictionary<int, MeasurementRow> GetData(int idx)
        {
            using (SqlCommand cmd = new SqlCommand(BaseSelectQuery + "WHERE [Idx] = @ID ", DBConnection))
            {
                cmd.Parameters.AddWithValue("@ID", idx);
                return GetResult(cmd);
            }         
        }

        public Dictionary<int, MeasurementRow> GetDataFromChid (int chid)
        {
            using (SqlCommand cmd = new SqlCommand(BaseSelectQuery + "WHERE [ChannelId] = @CHID", DBConnection))
            {
                cmd.Parameters.AddWithValue("@CHID", chid);
                return GetResult(cmd);
            }
            
        }

        public void InsertData(int id, int type, int chid, int l=0, int h=0, int i1=0, int i2=0, int i3 =0)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Measurement] VALUES(@ID, @TYPE, @CHID, @LOW, @HIGH, @INTERVAL1, @INTERVAL2, @INTERVAL3)", DBConnection))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@TYPE", type);
                cmd.Parameters.AddWithValue("@CHID", chid);
                cmd.Parameters.AddWithValue("@LOW", l);
                cmd.Parameters.AddWithValue("@HIGH", h);
                cmd.Parameters.AddWithValue("@INTERVAL1", i1);
                cmd.Parameters.AddWithValue("@INTERVAL2", i2);
                cmd.Parameters.AddWithValue("@INTERVAL3", i3);
                if (DBConnection.State != ConnectionState.Open) DBConnection.Open();
                DBConnection.ChangeDatabase(Settings.Default.DBName);
                cmd.ExecuteNonQuery();
                if (DBConnection.State != ConnectionState.Closed) DBConnection.Close();
            }
        }

        public void DeleteData(int idx)
        {
            using (SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Measurement] WHERE Idx = @ID", DBConnection))
            {
                cmd.Parameters.AddWithValue("@ID", idx);
                if (DBConnection.State != ConnectionState.Open) DBConnection.Open();
                DBConnection.ChangeDatabase(Settings.Default.DBName);
                cmd.ExecuteNonQuery();
                if (DBConnection.State != ConnectionState.Closed) DBConnection.Close();
            }
        }

        private Dictionary<int, MeasurementRow> GetResult(SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            if (DBConnection.State != ConnectionState.Open) DBConnection.Open();
            DBConnection.ChangeDatabase(Settings.Default.DBName);
            using (var da = new SqlDataAdapter(cmd.CommandText,DBConnection)) da.Fill(dt);
            if (DBConnection.State != ConnectionState.Closed) DBConnection.Close();
            Dictionary<int,MeasurementRow> result = new Dictionary<int, MeasurementRow>();
            foreach (DataRow row in dt.Rows)
                result.Add((int)row.ItemArray[0],new MeasurementRow(row.ItemArray[0], row.ItemArray[1], row.ItemArray[2], row.ItemArray[3], row.ItemArray[4], row.ItemArray[5], row.ItemArray[6], row.ItemArray[7]));
            return result;
        }
    }
}
