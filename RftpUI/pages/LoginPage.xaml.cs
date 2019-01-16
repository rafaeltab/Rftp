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

namespace RftpUI
{
    /// <summary>
    /// Interaction logic for TestPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            ShowsNavigationUI = false;
        }

        private void Click_Login(object sender, RoutedEventArgs e)
        {
            User u = new User(NameField.Text, PassField.Password);

            if(u.CorrectUser)
            {
                EventsManager.InvokeChangeMainPageEvent(this, "pages/FTPPage.xaml", u);
            }
            else
            {
                NameField.Text = "login denied";
            }
                        
        }

        private void Link_Register(object sender, RoutedEventArgs e)
        {
            EventsManager.InvokeChangeMainPageEvent(this, "pages/Register.xaml", null);
        }
    }
}