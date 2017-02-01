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
using Newtonsoft.Json;

namespace RunningApp.Tracker
{
    public class Segment
    {
        [JsonProperty]
        protected List<SerializableLocation> points;

        public Segment()
        {
            this.points = new List<SerializableLocation>();
        }

        public Segment(List<SerializableLocation> track)
        {
            this.points = track;
        }

        public float GetTotalDistance()
        {
            float distance = 0;
            for (int i = 0; i < this.points.Count; i++)
            {
                if (i == 0) continue;
                distance += this.points[i].Location.DistanceTo(this.points[i - 1].Location);
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
            this.points.Add(new SerializableLocation(location));
        }

        public List<Location> GetPoints()
        {
            List<Location> l = new List<Location>();
            foreach (SerializableLocation p in this.points) l.Add(p.Location);
            return l;
        }

        public int CountPoints()
        {
            return this.points.Count;
        }
    }
}