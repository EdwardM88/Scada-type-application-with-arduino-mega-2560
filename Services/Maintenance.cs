using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MiniSCADA
{
    public class Maintenance
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string sensor {  get; set; }
        public DateTime dateMaintenance {  get; set; }

        public string type { get; set; }
        public string observations { get; set; }
        public object MeasureTime { get; internal set; }
        public string Values { get; internal set; }

        public static async Task DaysToMaintenance(string text, System.Windows.Controls.TextBlock textboxWrite)
        {
            string sensorname = text;
            string connectionM = "mongodb+srv://EdyDBForProjects:PasswordDBPrj@dbforprojects.6mlakrt.mongodb.net/?appName=DBForProjects";
            var settings = MongoClientSettings.FromConnectionString(connectionM);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            var client = new MongoClient(settings);
            var database = client.GetDatabase("ScadaSystem");
            var collection = database.GetCollection<Maintenance>("Maintenance_Data");

            var filter = Builders<Maintenance>.Filter.Eq(e => e.sensor ,sensorname);
            var filterevent = await collection.Find(filter).FirstOrDefaultAsync();

            if (filterevent != null)
            {
                DateTime dateMongo = filterevent.dateMaintenance.ToLocalTime();
                TimeSpan dif = dateMongo - DateTime.Today;
                int daytoM = dif.Days;
                if (daytoM > 0)
                {
                    textboxWrite.Text = $"{daytoM} days to {"Maintenance"}.";
                }
                else if (daytoM == 0) { 
                    textboxWrite.Text = $"Maintenance is today!";
                }
                else
                {
                    textboxWrite.Text = $"Maintenance is not made!";
                }
            }
            else
            {
                textboxWrite.Text = $"No maintenance";
            }
        }

    }
}
