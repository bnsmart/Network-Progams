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
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace DocumentIconDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string currentDir;
        Dictionary<string, List<ListBoxData>> pathList = new Dictionary<string, List<ListBoxData>>();
        string path;

        class ListBoxData
        {
            public BitmapSource ItemIcon { get; set; }
            public string ItemText { get; set; }
            public string ItemPath { get; set; }
            public string ItemType { get; set; }
            public string CurrentDir { get; set; }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            path = getPath();
            lstBox.ItemsSource = DataList(path, null);
        }

        List<ListBoxData> DataList(string path, List<ListBoxData> data)
        {
            
            if (!pathList.ContainsKey(path))
            {
                pathList.Add(path,Searcher(1, 1, path));
            }
            currentDir = path;
            List<ListBoxData> pulledList = null;
            pathList.TryGetValue(path, out pulledList);
            showBackButton();
            return pulledList;

        }

        string getPath()
        {
            if (IsWindows10())
            {
                return GetLine("config.txt", 2);
            }
            else
            {
                return GetLine("config.txt", 1); 
            }
        }

        static bool IsWindows10()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            string productName = (string)reg.GetValue("ProductName");

            return productName.StartsWith("Windows 10");
        }

        string GetLine(string fileName, int line)
        {
            using (var sr = new StreamReader(fileName))
            {
                for (int i = 1; i < line; i++)
                sr.ReadLine();
                //DirExists(sr.ReadLine());
                return sr.ReadLine();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void showBackButton()
        {
            if (currentDir == path)
            {
                BackButt.Visibility = Visibility.Hidden;
                //MessageBox.Show(currentDir + "no!");
            }
            else
            {
                //MessageBox.Show(currentDir);
                BackButt.Visibility = Visibility.Visible;
            }
        }

        public BitmapImage FolderIcon()
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(@"C:\Users\bsmart\Documents\Visual Studio 2015\Projects\DocumentIconDemo\DocumentIconDemo\Images\Folder_16x.png");
            b.EndInit();
            return(b);
        }

        public void DirExists(string NewDirectory)
        {
            if (Directory.Exists(NewDirectory) == false)
            {
                Environment.Exit(0);
            }
        }

        List<ListBoxData> Searcher(int Dir, int File, string NewDirectory)
        {
            currentDir = NewDirectory;
            var data = new List<ListBoxData>();

            if (Dir == 1)
            {
               
                string[] dir = Directory.GetDirectories(NewDirectory);

                foreach (var filePath in dir)
                {
                    var itm = new ListBoxData() { ItemIcon = FolderIcon(), ItemText = System.IO.Path.GetFileName(filePath), ItemPath = filePath, ItemType = "Dir"};
                    data.Add(itm);
                }
            }

            if (File == 1)
            {
                string[] file = Directory.GetFiles(NewDirectory);
                foreach (var filePath in file)
                {
                    var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
                    var bmpSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                sysicon.Handle,
                                System.Windows.Int32Rect.Empty,
                                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    sysicon.Dispose();

                    var itm = new ListBoxData() { ItemIcon = bmpSrc, ItemText = System.IO.Path.GetFileName(filePath), ItemPath = filePath, ItemType = "File" };
                    data.Add(itm);
                }
            }
            return data;
        }

        private void Updater(List<ListBoxData> d)
        {
            lstBox.ItemsSource = d;
        }

        private void lstBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstBox.SelectedIndex > -1)
            {
                string NewDir = ((ListBoxData)lstBox.SelectedItem).ItemPath;
                string ItemType = ((ListBoxData)lstBox.SelectedItem).ItemType;

                if (ItemType == "Dir")
                {
                    //DataList(data);
                    //MessageBox.Show(NewDir);
                    lstBox.ItemsSource = DataList(NewDir, null);
                    lstBox.SelectedIndex = -1;
                }
                else
                {
                    if (File.Exists(NewDir))
                    {
                        Process Proc = new Process();
                        Proc.StartInfo.FileName = NewDir;
                        try
                        {
                            Proc.Start();
                            lstBox.SelectedIndex = -1;
                        }
                        catch
                        {
                            MessageBox.Show("File not found");
                            lstBox.SelectedIndex = -1;
                        }
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string newPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDir, @"..\"));
            //Searcher(1, 1, newPath);
            lstBox.ItemsSource = DataList(newPath, null);
            
        }
    }
}
