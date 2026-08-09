using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.IO.Ports;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MiniSCADA
{
    //ProiectScadaDB parola PasswordDBPrj

    public static class DatabaseService
    {
        private static IMongoClient _client;
        private static IMongoDatabase _database;

        private const string connectionDB  = "mongodb+srv://EdyDBForProjects:PasswordDBPrj@dbforprojects.6mlakrt.mongodb.net/?appName=DBForProjects";
        public static void Initialize()
    {
        try
        {
            var settings = MongoClientSettings.FromConnectionString(connectionDB);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            
            _client = new MongoClient(settings);
            _database = _client.GetDatabase("ScadaSystem"); 

            _database.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            Console.WriteLine("Successfull connection to  MongoDB!");
        }
        catch (Exception ex)
        {
            throw new Exception("Eroare on connecting to MongoDB: " + ex.Message);
        }
    }

    }

    public class Measurement
    {
        public string Time { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string Light { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class SensorReadDB
    {
        [BsonId]
        public MongoDB.Bson.ObjectId SId { get; set; }
        public string SensorName { get; set; }
        public double Value { get; set; }
        public string Unit {  get; set; }
        public DateTime MeasureTime { get; set; }
    }
    public partial class MainWindow : Window
    {
        public ObservableCollection<Measurement> Measurement { get; set; }

        DispatcherTimer timer = new DispatcherTimer();

        public string lastTemp = "0";
        public string lastHum = "0";
        public string lastLight = "0";

        public SerialPort portArduino;
        public MainWindow()
        {
            InitializeComponent();
            LoadPorts();    
            
            Measurement = new ObservableCollection<Measurement>();
            DataMeasureLog.ItemsSource = Measurement;

            //Set a timer
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (sender, e) =>
            {
                Clocktxt.Text = DateTime.Now.ToString("HH:mm:ss");
            };

            timer.Start();
            Clocktxt.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        //Load available COM ports into the dropdown
        public void LoadPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            comboBoxPort.ItemsSource = ports;

            if (ports.Length > 0)
            {
                comboBoxPort.SelectedIndex = 0;
            }
        }

        private void cmbPorts_DropDownOpened(object sender, EventArgs e)
        {

        }

        private void bntConnect_Click(object sender, RoutedEventArgs e)
        {
            if (portArduino != null && portArduino.IsOpen)
            {
                portArduino.Close();
                btnConnect.Content = "Connect to Microcontroller";
                StatusBar.Text = "Disconnected";

                UpdateSensorStatus("Stopped", Brushes.Yellow);
                return;
            }

            //Include all ports form Arduino IDE
            if(comboBoxPort.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Please select a COM port first.");
                return;
            }

            portArduino = new SerialPort(comboBoxPort.SelectedItem.ToString(),9600);
            portArduino.DataReceived += ArduinoData_Update;

            try
            {
                portArduino.Open();
                btnConnect.Content = "Disconnected";
                StatusBar.Text = "Connected";
                System.Windows.Forms.MessageBox.Show("Microcontroller conncect succesfully!");

                UpdateSensorStatus("Waiting...", Brushes.LightGray);

            }
            catch (Exception ex)
            {
                UpdateSensorStatus("Not Connected", Brushes.Red);
                System.Windows.Forms.MessageBox.Show($"Error connecting: {ex.Message}");
            }
        }

        private void UpdateSensorStatus(string text,Brush color)
        {
            txtHum.Text = text;
            txtHum.Background = color;
            txtTemp.Text = text;
            txtTemp.Background = color;
            txtLight.Text = text;
            txtLight.Background = color;

        }

        public void ArduinoData_Update(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string dataArduino = portArduino.ReadLine();
                this.Dispatcher.Invoke(() =>
                {
                    string[] measure = dataArduino.Split('|');
                    string cleanData = dataArduino.Trim();
                    if (measure.Length == 3)
                    {
                        lastTemp = measure[0].Trim(); // DHT Temp
                        lastHum = measure[1].Trim();  // DHT Hum
                        lastLight = measure[2].Trim();// Photoresistor

                        if (lastTemp.ToLower().Contains("nan"))
                        {
                            txtTemp.Text = "Sensor Error";
                            txtTemp.Background = Brushes.Red;
                        }
                        else
                        {
                            txtTemp.Text = "Working";
                            txtTemp.Background = Brushes.Green;
                        }

                        if (double.TryParse(lastTemp, out double TempVal))
                        {
                            if (TempVal > 28)
                            {
                                txtHVAC.Text = "Working";
                                txtHVAC.Background = Brushes.DarkGreen;
                            }
                            else if (TempVal > 25)
                            {
                                txtHVAC.Text = "Working";
                                txtHVAC.Background = Brushes.Green;
                            }
                            else if (TempVal > 20)
                            {
                                txtHVAC.Text = "Working";
                                txtHVAC.Background = Brushes.LightGreen;
                            }
                            else
                            {
                                txtHVAC.Text = "Off";
                                txtHVAC.Background = Brushes.LightBlue;
                            }
                        }
                        else
                        {
                            txtHVAC.Text = "HVAC Off";
                            txtHVAC.Background = Brushes.Orange;
                        }

                        if (double.TryParse(lastHum, out double HumVal))
                        {
                            if (HumVal > 70)
                            {
                                txtMotor.Text = "Working";
                                txtMotor.Background = Brushes.LightGreen;
                            }
                            else if (HumVal > 60)
                            {
                                txtMotor.Text = "Working";
                                txtMotor.Background = Brushes.Green;
                            }
                            else if (HumVal > 50)
                            {
                                txtMotor.Text = "Working";
                                txtMotor.Background = Brushes.LawnGreen;
                            }
                            else if (HumVal > 45)
                            {
                                txtMotor.Text = "Working";
                                txtMotor.Background = Brushes.LightSeaGreen;
                            }
                            else
                            {
                                txtMotor.Text = "Off";
                                txtMotor.Background = Brushes.LightBlue;
                            }
                        }
                        else
                        {
                            txtHVAC.Text = "Motor Off";
                            txtHVAC.Background = Brushes.Orange;
                        }

                        if (lastHum.ToLower().Contains("nan"))
                        {
                            txtHum.Text = "Sensor Error";
                            txtHum.Background = Brushes.Red;
                        }
                        else
                        {
                            txtHum.Text = "Working";
                            txtHum.Background = Brushes.Green;
                        }
                        

                        if (int.TryParse(lastLight, out int lightVal))
                        {
                            txtLight.Text = "Working";
                            txtLight.Background = Brushes.LightGreen;

                            if(lightVal < 300)
                            {
                                txtLED.Text = "Working";
                                txtLED.Background = Brushes.Green;
                            }
                            else if(lightVal <500)
                            {
                                txtLED.Text = "Working";
                                txtLED.Background = Brushes.LightGreen;
                            }
                            else
                            {
                                txtLED.Text = "Off";
                                txtLED.Background= Brushes.LightBlue;
                            }
                        }
                        else
                        {
                            txtLight.Text = "Sensor Error";
                            txtLight.Background = Brushes.OrangeRed;
                        }



                            var newMeasurement = new Measurement()
                        {
                            Time = DateTime.Now.ToString("HH:mm:ss"),
                            Temperature = lastTemp + "°C",
                            Humidity = lastHum + "%",
                            Light = lastLight,
                        };

                        Measurement.Insert(0, newMeasurement);

                    }
                });

            }
            catch (Exception ex) { 
            }

           
        }
        

        private void Sensorsbtn_Click(object sender, RoutedEventArgs e)
        {
            Sensors sensors = new Sensors(this);
            sensors.Show();
        }   

        private void Mainbtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void AlarmsBtn_Click(object sender, RoutedEventArgs e)
        {
            Alarms alarms = new Alarms();   
            alarms.Show();
        }   

        private void DataBtn_Click(object sender, RoutedEventArgs e)
        {
            Analytics analytics = new Analytics();
            analytics.Show();
        }

        private void TestCon_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                if (lastTemp.Contains("nan") || lastHum.Contains("nan"))
                {
                    MessageBox.Show("Eroare: Can't save in database (NaN).");
                    return;
                }
                string connectionDB = "mongodb+srv://EdyDBForProjects:PasswordDBPrj@dbforprojects.6mlakrt.mongodb.net/?appName=DBForProjects";
                var client = new MongoClient(connectionDB);
                var database = client.GetDatabase("ScadaSystem");
                var collection = database.GetCollection<SensorReadDB>("Sensor_Data");

                DateTime now = DateTime.Now;

                //create collection and insert into mongoDB
                var reading = new List<SensorReadDB>
                {
                    new SensorReadDB
                    {
                        SensorName = "Temperatura",
                        Value = double.Parse(lastTemp, System.Globalization.CultureInfo.InvariantCulture),
                        Unit = "°C",
                        MeasureTime = now
                    },
                    new SensorReadDB {
                        SensorName = "Umiditate",
                        Value = double.Parse(lastHum, System.Globalization.CultureInfo.InvariantCulture),
                        Unit = "%",
                        MeasureTime = now
                    },
                    new SensorReadDB {
                        SensorName = "Lumina",
                        Value = double.Parse(lastLight, System.Globalization.CultureInfo.InvariantCulture),
                        Unit = "Raw",
                        MeasureTime = now
                    }
                };

                collection.InsertMany(reading);


                System.Windows.Forms.MessageBox.Show("Data was saved successfully in database!");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error on write: " + ex.Message);
             }  
            
        }

        private void SOSbtn_Click(object sender, RoutedEventArgs e)
        {
            if (portArduino != null)
            {
                portArduino.Close();
                StatusBar.Text = "Disconnected";
            }else
            {
                MessageBox.Show("Arduino is not connected!");
            }
        }

        private void Measurebtn_Click(object sender, RoutedEventArgs e)
        {
            if (portArduino != null && portArduino.IsOpen)
            {
                try
                {
                    portArduino.Write("M");
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Arduino is not connected!");
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
      
        }
    }
}
