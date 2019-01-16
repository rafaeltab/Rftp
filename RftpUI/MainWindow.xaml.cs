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
using System.Runtime.InteropServices;
using RftpUI.pages;

namespace RftpUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //used when changing to FTPPage
        public User u;


        public MainWindow()
        {
            InitializeComponent();
            windowTitle.Content = Title;
            EventsManager.AddChangeMainPageEventListener(ChangePageEventListener);

        }

        #region drag,close,min,max

        #region vars and externals

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        #endregion vars

        private void Maximize(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Maximized) {
                WS_ToMaximized();
            }
            else
            {
                WS_ToNormal();
            }
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Minimize(object sender, RoutedEventArgs e)
        {
            WS_ToMinimized();
        }

        #region dragging

        private void ChangePageEventListener(object e, MainPageArgs args)
        {
            if (args.PageLocation == "pages/FTPPage.xaml")
            {
                u = (User)args.extra;
            }

            contentLoaderFrame.Navigate(new Uri(args.PageLocation, UriKind.RelativeOrAbsolute));      
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            if(WindowState == WindowState.Maximized)
            {
                WS_ToNormal();
                Left = GetMousePosition().X - Width / 2;
                Top = GetMousePosition().Y - 20;
            }
            if(WindowState == WindowState.Normal && e.ClickCount == 2)
            {
                WS_ToMaximized();
            }

            DragMove();                
        }       

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        #endregion dragging

        private void WS_ToNormal()
        {
            WindowState = WindowState.Normal;
        }
        private void WS_ToMaximized()
        {
            WindowState = WindowState.Maximized;
        }
        private void WS_ToMinimized()
        {
            WindowState = WindowState.Minimized;
        }       
        
        #endregion drag,close,min,max

    }
}