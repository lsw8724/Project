using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TestCMSCommon.Properties;

namespace TestCMSCommon.DataBase
{
    public class ReceiverRow
    {
        public int Idx;
        public byte ReceiverType;
        public string Ip;
        public string FilePath;
        public int Port;
        public byte SerializerType;

        public ReceiverRow(object id, object type, object ip, object path, object port, object serializer)
        {
            Idx=(int)id;
            ReceiverType = (byte)type;
            Ip = ip.ToString();
            FilePath = path.ToString();
            Port = (int)port;
            SerializerType = (byte)serializer;
        }
    }

    public class Receiver
    {
        private SqlConnection DBConnection;
        private const string BaseSelectQuery = "SELECT * FROM [dbo].[Receiver] ";

        public Receiver(SqlConnection conn)
        {
            DBConnection = conn;
            if (!SQLRepository.CheckExistTable("Receiver"))
                SQLRepository.CreateReceiverTable();
        }

        public Dictionary<int, ReceiverRow> GetAllReceiver()
        {
            using (SqlCommand cmd = new SqlCommand(BaseSelectQuery, DBConnection))
                return GetResult(cmd);
        }

        public void InsertData(int id, int type, string ip, string path, int port, int serializer)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Receiver] VALUES(@ID, @TYPE, @IP , @PATH, @PORT, @SERIALIZER)", DBConnection))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@TYPE", type);
                cmd.Parameters.AddWithValue("@IP", ip);
                cmd.Parameters.AddWithValue("@PATH", path);
                cmd.Parameters.AddWithValue("@PORT", port);
                cmd.Parameters.AddWithValue("@SERIALIZER", serializer);

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

        private Dictionary<int, ReceiverRow> GetResult(SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            if (DBConnection.State != ConnectionState.Open) DBConnection.Open();
            DBConnection.ChangeDatabase(Settings.Default.DBName);
            using (var da = new SqlDataAdapter(cmd.CommandText,DBConnection)) da.Fill(dt);
            if (DBConnection.State != ConnectionState.Closed) DBConnection.Close();
            Dictionary<int, ReceiverRow> result = new Dictionary<int, ReceiverRow>();
            foreach (DataRow row in dt.Rows)
                result.Add((int)row.ItemArray[0],new ReceiverRow(row.ItemArray[0], row.ItemArray[1], row.ItemArray[2], row.ItemArray[3], row.ItemArray[4], row.ItemArray[5]));
            return result;
        }
    }
}
