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
using FluentFTP;
using System.Net;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Drawing;
using Etier.IconHelper;



namespace RftpUI.pages
{
    /// <summary>
    /// Interaction logic for FTPPage.xaml
    /// </summary>
    public partial class FTPPage : Page
    {
        public string sortby = "name";

        private FtpClient client;
        private FtpListItem[] currentDisplayItems;
        public User user;
        private string currentRoot = "/";

        public FTPPage()
        {
            InitializeComponent();
            var creds = JsonConfigReader.ReadJson<NetCred>(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName) + "/ftplogin.json");
            client = new FtpClient(creds.host)
            {
                Credentials = new NetworkCredential(creds.username, creds.password)
            };

            client.Connect();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mw = (MainWindow) Window.GetWindow(this);
            user = mw.u;
            if (user.CorrectUser)
            {
                if (user.AuthorityLevel >= 1)
                {
                    removeBtn.Visibility = Visibility.Visible;
                    uploadBtn.Visibility = Visibility.Visible;
                    reloadBtn.Visibility = Visibility.Visible;

                    FileDisplayPort.Visibility = Visibility.Visible;
                    topBar.Visibility = Visibility.Visible;
                    AccDisplay.Visibility = Visibility.Visible;
                    DisplayName.Visibility = Visibility.Visible;

                    DisplayName.Content = user.Username;
                    DisplayName.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                    double tw = DisplayName.DesiredSize.Width;

                    AccDisplayColumn.Width = new GridLength(tw + 20);
                    AccDisplay.Width = tw + 20;                    

                    currentDisplayItems = client.GetListing(currentRoot);

                    switch (sortby)
                    {
                        case "name":
                            currentDisplayItems = (from item in currentDisplayItems orderby item.FullName descending select item).ToArray();
                            break;
                        case "-name":
                            currentDisplayItems = (from item in currentDisplayItems orderby item.FullName ascending select item).ToArray();
                            break;
                        case "type":
                            currentDisplayItems = (from item in currentDisplayItems orderby (item.Type != FtpFileSystemObjectType.Directory ? IconReader.GetDescription(System.IO.Path.GetExtension(item.FullName)) : "Directory") descending select item).ToArray();
                            break;
                        case "-type":
                            currentDisplayItems = (from item in currentDisplayItems orderby (item.Type != FtpFileSystemObjectType.Directory ? IconReader.GetDescription(System.IO.Path.GetExtension(item.FullName)) : "Directory") ascending select item).ToArray();
                            break;
                        case "mod":
                            currentDisplayItems = (from item in currentDisplayItems orderby item.Modified descending select item).ToArray();
                            break;
                        case "-mod":
                            currentDisplayItems = (from item in currentDisplayItems orderby item.Modified ascending select item).ToArray();
                            break;
                        default:
                            break;
                    }

                    currentDisplayItems = (from item in currentDisplayItems orderby item.FullName select item).ToArray();
                    foreach (var item in currentDisplayItems)
                    {
                        FileDisplayPort.Items.Add(new Item {
                            Icon = item.Type != FtpFileSystemObjectType.Directory ? IconReader.GetFileIcon(System.IO.Path.GetExtension(item.FullName),IconReader.IconSize.Large,false) : IconReader.GetFolderIcon(IconReader.IconSize.Large,IconReader.FolderType.Open) ,
                            Name = item.FullName,
                            Type = item.Type != FtpFileSystemObjectType.Directory ? IconReader.GetDescription(System.IO.Path.GetExtension(item.FullName)) : "Directory",
                            Modified = item.Modified });
                    }
                }
                else
                {
                    LowPermsWarn.Visibility = Visibility.Visible;
                }
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((Item)((ListViewItem)e.Source).Content).Type == "Directory")
            {
                currentRoot = ((Item)((ListViewItem)e.Source).Content).Name;
                UpdateList();
            }
            else if (((Item)((ListViewItem)e.Source).Content).Type == "Parent")
            {
                currentRoot = currentRoot.GetFtpDirectoryName();
                UpdateList();
            }
            else
            {
                Download(((Item)((ListViewItem)e.Source).Content).Name);
            }            
        }

        private void Download(string path)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName = System.IO.Path.GetFileNameWithoutExtension(path),
                DefaultExt = System.IO.Path.GetExtension(path),
            };
            
            if (sfd.ShowDialog() == true)
            {
                foreach (var item in client.GetListing(path.GetFtpDirectoryName()))
                {
                    if (item.FullName == path)
                    {
                        if (item.Type == FtpFileSystemObjectType.Directory)
                        {
                            DownloadDirectory(path, sfd.FileName);
                            
                        }
                        else
                        {
                            client.DownloadFile(sfd.FileName, path);
                        }
                        break;
                    }                    
                }                
            }            
        }

        private void DownloadDirectory(string basePath, string location)
        {
            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }            

            var listings = client.GetListing(basePath);

            foreach (var item in listings)
            {
                if (item.Type == FtpFileSystemObjectType.Directory)
                {
                    DownloadDirectory(item.FullName, location + item.Name);
                }
                else
                {
                    client.DownloadFile(item.FullName,location + item.Name);
                }
            }
        }

        private void Upload_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if(ofd.ShowDialog() == true)
            {
                client.UploadFile(ofd.FileName, currentRoot + System.IO.Path.GetFileName(ofd.FileName));
                
            }

            UpdateList();
        }

        private void Reload_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            UpdateList();
        }

        private void Delete_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in FileDisplayPort.SelectedItems)
            {
                try
                {
                    client.DeleteFile(((Item)item).Name);
                }
                catch (Exception ex) { }

                try
                {
                    client.DeleteFile(((Item)item).Name);
                }
                catch (Exception ex) { }

            }

            UpdateList();            
        }

        private void UpdateList()
        {
            FileDisplayPort.Items.Clear();
            
            currentDisplayItems = client.GetListing(currentRoot);

            switch (sortby)
            {
                case "name":
                    currentDisplayItems = (from item in currentDisplayItems orderby item.FullName descending select item).ToArray();
                    break;
                case "-name":
                    currentDisplayItems = (from item in currentDisplayItems orderby item.FullName ascending select item).ToArray();
                    break;
                case "type":
                    currentDisplayItems = (from item in currentDisplayItems orderby (item.Type != FtpFileSystemObjectType.Directory ? IconReader.GetDescription(System.IO.Path.GetExtension(item.FullName)) : "Directory") descending select item).ToArray();
                    break;
                case "-type":
                    currentDisplayItems = (from item in currentDisplayItems orderby (item.Type != FtpFileSystemObjectType.Directory ? IconReader.GetDescription(System.IO.Path.GetExtension(item.FullName)) : "Directory") ascending select item).ToArray();
                    break;
                case "mod":
                    currentDisplayItems = (from item in currentDisplayItems orderby item.Modified descending select item).ToArray();
                    break;
                case "-mod":
                    currentDisplayItems = (from item in currentDisplayItems orderby item.Modified ascending select item).ToArray();
                    break;
                default:
                    break;
            }

            FileDisplayPort.Items.Add(new Item { Name = "..", Type = "Parent", Modified = DateTime.Now });
            foreach (var item in currentDisplayItems)
            {
                FileDisplayPort.Items.Add(new Item
                {
                    Icon = item.Type != FtpFileSystemObjectType.Directory ? IconReader.GetFileIcon(System.IO.Path.GetExtension(item.FullName), IconReader.IconSize.Large, false) : IconReader.GetFolderIcon(IconReader.IconSize.Large, IconReader.FolderType.Open),
                    Name = item.FullName,
                    Type = item.Type != FtpFileSystemObjectType.Directory ? IconReader.GetDescription(System.IO.Path.GetExtension(item.FullName)) : "Directory",
                    Modified = item.Modified
                });
            }           
        }

        public static string NormalisePath(string path)
        {
            return new Regex(@"\.{2}/.*/(?=\.\.)").Replace(path, "");
        }

        public void SortingClick(object sender, RoutedEventArgs e)
        {
            string column = ((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString();

            switch (column)
            {
                case "Name":
                    if(sortby == "name")
                    {
                        sortby = "-name";
                    }
                    else
                    {
                        sortby = "name";
                    }
                    
                    break;
                case "Type":

                    if (sortby == "type")
                    {
                        sortby = "-type";
                    }
                    else
                    {
                        sortby = "type";
                    }

                    break;
                case "Modified":

                    if (sortby == "mod")
                    {
                        sortby = "-mod";
                    }
                    else
                    {
                        sortby = "mod";
                    }

                    break;
            }

            UpdateList();
        }

        private void Download_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            var selected = FileDisplayPort.SelectedItems;
            foreach (var item in selected)
            {
                if (!(((Item)item).Type == "Parent"))
                {
                    Download(((Item)item).Name);
                }               
            }
        }
    }

    public class Item
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime Modified { get; set; }
    }

    public class NetCred
    {
        public string host;
        public string username;
        public string password;
    }
}