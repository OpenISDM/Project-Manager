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
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Shell;
using System.Xml.Linq;

namespace PM
{
    public partial class ProjectsList : System.Windows.Controls.UserControl
    {
        /* 
         * Global variable for the Project (folder) selected
         */
        public ProjectFiles SelectedProject { get; set; }

        /*
         * Constructor for the project list
         */
        public ProjectsList()
        {
            DataContext = this;
            InitializeComponent();
        }

        /*
         * Establishing what each project (file) consists of
         */
        public class ProjectFiles
        {
            public string Title { get; set; }
            public string FullPath { get; set; }
            public System.IO.FileAttributes Attr { get; set; }
            public DateTime AccsTime { get; set; }
            public BitmapSource Icon { get; set; }
            public BitmapSource RenderedImg { get; set; }
            public int BeaconCount { get; set; }
            public int Progress { get; set; }
        }

        /* Updates the listview to show the files in the current directory */
        private void updateFiles(string proj = null)
        {
            Random rnd = new Random();
            if (String.IsNullOrEmpty(proj) || proj == "Please Select")
            {
                lbProject.ItemsSource = null;
                return;
            }
            List<ProjectFiles> lopf = new List<ProjectFiles>();
            var fileNames = Directory.GetFiles(proj, "*.rvt");
            foreach (string fn in fileNames)
            {
                FileInfo fi = new FileInfo(fn);
                var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(fi.FullName);
                var bmpSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        sysicon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                sysicon.Dispose();
                ShellFile sf = ShellFile.FromFilePath(fi.FullName);
                //sf.Thumbnail.FormatOption = ShellThumbnailFormatOption.ThumbnailOnly;
                var renderedImg = sf.Thumbnail.ExtraLargeBitmapSource;
                var bc = 0;

                try
                {
                    Console.WriteLine("Trying to load XML for {0}......", fi.Name);
                    XDocument objDoc = XDocument.Load(fi.FullName.Replace(".rvt", ".xml"));
                    bc = objDoc.Descendants("node").Count();
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Error Finding XML");
                }

                lopf.Add(new ProjectFiles() { Title = fi.Name, FullPath = fi.FullName, Attr = fi.Attributes, AccsTime = fi.LastWriteTime, Icon = bmpSrc, RenderedImg = renderedImg, BeaconCount = bc, Progress = rnd.Next(0, 100) });

            }
            lbProject.ItemsSource = lopf;
        }

        /* 
         * Opens up the Revit file when the users double click the item.
         */
        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                System.Windows.Controls.ListBox lb = (System.Windows.Controls.ListBox)sender;
                ProjectFiles p = (ProjectFiles)lb.SelectedItem;
                if (p != null)
                {
                    Process.Start(p.FullPath);
                }
            }
            updateFiles();
        }

        /* 
         * Update the panel on the right hand side to show most recently selected file
         */
        private void updateMostRecentFile(ProjectFiles p)
        {
            RFImage.Source = p.RenderedImg;
            RFLabel1.Content = p.Title;
            RFLabel2.Content = p.AccsTime;
            //RFLabel3.Content = p.Progress;
            RFLabel4.Content = String.Format("Number of Beacons: {0}", p.BeaconCount);

            pbStatus.Value = p.Progress;
        }

        /*
         * opens the file when users click on the open button
         */
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                System.Windows.Controls.Button btn = (System.Windows.Controls.Button)sender;
                ProjectFiles p = btn.DataContext as ProjectFiles;
                Process.Start(p.FullPath);
            }
            updateFiles();
        }

        /* 
         * Deletes the file when users click on the delete button 
         */
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    System.Windows.Controls.Button btn = (System.Windows.Controls.Button)sender;
                    ProjectFiles p = btn.DataContext as ProjectFiles;
                    File.Delete(p.FullPath);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
            }
            updateFiles();
        }

        /*
         * Event handler for project select
         */
        private void projectSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var comboBox = sender as System.Windows.Controls.ComboBox;

            // ... Set SelectedItem as Window Title.
            string value = comboBox.SelectedItem as string;
            updateFiles(value);
        }

        /*
         * loads the project select
         */
        private void projectSelect_Loaded(object sender, RoutedEventArgs e)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string[] subdirectories = Directory.GetDirectories(desktop);


            List<string> data = new List<string>();
            data.Add("Please Select");
            data.AddRange(subdirectories);

            // ... Get the ComboBox reference.
            var comboBox = sender as System.Windows.Controls.ComboBox;

            // ... Assign the ItemsSource to the List.
            comboBox.ItemsSource = data;

            // ... Make the first item selected.
            comboBox.SelectedIndex = 0;
        }

        /*
         * Event handler for project selection changed
         */
        private void lbProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                System.Windows.Controls.ListBox lb = (System.Windows.Controls.ListBox)sender;
                ProjectFiles p = (ProjectFiles)lb.SelectedItem;
                if (p != null)
                {
                    updateMostRecentFile(p);
                    SelectedProject = p;
                }
            }
        }

        private void BtnBeaconList_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProject != null)
            {
                this.Content = new BeaconsList(SelectedProject);
            }
        }
    }
}


