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
        private GMapOverlay THOverlay = new GMapOverlay(id: "THOverlay");
        private GMapOverlay THRingsOverlay = new GMapOverlay(id: "THRingsOverlay");
        private GMapOverlay polygonOverlay = new GMapOverlay(id: "polygonOverlay");
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
            gMapControl1.Overlays.Add(THOverlay);
            gMapControl1.Overlays.Add(THRingsOverlay);
            gMapControl1.Overlays.Add(markersOverlay);
            gMapControl1.Overlays.Add(polygonOverlay);
            gMapControl1.Height = Screen.PrimaryScreen.Bounds.Height;
            gMapControl1.Width = Screen.PrimaryScreen.Bounds.Width;
            gMapControl1.Manager.BoostCacheEngine = true;
            //this.generateOutestRadius();
            this.createAirportMarkers();
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
                createCirclePolygon(airportICAOPoints[i], 100000);
            }
        }
        void createCirclePolygon(PointLatLng point, double radius)
        {
            int segments = 1000;

            List<PointLatLng> gpollist = new List<PointLatLng>();

            for (int i = 0; i < segments; i++)
                gpollist.Add(FindPointAtDistanceFrom(point, i, radius / 1000));

            GMapPolygon gpol = new GMapPolygon(gpollist, "pol")
            {
                Fill = new SolidBrush(Color.FromArgb(0, Color.Red)),
                Stroke = new Pen(Color.Black, 0)
            };

            polygonOverlay.Polygons.Add(gpol);
        }
        public static GMap.NET.PointLatLng FindPointAtDistanceFrom(GMap.NET.PointLatLng startPoint, double initialBearingRadians, double distanceKilometres)
        {
            const double radiusEarthKilometres = 6371.01;
            var distRatio = distanceKilometres / radiusEarthKilometres;
            var distRatioSine = Math.Sin(distRatio);
            var distRatioCosine = Math.Cos(distRatio);

            var startLatRad = DegreesToRadians(startPoint.Lat);
            var startLonRad = DegreesToRadians(startPoint.Lng);

            var startLatCos = Math.Cos(startLatRad);
            var startLatSin = Math.Sin(startLatRad);

            var endLatRads = Math.Asin((startLatSin * distRatioCosine) + (startLatCos * distRatioSine * Math.Cos(initialBearingRadians)));

            var endLonRads = startLonRad + Math.Atan2(
                          Math.Sin(initialBearingRadians) * distRatioSine * startLatCos,
                          distRatioCosine - startLatSin * Math.Sin(endLatRads));

            return new GMap.NET.PointLatLng(RadiansToDegrees(endLatRads), RadiansToDegrees(endLonRads));
        }

        public static double DegreesToRadians(double degrees)
        {
            const double degToRadFactor = Math.PI / 180;
            return degrees * degToRadFactor;
        }

        public static double RadiansToDegrees(double radians)
        {
            const double radToDegFactor = 180 / Math.PI;
            return radians * radToDegFactor;
        }
        void readJson()
        {
            outestRadiusPoints = new List<PointLatLng>();
            var jsonString = File.ReadAllText("FIR_thai.json");
            var jsonObject = JObject.Parse(jsonString);
            var jArray = (JArray)jsonObject["geometry"]["rings"];
            var jsonStringTHPolygon = File.ReadAllText("thailand_polygon.json");
            var jsonObjectTHPolygon = JObject.Parse(jsonStringTHPolygon);
            var jArrayTHPolygon = (JArray)jsonObjectTHPolygon["features"][0]["geometry"]["coordinates"][727];
            int i = 0;
            List<PointLatLng> points = new List<PointLatLng>();
            foreach (var pos in jArrayTHPolygon)
            {
                foreach (var point in pos)
                {
                    points.Add(new PointLatLng((double)point[1], (double)point[0]));
                }
            }
            GMapPolygon THPolygon = new GMapPolygon(points, "Thailand")
            {
                Fill = new SolidBrush(Color.FromArgb(0, Color.White))
            };
            THOverlay.Polygons.Add(THPolygon);
            //foreach (var j in jArrayTHPolygon)
            //{
            //    foreach (var x in j)
            //    {
            //        string name = "Thailand Main Land";
            //        if (i == 727)
            //        {
            //            int cntToThree = 0;
            //            List<PointLatLng> pointsEachPolygon = new List<PointLatLng>();
            //            int index = 0;
            //            foreach (var z in x)
            //            {
            //                pointsEachPolygon.Add(new PointLatLng((double)z[1], (double)z[0]));
            //                cntToThree = 0;

            //                //if ((double)z[1] >= 6.27 && (double)z[0] >= 99.36)
            //                //{
            //                //    Console.WriteLine("Found at " + index);
            //                //}\
            //                //satun index = 120111
            //                //ranong index = 14000;
            //                Console.WriteLine(index);
            //                index++;
            //                cntToThree++;
            //            }
            //            GMapPolygon subPolygonTH = new GMapPolygon(pointsEachPolygon, name);
            //            subPolygonTH.IsHitTestVisible = true;
            //            subPolygonTH.Fill = new SolidBrush(Color.FromArgb(0, Color.Red));
            //            subPolygonTH.Stroke = new Pen(Color.Black, 2);
            //            THOverlay.Polygons.Add(subPolygonTH);
            //        }
            //i++;
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
            THRingsOverlay.Polygons.Add(polygon);
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

        private void gMapControl1_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Console.WriteLine(item.LocalArea);
            }
        }
    }
}
