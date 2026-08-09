using LiveCharts;
using LiveCharts.Wpf;
using MongoDB.Driver;
using MySql.Data.MySqlClient; // Namespace-ul pentru MySQL
using System;
using System.CodeDom;
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
using System.Windows.Shapes;

namespace MiniSCADA
{
    /// <summary>
    /// Interaction logic for Analytics.xaml
    /// </summary>
    /// 


    public partial class Analytics : Window
    {
        public Analytics()
        {
            InitializeComponent();
            this.DataContext = this;    
            SeriesCollection = new SeriesCollection();

        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        private void Mainbtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        
        public void PlotValues(string sensordb)
        {
            try
            {
                string connectionM = "mongodb+srv://EdyDBForProjects:PasswordDBPrj@dbforprojects.6mlakrt.mongodb.net/?appName=DBForProjects";
                var settings = MongoClientSettings.FromConnectionString(connectionM);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                var client = new MongoClient(settings);
                var database = client.GetDatabase("ScadaSystem");
                var collection = database.GetCollection<SensorReadDB>("Sensor_Data");

                var dateDinDb = collection.Find(x => x.SensorName == sensordb.Trim())
                                  .SortByDescending(x => x.MeasureTime)
                                  .Limit(10)
                                  .ToList();

                MessageBox.Show($"Pentru '{sensordb}' am găsit {dateDinDb.Count} rânduri în baza de date.");

                var values = new ChartValues<double>();
                var labels = new List<string>();

                dateDinDb.Reverse();

                foreach (var item in dateDinDb)
                {
                    if (double.TryParse(item.Value.ToString(), out double valGrafic))
                    {
                        values.Add(valGrafic);

                    }
                    else
                    {
                        values.Add(0);
                    }

                    labels.Add(item.MeasureTime.ToString("HH:mm:ss"));
                }

                
                if (SeriesCollection == null) SeriesCollection = new SeriesCollection();
                SeriesCollection.Clear();
                SeriesCollection.Add(new LineSeries
                {
                    Title = "Sensor Value",
                    Values = values,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10
                });

                Labels = labels.ToArray();
                YFormatter = value => value.ToString("F2");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la conectarea la baza de date: {ex.Message}");

            }

        }



        private void AlarmsBtn_Click(object sender, RoutedEventArgs e)
        {
            Alarms alarms = new Alarms();
            alarms.Show();
            this.Close();   
        }

        private void DataBtn_Click(object sender, RoutedEventArgs e)
        {
        }
        private void Sensorsbtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();   
            Sensors sensors = new Sensors(mainWindow);
            sensors.Show();
            this.Close();   
        }

        private void MaintenanceBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void SelectSensor_Click(object sender, RoutedEventArgs e)
        {
            string sensordb = "";
            if(SensorDBCombobox.SelectedItem is ComboBoxItem item)
            {
                sensordb = item.Content.ToString();
            }
            else
            {
                sensordb=SensorDBCombobox.SelectedItem?.ToString() ?? "";
            }

            PlotValues(sensordb);
        }

        private void CalculsBtn_Click(object sender, RoutedEventArgs e)
        {
            string sensorC = sensorComboBox.Text.Trim();
            string operation = operationComboBox.Text.Trim();

            if ( string.IsNullOrEmpty(sensorC) || string.IsNullOrEmpty(operation))
            {
                MessageBox.Show("Please select an operation and a sensor!");
                return;
            }
            try
            {
                string connectionM = "mongodb+srv://EdyDBForProjects:PasswordDBPrj@dbforprojects.6mlakrt.mongodb.net/?appName=DBForProjects";
                var settings = MongoClientSettings.FromConnectionString(connectionM);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                var client = new MongoClient(settings);
                var database = client.GetDatabase("ScadaSystem");
                var collection = database.GetCollection<SensorReadDB>("Sensor_Data");
                var query = collection.AsQueryable()
                              .Where(x => x.SensorName == sensorC);

                var listaDate = query.ToList();

                if (listaDate.Any())
                {
                    if(operation == "Average")
                    {
                        double medie = query.Average(x => x.Value);
                        if (medie != 0)
                        {
                            TxtRezultatMedia.Text = medie.ToString("F2");
                        }
                        else
                        {
                            TxtRezultatMedia.Text = "No data";
                        }
                    }else if(operation == "Min")
                    {
                        double minimum = query.Min(x => x.Value);
                        TxtRezultatMedia.Text = minimum.ToString("F2");
                    }
                    else if (operation == "Max")
                    {
                        double maxim = query.Max(x => x.Value);
                        TxtRezultatMedia.Text = maxim.ToString("F2");
                    }else if (operation == "Deviation")
                    {
                        var valori = query.Select(x => x.Value).ToList();

                        if (valori.Any())
                        {

                            double medie = valori.Average();

                            double sum = valori.Sum(v => Math.Pow(v - medie, 2));


                            double deviatia = Math.Sqrt(sum / valori.Count);

                            TxtRezultatMedia.Text = deviatia.ToString("F2");
                        }
                        else
                        {
                            TxtRezultatMedia.Text = "0.00";
                        }
                    }
                } else
                {
                    TxtRezultatMedia.Text = "Zero date";
                    System.Windows.MessageBox.Show($"Baza de date nu a returnat nimic pentru '{sensorC}'. Verifică numele senzorului în MongoDB Compass!");
                }
            
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("A apărut o eroare: " + ex.Message);
            }
        }
    }
}
