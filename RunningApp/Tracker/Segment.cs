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
        public List<SerializableLocation> points { get; protected set; }

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
            for (int i = 1; i < this.points.Count; i++)
            {
                distance += this.points[i].Location.DistanceTo(this.points[i - 1].Location);
            }
            return distance;
        }

        public TimeSpan GetTimeSpan()
        {
            if (this.points.Count <= 0) return new TimeSpan();
            return TimeSpan.FromMilliseconds(this.points[this.points.Count - 1].Time - this.points[0].Time);
        }

        public double GetAvarageSpeed()
        {
            return this.GetTotalDistance() / 1000 / this.GetTimeSpan().TotalHours;
        }

        public void Add(Location location)
        {
            this.points.Add(new SerializableLocation(location));
        }

        public List<Location> GetLocations()
        {
            List<Location> l = new List<Location>();
            foreach (SerializableLocation p in this.points) l.Add(p.Location);
            return l;
        }

        public List<OxyPlot.DataPoint> GetDistanceTraveledDataPoints(Double TimeSpanOffset, Double DistanceOffset)
        {
            if (this.points.Count <= 0) return new List<OxyPlot.DataPoint>();

            List<OxyPlot.DataPoint> DataPoints = new List<OxyPlot.DataPoint>();

            Double DistanceTraveled = 0;
            DateTime FirstDateTime = this.points[0].GetDateTime();
            Location PreviousLocation = this.points[0].Location;

            foreach (SerializableLocation l in this.points)
            {
                DistanceTraveled += l.Location.DistanceTo(PreviousLocation);
                DataPoints.Add(new OxyPlot.DataPoint(OxyPlot.Axes.TimeSpanAxis.ToDouble(l.GetDateTime() - FirstDateTime) + TimeSpanOffset, DistanceTraveled + DistanceOffset));
                PreviousLocation = l.Location;
            }
            return DataPoints;
        }

        public List<OxyPlot.DataPoint> GetSpeedDataPoints(Double TimeSpanOffset)
        {
            if (this.points.Count <= 0) return new List<OxyPlot.DataPoint>();
            List<OxyPlot.DataPoint> DataPoints = new List<OxyPlot.DataPoint>();

            DateTime FirstDateTime = this.points[0].GetDateTime();
            SerializableLocation PreviousPoint = this.points[0];

            foreach (SerializableLocation l in this.points)
            {
                TimeSpan dt = l.GetDateTime() - PreviousPoint.GetDateTime();
                Double ds = l.Location.DistanceTo(PreviousPoint.Location);

                DataPoints.Add(new OxyPlot.DataPoint(OxyPlot.Axes.TimeSpanAxis.ToDouble(l.GetDateTime() - FirstDateTime) + TimeSpanOffset, ds/1000/dt.TotalHours));
                PreviousPoint = l;
            }
            return DataPoints;
        }
    }
}