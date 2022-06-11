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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private GMapPolygon outestRadius;
        private List<PointLatLng> outestRadiusPoints;
        private string outestRadiusName = "OutestRadius";
        private GMapOverlay radiusOverlay = new GMapOverlay(id: "radiusOverlay");
        public Form1()
        {
            InitializeComponent();
            this.LoadMap();
        }
        void LoadMap()
        {
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.Position = new PointLatLng(13.736717, 100.523186);
            gMapControl1.MinZoom = 5;
            gMapControl1.MaxZoom = 100;
            gMapControl1.Zoom = 10;
            gMapControl1.Overlays.Add(radiusOverlay);
            this.generateOutestRadius();
        }
        void generateOutestRadius()
        {
            outestRadiusPoints = new List<PointLatLng>();
            outestRadiusPoints.Add(new PointLatLng(20.077023, 97.915022));
            outestRadiusPoints.Add(new PointLatLng(19.249010, 97.583045));
            outestRadiusPoints.Add(new PointLatLng(14.697227, 105.805946));
            outestRadiusPoints.Add(new PointLatLng(5.608590, 101.430704));
            outestRadius = new GMapPolygon(outestRadiusPoints, this.outestRadiusName);
            radiusOverlay.Polygons.Add(outestRadius);

        }
    }
}
