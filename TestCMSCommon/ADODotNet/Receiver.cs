using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TestCMSCommon.Properties;

namespace TestCMSCommon.ADODotNET
{
    public class Receiver
    {
        public int Idx;
        public byte ReceiverType;
        public string Ip;
        public string FilePath;
        public int Port;
        public byte SerializerType;

        public Receiver(int id, byte type, string ip, string path, int port, byte serializer)
        {
            Idx=id;
            ReceiverType = type;
            Ip = ip;
            FilePath = path;
            Port = port;
            SerializerType = serializer;
        }
    }

    public class ReceiverTable
    {
        private SqlConnection DBConnection;

        public ReceiverTable(SqlConnection conn)
        {
            DBConnection = conn;
        }

        public Receiver[] GetAllReceiver()
        {
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Receiver]", DBConnection))
                return GetResult(cmd);
        }

        public void InsertData(Receiver row)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Receiver] VALUES(@ID, @TYPE, @IP , @PATH, @PORT, @SERIALIZER)", DBConnection))
            {
                cmd.Parameters.AddWithValue("@ID", row.Idx);
                cmd.Parameters.AddWithValue("@TYPE", row.ReceiverType);
                cmd.Parameters.AddWithValue("@IP", row.Ip);
                cmd.Parameters.AddWithValue("@PATH", row.FilePath);
                cmd.Parameters.AddWithValue("@PORT", row.Port);
                cmd.Parameters.AddWithValue("@SERIALIZER", row.SerializerType);

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

        private Receiver[] GetResult(SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            if (DBConnection.State != ConnectionState.Open)
                DBConnection.Open();

            DBConnection.ChangeDatabase(Settings.Default.DBName);

            using (var da = new SqlDataAdapter(cmd.CommandText, DBConnection))
                da.Fill(dt);

            if (DBConnection.State != ConnectionState.Closed)
                DBConnection.Close();

            return dt.Select().Select(row=>new Receiver((int)row.ItemArray[0], (byte)row.ItemArray[1], row.ItemArray[2].ToString(), row.ItemArray[3].ToString(), (int)row.ItemArray[4], (byte)row.ItemArray[5])).ToArray();
        }
    }
}
