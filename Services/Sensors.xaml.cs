using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Core.Connections;
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
using System.Windows.Shapes;


namespace MiniSCADA
{

    public partial class Sensors : Window
    {

        MainWindow dashboard;

       
        public Sensors(MainWindow mainScreen)
        {
            InitializeComponent();
            dashboard = mainScreen;
        }

        private void Mainbtn_Click(object sender, RoutedEventArgs e)
        {

            this.Close();   
        }

        public bool DHT11open = true;
        public bool PRopen = true;
        public bool HVACopen = true;
        public bool Motoropen = true;
        public bool LEDopen = true; 

        public void UpdateStatus(object sender, EventArgs e)
        {
            //for DHT11
            if (dashboard.lastTemp.ToLower().Contains("nan") && dashboard.lastHum.ToLower().Contains("nan"))
            {
                DHT11.Text = "No Temperature";
                DHT11.Background = Brushes.Yellow;
            }else if (dashboard.lastHum.ToLower().Contains("nan"))
            {
                DHT11.Text = "No Humidity";
                DHT11.Background = Brushes.Yellow;
            }else if(dashboard.lastHum.ToLower().Contains("nan"))
            {
                DHT11.Text = "Off";
                DHT11.Background = Brushes.OrangeRed;
            }
            else
            {
                DHT11.Text = "Work";
                DHT11.Background = Brushes.LightGreen;
            }

            //For photoresistor
            if(double.TryParse(dashboard.lastLight,out double Lightout))
            {   
                if(Lightout == 0)
                {
                    Photoresistor.Text = "Off";
                    Photoresistor.Background = Brushes.OrangeRed;
                }
            else
                {
                    Photoresistor.Text = "Work";
                    Photoresistor.Background = Brushes.LightGreen;
                }
            }
            

            //for "HVAC"
            if(dashboard.txtHVAC.Text == "Off")
            {
                Photoresistor.Text = "Off";
                Photoresistor.Background = Brushes.OrangeRed;
            }
            else
            {
                HVAC.Text = "Work";
                HVAC.Background = Brushes.LightGreen;
            }

            //for LEDS
            if(dashboard.txtLED.Text == "Off")
            {
                LEDS.Text = "Off";
                LEDS.Background = Brushes.OrangeRed;
            }
            else
            {
                LEDS.Text = "Work";
                LEDS.Background = Brushes.LightGreen;
            }

            //for servoMotor
            if (dashboard.txtMotor.Text == "Off")
            {
                Motor.Text = "Off";
                Motor.Background = Brushes.OrangeRed;
            }
            else
            {
                Motor.Text = "Work";
                Motor.Background = Brushes.LightGreen;
            }
        }

        public void Update_Control(object sender, EventArgs e)
        {
            if (dashboard.lastTemp.ToLower().Contains("nan") && dashboard.lastHum.ToLower().Contains("nan"))
            {
                DHT11_Control.Text = "nan";
            }
            else
            {
                DHT11_Control.Text = dashboard.lastTemp + " °C / " + dashboard.lastHum + " %";
            }

            if (double.TryParse(dashboard.lastLight, out double Lightout))
            {
                if(Lightout  < 300)
                {
                    Pr_Control.Text = "Poor Light";
                }
                else if(Lightout < 500 && Lightout > 300)
                {
                    Pr_Control.Text = "Good Light";
                }else if(Lightout < 1000 && Lightout > 500)
                {
                    Pr_Control.Text = "Optimal light";
                }
                else
                {
                    Pr_Control.Text = "To bright";
                }
            }

            if(HVACopen == true)
            {
                if(double.Parse(dashboard.lastTemp) > 28)
                {
                    HVAC_Control.Text = "Full open";
                }else if(double.Parse(dashboard.lastTemp) > 24 && double.Parse(dashboard.lastTemp) < 28)
                {
                    HVAC_Control.Text = "Partial open";
                }
                else if(double.Parse(dashboard.lastTemp) > 21 && double.Parse(dashboard.lastTemp) < 24)
                {
                    HVAC_Control.Text = "Closed";
                }
                else
                {
                    HVAC_Control.Text = "Full open";
                }
            }
        }

        private void AlarmsBtn_Click(object sender, RoutedEventArgs e)
        {
            Alarms alarmsWindow = new Alarms();
            alarmsWindow.Show();
            this.Close();  
        }


        private void DataBtn_Click(object sender, RoutedEventArgs e)
        {
            Analytics analyticsWindow = new Analytics();
            analyticsWindow.Show();
            this.Close();
        }
        private void Sensorsbtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ObMentenanta_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                string textIntrodus = ObcomboBox.Text;

                if (!ObcomboBox.Items.Contains(textIntrodus))
                {
                    ObcomboBox.Items.Add(textIntrodus);
                    System.Windows.MessageBox.Show($"'{textIntrodus}' a fost adăugat în listă!");
                }
            }
        }

        private async void MaintenanceBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime? dataM = DateMaintenance.SelectedDate;

            string sensorM = (SensorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            string maintenanceM = (Sensor2ComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            string observationsM = ObcomboBox.Text;

            if(dataM == null || string.IsNullOrEmpty(sensorM) || string.IsNullOrEmpty(maintenanceM) || string.IsNullOrEmpty(observationsM))
            {
                System.Windows.MessageBox.Show("Please select a data,and complete all camps!");
                return;
            }

            var maintenanceDB = new Maintenance
            {
                sensor = sensorM,
                dateMaintenance = dataM.Value,
                type = maintenanceM,
                observations = observationsM
            };

            try
            {
                string connectionM = "mongodb+srv://EdyDBForProjects:PasswordDBPrj@dbforprojects.6mlakrt.mongodb.net/?appName=DBForProjects";
                var settings = MongoClientSettings.FromConnectionString(connectionM);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                var client = new MongoClient(settings);
                var database = client.GetDatabase("ScadaSystem");
                var collection = database.GetCollection<Maintenance>("Maintenance_Data");

                await collection.InsertOneAsync(maintenanceDB);

                System.Windows.MessageBox.Show("Dates are saved in collection!");

                DateMaintenance.SelectedDate = null;
                Sensor2ComboBox.SelectedIndex = -1;
                SensorComboBox.SelectedIndex = -1;
                ObcomboBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error on saving!{ex.Message}");
            }

            Maintenance.DaysToMaintenance("DHT11", DHT11M);
            Maintenance.DaysToMaintenance("Photoresistor", PrM);
            Maintenance.DaysToMaintenance("HVAC", HVACM);
            Maintenance.DaysToMaintenance("Motor", MotorM);
            Maintenance.DaysToMaintenance("LEDS", LEDM);

        }

        private void DHT11_Click(object sender,RoutedEventArgs e)
        {
           if(dashboard.portArduino != null && dashboard.portArduino.IsOpen)
           {
                if (DHT11open == true)
                {
                    dashboard.portArduino.Write("D");
                    System.Windows.MessageBox.Show("DHT11 sensor is turned off!");
                    ConnBtn_DHT11.Content = "Connect";
                    DHT11open = false;

                    DHT11.Text = "Disconnected";
                    DHT11.Background = Brushes.Yellow;
                }
                else
                {
                    dashboard.portArduino.Write("d");
                    System.Windows.MessageBox.Show("DHT11 sensor is turned on!");
                    ConnBtn_DHT11.Content = "Disconnect";
                    DHT11open = true;

                    DHT11.Text = "Work";
                    DHT11.Background = Brushes.LightGreen;
                }
            }
            else
            {
               System.Windows.MessageBox.Show("Microcontroller is not connected!");
            }
        }

        private void PResistor_Click(object sender,RoutedEventArgs e)
        {
            if (dashboard.portArduino != null && dashboard.portArduino.IsOpen)
            {
                if (PRopen == true)
                {
                    dashboard.portArduino.Write("P");
                    System.Windows.MessageBox.Show("PhotoResistor is turned off!");
                    ConnBtn_presistor.Content = "Connect";
                    Photoresistor.Text = "Disconnected";
                    Photoresistor.Background = Brushes.Yellow;
                    PRopen = false;
                }
                else
                {
                    dashboard.portArduino.Write("p");
                    System.Windows.MessageBox.Show("PhotoResistor is turned on!");
                    Photoresistor.Text = "Work";
                    Photoresistor.Background = Brushes.LightGreen;
                    ConnBtn_presistor.Content = "Disconnect";
                    PRopen = true;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Microcontroller is not connected!");
            }
        }

        private void HVAC_Click(object sender, RoutedEventArgs e)
        {
            if (dashboard.portArduino != null && dashboard.portArduino.IsOpen)
            {
                if (HVACopen == true)
                {
                    dashboard.portArduino.Write("H");
                    System.Windows.MessageBox.Show("HVAC is turned off");
                    ConnBtn_HVAC.Content = "Connect";
                    HVACopen = false;

                    HVAC.Text = "Disconnected";
                    HVAC.Background = Brushes.Yellow;
                }
                else
                {
                    dashboard.portArduino.Write("h");
                    System.Windows.MessageBox.Show("HVAC is turned on");
                    ConnBtn_HVAC.Content = "Disconnect";
                    HVACopen = true;

                    HVAC.Text = "Work";
                    HVAC.Background = Brushes.LightGreen;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Microcontroller is not connected!");
            }
        }

        private void Motor_Click(object sender, RoutedEventArgs e)
        {
            if (dashboard.portArduino != null && dashboard.portArduino.IsOpen)
            {
                if (Motoropen == true)
                {

                    dashboard.portArduino.Write("S");
                    System.Windows.MessageBox.Show("Motor is turned off");
                    ConnBtn_Motor.Content = "Connect";
                    Motoropen = false;

                    Motor.Text = "Disconnected";
                    Motor.Background = Brushes.Yellow;
                }
                else
                {
                    dashboard.portArduino.Write("s");
                    System.Windows.MessageBox.Show("Motor is turned on");
                    ConnBtn_Motor.Content = "Disconnect";
                    Motoropen = true;

                    Motor.Text = "Work";
                    Motor.Background = Brushes.LightGreen;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Microcontroller is not connected!");
            }
        }

        private void LED_Click(object sender, RoutedEventArgs e)
        {
            if (dashboard.portArduino != null && dashboard.portArduino.IsOpen)
            {
                if (LEDopen == true)
                {
                    dashboard.portArduino.Write("L");
                    System.Windows.MessageBox.Show("LED lamp is turned off");
                    ConnBtn_LED.Content = "Connect";
                    LEDopen = false;

                    LEDS.Text = "Disconnected";
                    LEDS.Background = Brushes.Yellow;
                }
                else {

                    dashboard.portArduino.Write("l");
                    System.Windows.MessageBox.Show("LED lamp is turned on");
                    ConnBtn_LED.Content = "Disconnect";
                    LEDopen = false;

                    LEDS.Text = "Work";
                    LEDS.Background = Brushes.LightGreen;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Microcontroller is not connected!");
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateStatus(sender,e);


        }
    }
}
