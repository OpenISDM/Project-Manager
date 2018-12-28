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
using System.Drawing;
using System.Diagnostics;

namespace PM
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class BeaconSpecs : UserControl
    {
        public BeaconSpecs()
        {
            InitializeComponent();
            var beacons = addBeacons();
            listBox.ItemsSource = beacons;
        }

        public class Beacons
        {
            public string Title { get; set; }
            public BitmapSource BeaconImage { get; set; }
            public string Completion { get; set; }
            public string Degree { get; set; }
            public string Radius { get; set; }
            public string FamilyPath { get; set; }
        }

        public List<Beacons> addBeacons()
        {
            List<Beacons> items = new List<Beacons>();
            //ShellFile shellfile = ShellFile.FromFilePath("C:\\");
            items.Add(new Beacons() { Title = "10M high Ceiling", BeaconImage = new BitmapImage(new Uri("Resources/Unidirectional.png",  UriKind.Relative)), Degree = "60°", Radius = "150 cm", Completion = "Max Coverage Area: \n 7.07 sq. meters", FamilyPath = @"C:\Users\bookjan\Desktop\BDE-master\Revit Family\2016LBeacon.rfa"});
            items.Add(new Beacons() { Title = "5M low Ceiling", BeaconImage = new BitmapImage(new Uri("Resources/omnidirectional.png", UriKind.Relative)), Degree = "30°", Radius = "90 cm", Completion = "Max Coverage Area: \n 2.54 sq. meters", FamilyPath = @"C:\Users\bookjan\Desktop\BDE-master\Revit Family\2016LBeacon.0001.rfa"});
            items.Add(new Beacons() { Title = "10M std Ceiling", BeaconImage = new BitmapImage(new Uri("Resources/omnidirectional.png", UriKind.Relative)), Degree = "60°", Radius = "90 cm", Completion = "Max Coverage Area: \n 5.04 sq. meters", FamilyPath = @"C:\Users\bookjan\Desktop\BDE-master\Revit Family\2016LBeacon.0002.rfa" });
            return items;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                System.Windows.Controls.Button btn = (System.Windows.Controls.Button)sender;
                Beacons p = btn.DataContext as Beacons;
                Console.WriteLine(p.FamilyPath);
                Process.Start(p.FamilyPath);
            }
        }
    }
}
