using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Linq;

namespace PM
{
    /// <summary>
    /// Interaction logic for BeaconsList.xaml
    /// </summary>
    public partial class BeaconsList : UserControl
    {
        public class LBeacon
        {
            public string Name { get; set; }
            public string UUID { get; set; }
            public string Longitude { get; set; }
            public string Latitude { get; set; }
            public string Level { get; set; }
        }

        public List<LBeacon> LBeacons { get; set; }
        public ProjectsList.ProjectFiles Project { get; set; }
        public string[] ConfigFile { get; set; }

        public BeaconsList(ProjectsList.ProjectFiles project)
        {
            DataContext = this;
            InitializeComponent();
            LBeacons = new List<LBeacon>();
            Project = project;
            ConfigFile = new string[13];
            ConfigFile[0] = "coordinate_X=";
            ConfigFile[1] = "coordinate_Y=";
            ConfigFile[2] = "coordinate_Z=";
            ConfigFile[3] = "filename=broadcast.txt";
            ConfigFile[4] = "filepath=/home/pi/LBeacon/doc/";
            ConfigFile[5] = "maximum_number_of_devices=10";
            ConfigFile[6] = "number_of_groups=3";
            ConfigFile[7] = "number_of_messages=15";
            ConfigFile[8] = "number_of_push_dongles=2";
            ConfigFile[9] = "RSSI_coverage=60";
            ConfigFile[10] = "gateway_addr=";
            ConfigFile[11] = "gateway_port=";
            ConfigFile[12] = "local_client_port=";
        }

        private void lbBeaconList_Loaded(object sender, RoutedEventArgs e)
        {
            XElement root = XElement.Load(Project.FullPath.Replace(".rvt", ".xml"));
            foreach (XElement node in root.Element("region").Elements("node"))
            {
                LBeacon beacon = new LBeacon();
                beacon.Name = (string)node.Attribute("name");
                beacon.Latitude = (string)node.Attribute("lat");
                beacon.Longitude = (string)node.Attribute("lon");
                beacon.UUID = (string)node.Attribute("id");
                beacon.Level = (string)node.Attribute("level");
                LBeacons.Add(beacon);
            }
            lbBeaconList.ItemsSource = LBeacons;
        }

        private void lbBeaconList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LBeacon beacon = (LBeacon)lbBeaconList.SelectedItem;
            tbName.Text = beacon.Name;
            tbLatitude.Text = beacon.Latitude;
            tbLongtitude.Text = beacon.Longitude;
            tbLevel.Text = beacon.Level;
            tbUUID.Text = beacon.UUID;
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            foreach (LBeacon beacon in LBeacons)
            {
                string folderPath = Project.FullPath.Replace(".rvt", "_" + beacon.Name);
                Directory.CreateDirectory(folderPath);
                using (StreamWriter file = new StreamWriter(folderPath + "\\conifg.conf"))
                {
                    for (int i = 0; i < ConfigFile.Length; i++)
                    {
                        if (i == 0)
                        {
                            file.WriteLine(ConfigFile[i] + beacon.Longitude);
                        }
                        else if (i == 1)
                        {
                            file.WriteLine(ConfigFile[i] + beacon.Latitude);
                        }
                        else if (i == 2)
                        {
                            file.WriteLine(ConfigFile[i] + beacon.Level);
                        }
                        else
                        {
                            file.WriteLine(ConfigFile[i]);
                        }
                    }
                }
            }
            MessageBox.Show("Success");
        }
    }
}
