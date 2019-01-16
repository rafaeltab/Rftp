using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RftpLib;

namespace RftpUI.pages
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Page
    {
        public Register()
        {
            InitializeComponent();
        }

        private void Click_CreateAccount(object sender, RoutedEventArgs e)
        {
            string user = NameField.Text;
            string pass = PassField.Password;
            string confirmPass = PassConfirmField.Password;

            if (pass != confirmPass)
            {
                Error_Box.Content = "Password and confirm password are not equal";
                Error_Box.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                Error_Box.Visibility = Visibility.Hidden;                
            }

            SqlDbLib dbLib = new SqlDbLib();

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("@name", user);

            long result;

            result =(long) dbLib.ExecuteScalar("SELECT COUNT(*) FROM `Users` WHERE Username = @name", param);

            if (result > 0)
            {
                Error_Box.Visibility = Visibility.Visible;
                Error_Box.Content = "Username already Taken";
                return;
            }else
            {
                Error_Box.Visibility = Visibility.Hidden;
            }

            param = new Dictionary<string, object>();
            param.Add("@name", user);
            param.Add("@pass", Encrypting.Encrypt(pass));

            
            dbLib.ExecuteNonReturningQuery("INSERT INTO Users (Username,Password,AuthLVL) VALUES (@name,@pass,0);", param);

            EventsManager.InvokeChangeMainPageEvent(this, "pages/LoginPage.xaml", null);
        }
    }
}
