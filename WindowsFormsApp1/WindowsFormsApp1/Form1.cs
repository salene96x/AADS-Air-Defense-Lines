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
    public partial class Form1 : Form
    {
        private GMapOverlay markersOverlay = new GMapOverlay(id: "markersOverlay");
        private List<PointLatLng> outestRadiusPoints;
        private GMapOverlay polygonOverlay = new GMapOverlay(id: "polygonsOverlay");
        private List<string> airportICAO;
        private List<PointLatLng> airportICAOPoints;
        public Form1()
        {
            InitializeComponent();
            this.LoadMap();
            this.WindowState = FormWindowState.Maximized;
            //this.createAirportMarkers();
            //this.getAirportData();
        }
        void LoadMap()
        {
            //gMapControl1.OnPolygonClick += Map_OnPolygonClick;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.Position = new PointLatLng(13.736717, 100.523186);
            gMapControl1.MinZoom = 5;
            gMapControl1.MaxZoom = 100;
            gMapControl1.Zoom = 10;
            this.readJson();
            gMapControl1.Overlays.Add(polygonOverlay);
            gMapControl1.Overlays.Add(markersOverlay);
            gMapControl1.Height = Screen.PrimaryScreen.Bounds.Height;
            gMapControl1.Width = Screen.PrimaryScreen.Bounds.Width;
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
            var jsonString = File.ReadAllText(@"C:\Users\Glaciiaz\Documents\GitHub\AADS-Air-Defense-Lines\FIR_thai.json");
            var jsonObject = JObject.Parse(jsonString);
            var jArray = (JArray)jsonObject["geometry"]["rings"];
            var jsonStringTHPolygon = File.ReadAllText(@"C:\Users\Glaciiaz\Documents\GitHub\AADS-Air-Defense-Lines\thailand_polygon.json");
            var jsonObjectTHPolygon = JObject.Parse(jsonStringTHPolygon);
            var jArrayTHPolygon = (JArray)jsonObjectTHPolygon["features"][0]["geometry"]["coordinates"];
            //int i = 0;
            //foreach (var j in jArrayTHPolygon)
            //{
            //    foreach (var x in j)
            //    {
            //        string name = "Thailand Main Land";
            //        if (i == 727)
            //        {
            //            List<PointLatLng> pointsEachPolygon = new List<PointLatLng>();
            //            int index = 0;
            //            foreach (var z in x)
            //            {
            //                //if ((double)z[1] >= 6.27 && (double)z[0] >= 99.36)
            //                //{
            //                //    Console.WriteLine("Found at " + index);
            //                //}\
            //                //satun index = 120111
            //                //ranong index = 140000
            //                pointsEachPolygon.Add(new PointLatLng((double)z[1], (double)z[0]));
            //                Console.WriteLine(index);
            //                index++;
            //            }
            //            GMapPolygon subPolygonTH = new GMapPolygon(pointsEachPolygon, name);
            //            subPolygonTH.IsHitTestVisible = true;
            //            subPolygonTH.Fill = new SolidBrush(Color.FromArgb(0, Color.Red));
            //            subPolygonTH.Stroke = new Pen(Color.Black, 2);
            //            polygonOverlay.Polygons.Add(subPolygonTH);
            //        }
            //        i++;
            //    }
                List<PointLatLng> pointsRings = new List<PointLatLng>();
                foreach (var x in jArray)
                {
                    foreach (var z in x)
                    {
                        pointsRings.Add(new PointLatLng((double)z[1], (double)z[0]));
                    }
                }
                GMapPolygon polygon = new GMapPolygon(pointsRings, "rings");
                polygon.Fill = new SolidBrush(Color.FromArgb(0, Color.DarkGray));
                polygon.Stroke = new Pen(Color.DarkBlue, 3);
                polygonOverlay.Polygons.Add(polygon);
                polygon.IsHitTestVisible = true;
            }
        private void gMapControl1_OnPolygonDoubleClick(GMapPolygon item, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Console.WriteLine(item.Name);
            }
        }

        private void gMapControl1_OnPolygonEnter(GMapPolygon item)
        {
            Console.WriteLine(item.Name.ToString());
        }

        private void gMapControl1_OnPolygonClick(GMapPolygon item, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Console.WriteLine(item.Name);
            }
        }

        private void gMapControl1_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            var mousePosition = gMapControl1.FromLocalToLatLng(e.X, e.Y);
            Console.WriteLine(mousePosition);
        }
    }
    public class Airport
    {
        public string iata { get; set; }
        public string icao { get; set; }
        public double airportLat { get; set; }
        public double aiportLon { get; set; }
    }
    public class ThailandPolygon
    {
        public List<List<List<List<double>>>> coordinates;
    }
}
