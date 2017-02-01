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
    public class Track
    {
        [JsonProperty]
        protected List<Segment> segments;
        protected int currentSegment = 0;

        public Track()
        {
            this.segments = new List<Segment>();
        }

        public Track(List<Segment> track)
        {
            this.segments = track;
        }

        public int CountSegments()
        {
            return this.segments.Count;
        }

        public List<Segment> GetSegments()
        {
            return this.segments;
        }

        public void NewSegment()
        {
            this.segments.Add(new Segment());
            this.currentSegment = this.segments.Count - 1;
        }

        public void Add(Location location)
        {
            this.segments[this.currentSegment].Add(location);
        }

        public DateTime GetFirstDateTime()
        {
            if (this.segments.Count <= 0) return new DateTime();
            if (this.segments[0].CountPoints() <= 0) return new DateTime();
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddMilliseconds(this.segments[0].GetPoints()[0].Time).ToLocalTime();
            return dt;
        }

        public float GetTotalDistance()
        {
            float distance = 0;
            foreach (Segment s in this.segments)
            {
                distance += s.GetTotalDistance();
            }
            return distance;
        }

        public TimeSpan GetTotalTimeSpan()
        {
            TimeSpan TotalTime = new TimeSpan();
            foreach (Segment s in this.segments)
            {
                TotalTime = s.GetTimeSpan();
            }
            return TotalTime;
        }
    }
}