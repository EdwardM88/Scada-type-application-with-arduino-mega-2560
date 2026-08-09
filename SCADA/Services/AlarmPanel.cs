using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace MiniSCADA
{
    public class AlarmPanel
    {
        MainWindow dashboard;
        Alarms alarms;
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string sensorName { get; set; }
        public DateTime dateAlarm { get; set; }
        public string Atype { get; set; }
        public string obsA {  get; set; }


   
    }

    public class AlarmsSevere
    {
        MainWindow dashboard;
        Alarms alarms;

        public string Timestamp { get; set; }   
        public string SensorName { get; set; }
        public string Message { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }



        public void DisplayAlarms(string sensorN,string value)
        {
            if (value == "0")
            {
                var insertAlarm = new AlarmsSevere
                {
                    Timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    SensorName = sensorN,
                    Message = "No data",
                    Priority = "2",
                    Status = "Active"
                };
                if (alarms == null)
                {
                    alarms = System.Windows.Application.Current.Windows.OfType<Alarms>().FirstOrDefault();
                }

                // 2. Acum verificăm dacă am găsit-o cu succes (dacă nu mai e null)
                if (alarms != null)
                {
                    // Ne asigurăm că lista ferestrei există
                    if (alarms.AlarmList == null)
                    {
                        alarms.AlarmList = new System.Collections.ObjectModel.ObservableCollection<AlarmsSevere>();
                    }

                    // 3. Adăugăm alarma! Acum variabila "alarms" știe exact cu cine vorbește.
                    alarms.AlarmList.Add(insertAlarm);
                }
            }
        }
   


    }
    
}
