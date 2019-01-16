using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RftpLib;

namespace RftpUI
{
    public class User
    {
        private readonly int _uid;
        private readonly string _username;
        private readonly int _authorityLevel;
        private readonly bool _correctUser;
        private readonly DateTime _signUpDate;

        public int AuthorityLevel { get { return _authorityLevel; } }
        public int Uid { get { return _uid; } }
        public string Username { get { return _username; } }
        public bool CorrectUser { get { return _correctUser; } }
        public DateTime SignUpDate { get { return _signUpDate; } }

        public User(string username, string password)
        {
            SqlDbLib db = new SqlDbLib();
            _correctUser = VerifyPass(username, password);
            if (_correctUser)
            {
                Dictionary<string, object> param = new Dictionary<string, object>
                {
                    { "@name", username }
                };

                var reader = db.ExecuteReturningQuery($"SELECT * FROM Users WHERE Username = @name", param);

                while (reader.Read())
                {
                    _uid = reader.GetInt32("UID");
                    _username = username;
                    _authorityLevel = reader.GetInt32("AuthLVL");
                    _signUpDate = reader.GetDateTime("SignUpDate");
                }
            }            
        }

        public static bool VerifyPass(string username, string password)
        {
            SqlDbLib db = new SqlDbLib();

            Dictionary<string, object> param = new Dictionary<string, object>
            {
                { "@name", username }
            };

            var reader = db.ExecuteReturningQuery("SELECT Password FROM Users WHERE Username = @name",param);
            string correctHash = "";
            while (reader.Read())
            {
                correctHash = reader.GetString(0);
            }
            

            return Encrypting.ValidatePassword(password, correctHash);
        }
    }
}
