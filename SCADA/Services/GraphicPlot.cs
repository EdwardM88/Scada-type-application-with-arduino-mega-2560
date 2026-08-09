using LiveCharts;
using LiveCharts.Wpf;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSCADA
{
    public class GraphicPlot
    {
        Analytics analytics = new Analytics();

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public double Values { get; set; }
        public DateTime Date { get; set; }

        public SeriesCollection SeriesCollection { get; set; }

       
    }
}
