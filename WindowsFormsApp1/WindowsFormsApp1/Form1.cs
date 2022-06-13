using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using IronXL;
using System.IO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using GMap.NET.WindowsForms.Markers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WindowsFormsApp1
{
    public class Airport {
        public string iata { get; set; }
        public string icao { get; set; }
        public double airportLat { get; set; }
        public double aiportLon { get; set; }
    }
    public class ThailandPolygon
    {
        public List<List<List<List<double>>>> coordinates; 
    }
    public partial class Form1 : Form
    {
        private GMapPolygon outestRadius;
        private GMapOverlay markersOverlay = new GMapOverlay(id: "markersOverlay");
        private List<PointLatLng> outestRadiusPoints;
        private string outestRadiusName = "OutestRadius";
        private GMapOverlay polygonOverlay = new GMapOverlay(id: "polygonsOverlay");
        private List<string> airportICAO;
        private List<PointLatLng> airportICAOPoints;
        public Form1()
        {
            InitializeComponent();
            this.LoadMap();
            //this.createAirportMarkers();
            //this.getAirportData();
        }
        void LoadMap()
        {
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.Position = new PointLatLng(13.736717, 100.523186);
            gMapControl1.MinZoom = 5;
            gMapControl1.MaxZoom = 100;
            gMapControl1.Zoom = 10;
            this.readJson();
            gMapControl1.Overlays.Add(polygonOverlay);
            gMapControl1.Overlays.Add(markersOverlay);
            //this.generateOutestRadius();
            //this.createAirportMarkers();
        }
        void getAirportData() 
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (var reader = new StreamReader(@"C:\Users\Glaciiaz\Downloads\airport.csv"))
            {
                airportICAO = new List<string>();
                airportICAOPoints = new List<PointLatLng>();
                while (!reader.EndOfStream)
                {
                    reader.ReadLine();
                    var line = reader.ReadLine();
                    var value = line.Split(',');
                    airportICAO.Add(value[1]);
                    airportICAOPoints.Add(new PointLatLng(Convert.ToDouble(value[3]), Convert.ToDouble(value[4])));
                }
            }
        }
        void createAirportMarkers()
        {
            GMapMarker marker;
            this.getAirportData();
            for (int i = 0; i < airportICAO.Count; i++)
            {
                marker = new GMarkerGoogle(airportICAOPoints[i], GMarkerGoogleType.red_dot);
                markersOverlay.Markers.Add(marker);
            }
        }
        void readJson()
        {
            outestRadiusPoints = new List<PointLatLng>();
            var jsonString = File.ReadAllText(@"C:\Users\Glaciiaz\Documents\GitHub\AADS-Air-Defense-Lines\thailand_polygon.json");
            var jsonObject = JObject.Parse(jsonString);
            var jArray = (JArray)jsonObject["features"][0]["geometry"]["coordinates"];
            Console.WriteLine(jArray[0][0]);
            List<PointLatLng> points = new List<PointLatLng>();
            foreach (var j in jArray)
            {
                foreach (var x in j)
                {
                    foreach (var z in x)
                    {
                        outestRadiusPoints.Add(new PointLatLng((double)z[1], (double)z[0]));
                    }
                }
            }
            GMapPolygon polygon = new GMapPolygon(outestRadiusPoints, "test");
            polygonOverlay.Polygons.Add(polygon);
        }
    }
}
