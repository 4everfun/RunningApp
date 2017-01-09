using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Graphics;
using Android.Locations;

namespace RunningApp.Tracker
{
    public class Segment
    {
        protected List<Location> points;

        public Segment()
        {
            this.points = new List<Location>();
        }

        public Segment(List<Location> track)
        {
            this.points = track;
        }

        public void Add(Location location)
        {
            this.points.Add(location);
        }

        public List<Location> GetPoints()
        {
            return this.points;
        }

        public int CountPoints()
        {
            return this.points.Count;
        }
    }
}