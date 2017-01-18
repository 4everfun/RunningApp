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

        public float GetTotalDistance()
        {
            float distance = 0;
            for (int i = 0; i < this.points.Count; i++)
            {
                if (i == 0) continue;
                distance += this.points[i].DistanceTo(this.points[i - 1]);
            }
            return distance;
        }

        public TimeSpan GetTimeSpan()
        {
            if (this.points.Count <= 0) return new TimeSpan();
            return TimeSpan.FromMilliseconds(this.points[this.points.Count - 1].Time - this.points[0].Time);
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