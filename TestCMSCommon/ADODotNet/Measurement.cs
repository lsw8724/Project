using System.Linq;
using System.Data;
using System.Data.SqlClient;
using TestCMSCommon.Properties;

namespace TestCMSCommon.ADODotNET
{
    public class Measurement
    {
        public int Idx;
        public byte MeasureType;
        public int ChannelId;
        public int LowFreq;
        public int HighFreq;
        public int Interval1;
        public int Interval2;
        public int Interval3;

        public Measurement(int id = -1, byte type = 0, int chid = 0, int l = 0, int h = 0, int i1 = 0, int i2 = 0, int i3=0)
        {
            Idx=id;
            MeasureType = type;
            ChannelId = chid;
            LowFreq= l;
            HighFreq = h;
            Interval1= i1;
            Interval2= i2;
            Interval3= i3;
        }
    }

    public class MeasurementTable
    {
        private SqlConnection DBConnection;

        public MeasurementTable(SqlConnection conn)
        {
            DBConnection = conn;
        }

        public Measurement[] GetAllMeasure()
        {
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Measurement]", DBConnection))
                return GetResult(cmd);
        }

        public Measurement[] GetData(int idx)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Measurement] WHERE [Idx] = @ID ", DBConnection))
            {
                cmd.Parameters.AddWithValue("@ID", idx);
                return GetResult(cmd);
            }         
        }

        public Measurement[] GetDataFromChid (int chid)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Measurement] WHERE [ChannelId] = @CHID", DBConnection))
            {
                cmd.Parameters.AddWithValue("@CHID", chid);
                return GetResult(cmd);
            }
            
        }

        public void InsertData(Measurement measure)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Measurement] VALUES(@ID, @TYPE, @CHID, @LOW, @HIGH, @INTERVAL1, @INTERVAL2, @INTERVAL3)", DBConnection))
            {
                cmd.Parameters.AddWithValue("@ID", measure.Idx);
                cmd.Parameters.AddWithValue("@TYPE", measure.MeasureType);
                cmd.Parameters.AddWithValue("@CHID", measure.ChannelId);
                cmd.Parameters.AddWithValue("@LOW", measure.LowFreq);
                cmd.Parameters.AddWithValue("@HIGH", measure.HighFreq);
                cmd.Parameters.AddWithValue("@INTERVAL1", measure.Interval1);
                cmd.Parameters.AddWithValue("@INTERVAL2", measure.Interval2);
                cmd.Parameters.AddWithValue("@INTERVAL3", measure.Interval3);

                if (DBConnection.State != ConnectionState.Open)
                    DBConnection.Open();

                DBConnection.ChangeDatabase(Settings.Default.DBName);
                cmd.ExecuteNonQuery();

                if (DBConnection.State != ConnectionState.Closed)
                    DBConnection.Close();
            }
        }

        public void DeleteData(int idx)
        {
            using (SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Measurement] WHERE Idx = @ID", DBConnection))
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

        private Measurement[] GetResult(SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            if (DBConnection.State != ConnectionState.Open)
                DBConnection.Open();

            DBConnection.ChangeDatabase(Settings.Default.DBName);

            using (var da = new SqlDataAdapter(cmd.CommandText, DBConnection))
                da.Fill(dt);

            if (DBConnection.State != ConnectionState.Closed)
                DBConnection.Close();

            return dt.Select().Select(row => new Measurement((int)row.ItemArray[0], (byte)row.ItemArray[1], (int)row.ItemArray[2], (int)row.ItemArray[3], (int)row.ItemArray[4], (int)row.ItemArray[5], (int)row.ItemArray[6], (int)row.ItemArray[7])).ToArray();
        }
    }
}
