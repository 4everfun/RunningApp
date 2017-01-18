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
    public class Track
    {
        protected List<Segment> segments;
        protected int currentSegment = 0;

        public Track()
        {
            this.segments = new List<Segment>();
            this.NewSegment();
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