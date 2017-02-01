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
    public class Track : Java.Lang.Object
    {
        [JsonProperty]
        public List<Segment> segments { get; protected set; }
        protected int currentSegment = 0;

        public Track()
        {
            this.segments = new List<Segment>();
        }

        public Track(List<Segment> track)
        {
            this.segments = track;
        }

        private static TimeSpan PauseTimeSpanBetweenSegments(Segment SegmentA, Segment SegmentB)
        {
            if (SegmentA == SegmentB) return new TimeSpan();
            return SegmentA.points[0].GetDateTime() - SegmentB.points[SegmentB.points.Count - 1].GetDateTime();
        }

        public List<Segment> NonEmptySegments()
        {
            List<Segment> NonEmptySegments = new List<Segment>();
            foreach (Segment s in this.segments)
            {
                if (s.points.Count <= 0) continue;
                NonEmptySegments.Add(s);
            }
            return NonEmptySegments;
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
            if (this.segments[0].points.Count <= 0) return new DateTime();
            return this.segments[0].points[0].GetDateTime();
        }

        public float GetTotalRunningDistance()
        {
            float distance = 0;
            foreach (Segment s in this.segments)
            {
                distance += s.GetTotalDistance();
            }
            return distance;
        }

        public TimeSpan GetTotalRunningTimeSpan()
        {
            TimeSpan TotalTime = new TimeSpan();
            foreach (Segment s in this.segments)
            {
                TotalTime += s.GetTimeSpan();
            }
            return TotalTime;
        }

        public float GetAvergageSpeed()
        {
            return (float)((double)(this.GetTotalRunningDistance() / 1000) / this.GetTotalRunningTimeSpan().TotalHours);
        }

        public TimeSpan GetTotalTimeSpan()
        {
            TimeSpan TotalTime = this.GetTotalRunningTimeSpan();
            for (int i = 1; i < this.NonEmptySegments().Count; i++)
            {
                TotalTime += Track.PauseTimeSpanBetweenSegments(this.NonEmptySegments()[i], this.segments[i - 1]);
            }
            return TotalTime;
        }

        public List<OxyPlot.DataPoint> GetDistanceTraveledDataPoints()
        {
            List<OxyPlot.DataPoint> DataPoints = new List<OxyPlot.DataPoint>();

            Double TimeSpanOffset = 0;
            Double DistanceOffset = 0;

            Segment PreviousSegment = this.NonEmptySegments()[0];

            foreach (Segment s in this.NonEmptySegments())
            {
                TimeSpanOffset += OxyPlot.Axes.TimeSpanAxis.ToDouble(Track.PauseTimeSpanBetweenSegments(s, PreviousSegment));

                DataPoints.AddRange(s.GetDistanceTraveledDataPoints(TimeSpanOffset, DistanceOffset));

                TimeSpanOffset = DataPoints[DataPoints.Count - 1].X;
                DistanceOffset = DataPoints[DataPoints.Count - 1].Y;

                DataPoints.Add(new OxyPlot.DataPoint(Double.NaN, Double.NaN));

                PreviousSegment = s;
            }
            return DataPoints;
        }

        public List<List<OxyPlot.DataPoint>> GetSegmentAvarageSpeedDataPoints()
        {
            List<List<OxyPlot.DataPoint>> Series = new List<List<OxyPlot.DataPoint>>();
            foreach (Segment s in this.NonEmptySegments())
            {
                Series.Add(new List<OxyPlot.DataPoint>
                {
                    new OxyPlot.DataPoint(OxyPlot.Axes.TimeSpanAxis.ToDouble(s.points[0].GetDateTime() - this.NonEmptySegments()[0].points[0].GetDateTime()), s.GetAvarageSpeed()),
                    new OxyPlot.DataPoint(OxyPlot.Axes.TimeSpanAxis.ToDouble(s.points[s.points.Count - 1].GetDateTime() - this.NonEmptySegments()[0].points[0].GetDateTime()), s.GetAvarageSpeed()),
                });
            }
            return Series;
        }

        public List<OxyPlot.DataPoint> GetAvergageSpeedDataPoints()
        {
            return new List<OxyPlot.DataPoint>
            {
                new OxyPlot.DataPoint(0, this.GetAvergageSpeed()),
                new OxyPlot.DataPoint(OxyPlot.Axes.TimeSpanAxis.ToDouble(this.NonEmptySegments()[this.NonEmptySegments().Count - 1].points[this.NonEmptySegments()[this.NonEmptySegments().Count - 1].points.Count - 1].GetDateTime() - this.NonEmptySegments()[0].points[0].GetDateTime()), this.GetAvergageSpeed())
            };
        }

        public List<OxyPlot.DataPoint> GetSpeedDataPoints()
        {
            List<OxyPlot.DataPoint> DataPoints = new List<OxyPlot.DataPoint>();

            Double TimeSpanOffset = 0;

            Segment PreviousSegment = this.NonEmptySegments()[0];

            foreach (Segment s in this.NonEmptySegments())
            {
                TimeSpanOffset += OxyPlot.Axes.TimeSpanAxis.ToDouble(Track.PauseTimeSpanBetweenSegments(s, PreviousSegment));

                DataPoints.AddRange(s.GetSpeedDataPoints(TimeSpanOffset));

                TimeSpanOffset = DataPoints[DataPoints.Count - 1].X;

                DataPoints.Add(new OxyPlot.DataPoint(Double.NaN, Double.NaN));

                PreviousSegment = s;
            }
            return DataPoints;
        }

        public double GetMaxSpeed()
        {
            double MaxSpeed = 0;
            foreach (OxyPlot.DataPoint dp in this.GetSpeedDataPoints())
            {
                if (dp.Y > MaxSpeed) MaxSpeed = dp.Y;
            }
            return MaxSpeed;
        }
    }
}