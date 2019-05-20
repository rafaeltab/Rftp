using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace RftpLib
{
    public class SqlDbLib
    {
        #region conn vars
        private MySqlConnection Conn;
        private readonly string Username = "RFTP";
        private readonly string Password = "3IQcE6mytunZwtvt";
        private readonly string Server = "rafaeltab.tk";
        private readonly string DefaultDB = "Rftp_Database";
        #endregion conn vars

        public SqlDbLib()
        {
            try
            {
                Conn = new MySqlConnection($"SERVER={Server};DATABASE={DefaultDB};UID={Username};PASSWORD={Password}");
                
            }catch(Exception e)
            {
                ConsoleManager.Show();
                Console.WriteLine(e.Message);
            }            
        }

        public MySqlDataReader ExecuteReturningQuery(string query, Dictionary<string, object> parameters)
        {
            if (Conn.State != System.Data.ConnectionState.Open)
            {
                Conn.Open();
            }
            MySqlCommand command = new MySqlCommand(query, Conn);

            foreach (var p in parameters)
            {
                command.Parameters.AddWithValue(p.Key, p.Value);
            }

            return command.ExecuteReader();           
        }

        public void ExecuteNonReturningQuery(string query,Dictionary<string,object> parameters)
        {
            if (Conn.State != System.Data.ConnectionState.Open)
            {
                Conn.Open();
            }

            MySqlCommand command = new MySqlCommand(query, Conn);
            foreach(var p in parameters)
            {
                command.Parameters.AddWithValue(p.Key, p.Value);
            }

            command.ExecuteNonQuery();           
        }    

        public object ExecuteScalar(string query, Dictionary<string, object> parameters)
        {
            if (Conn.State != System.Data.ConnectionState.Open)
            {
                Conn.Open();
            }

            MySqlCommand command = new MySqlCommand(query, Conn);
            foreach (var p in parameters)
            {
                command.Parameters.AddWithValue(p.Key, p.Value);
            }
            return command.ExecuteScalar();
        }
    }
}
