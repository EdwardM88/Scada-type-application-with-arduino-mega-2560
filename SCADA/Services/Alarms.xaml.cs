using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for Alarms.xaml
    /// </summary>
    public partial class Alarms : Window
    {
        MainWindow dashboard;
        public AlarmsSevere alarmsS;
        Sensors sensors;
        public ObservableCollection<AlarmsSevere> AlarmList { get; set; }
        public Alarms()
        {
            InitializeComponent();

            AlarmList = new ObservableCollection<AlarmsSevere>();

            dgAlarmHistory.ItemsSource = AlarmList; 
        }

        private void ComboMentenanta_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                string textIntrodus = AlarmcomboBox.Text;

                if (!AlarmcomboBox.Items.Contains(textIntrodus))
                {
                    AlarmcomboBox.Items.Add(textIntrodus);
                    MessageBox.Show($"'{textIntrodus}' a fost adăugat în listă!");
                }
            }
        }

        private void ObsMentenanta_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                string textIntrodus = ObscomboBox.Text;

                if (!ObscomboBox.Items.Contains(textIntrodus))
                {
                    ObscomboBox.Items.Add(textIntrodus);
                    MessageBox.Show($"'{textIntrodus}' a fost adăugat în listă!");
                }
            }
        }

        private void AlarmsBtn_Click(object sender, RoutedEventArgs e)
        {
        }


        private void DataBtn_Click(object sender, RoutedEventArgs e)
        {
            Analytics analitics = new Analytics();
            analitics.Show();
            this.Close();
        }
        private void Sensorsbtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            Sensors sensors = new Sensors(mainScreen: main);
            sensors.Show();
            this.Close();
        }

        private async void AlarmBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime? dataAlarm = dataA.SelectedDate;

            string sensorM = (SensorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            string AlarmM = (AlarmcomboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            string observationsA = ObscomboBox.Text;

            if (dataAlarm == null || string.IsNullOrEmpty(sensorM) || string.IsNullOrEmpty(AlarmM) || string.IsNullOrEmpty(observationsA))
            {
                System.Windows.MessageBox.Show("Please select a data,and complete all camps!");
                return;  
                
            }

            var alarmsDB = new AlarmPanel
            {
               sensorName = sensorM,
               dateAlarm = dataAlarm.Value,
               Atype = AlarmM,
               obsA = observationsA
            };

            try {
                string connectionM = "mongodb+srv://EdyDBForProjects:PasswordDBPrj@dbforprojects.6mlakrt.mongodb.net/?appName=DBForProjects";
                var settings = MongoClientSettings.FromConnectionString(connectionM);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                var client = new MongoClient(settings);
                var database = client.GetDatabase("ScadaSystem");
                var collection = database.GetCollection<AlarmPanel>("Alarms_Data");

                await collection.InsertOneAsync(alarmsDB);

                System.Windows.MessageBox.Show("Dates are saved in collection!");

                dataA.SelectedDate = null;
                SensorComboBox.SelectedIndex = -1;
                ObscomboBox.SelectedIndex = -1;
                AlarmcomboBox.SelectedIndex = -1;
            }
            catch(Exception ex) {
                System.Windows.MessageBox.Show($"Error on saving!{ex.Message}");
            }
        }

        private void Mainbtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
        
        public void Alarms_Loaded(object sender, RoutedEventArgs e)
        {
            string temp = dashboard?.lastTemp ?? "0";
            string light = dashboard?.lastLight ?? "0";

            if(alarmsS == null)
            {
                alarmsS = new AlarmsSevere(); 
            }

            alarmsS.DisplayAlarms("DHT11", temp);
            alarmsS.DisplayAlarms("Photoresistor", light);
        }
    }
}
