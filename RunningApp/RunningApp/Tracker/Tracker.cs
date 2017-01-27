using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;

using RunningApp.Tracker;

namespace RunningApp.Tracker
{
    public class Tracker : Java.Lang.Object, ILocationListener
    {
        public delegate void TrackUpdatedEventHandler(object sender, TrackUpdatedEventArgs e);
        public event TrackUpdatedEventHandler TrackUpdated;

        protected Stopwatch stopwatch = new Stopwatch();

        protected Track track;

        public Tracker(Context c)
        {
            this.StartNewTrack();
            this.Initialize(c);
        }

        public Tracker(Context c, Track track)
        {
            this.SetTrack(track);
            this.Initialize(c);
        }

        private void Initialize(Context c)
        {
            // Initialize the location listener. Use Fine Accuarcy and receive updates as often as possible.
            LocationManager lm = (LocationManager)c.GetSystemService(Context.LocationService);
            Criteria crit = new Criteria();
            crit.Accuracy = Accuracy.Fine;
            string lp = lm.GetBestProvider(crit, true);
            lm.RequestLocationUpdates(lp, 0, 0.5f, this);
        }

        public void StartNewTrack()
        {
            this.SetTrack(new Track());
            this.stopwatch.Reset();

            TrackUpdatedEventArgs args = new TrackUpdatedEventArgs(this.track);
            this.OnTrackUpdated(args);
        }

        protected void SetTrack(Track track)
        {
            this.track = track;
        }

        protected bool AddToTrack = false;
        protected bool Paused = false;

        public bool IsTracking()
        {
            return (this.Paused) ? true : this.AddToTrack;
        }

        public bool IsPaused()
        {
            return this.Paused;
        }

        public void StartTracking()
        {
            this.Paused = false;
            this.stopwatch.Start();
            this.AddToTrack = true;
        }

        public void StopTracking()
        {
            this.Paused = false;
            this.stopwatch.Stop();
            this.stopwatch.Reset();
            this.AddToTrack = false;
        }

        public void PauseTracking()
        {
            this.Paused = true;
            this.stopwatch.Stop();
            this.AddToTrack = false;
            this.track.NewSegment();
        }

        public TimeSpan GetTrackTimeSpan()
        {
            return this.track.GetTotalTimeSpan();
        }

        public TimeSpan GetTimeSpanTracking()
        {
            return this.stopwatch.Elapsed;
        }

        public float GetTotalDistance()
        {
            return this.track.GetTotalDistance();
        }

        public float GetAvergageSpeed()
        {
            return (float)((double)(this.GetTotalDistance() / 1000) / this.stopwatch.Elapsed.TotalHours);
        }

        protected void TrackLocation(Location location)
        {
            if (!this.AddToTrack) return;
            this.track.Add(location);

            TrackUpdatedEventArgs args = new TrackUpdatedEventArgs(this.track);
            this.OnTrackUpdated(args);
        }

        public void OnLocationChanged(Location location)
        {
            this.TrackLocation(location);
        }

        protected virtual void OnTrackUpdated(TrackUpdatedEventArgs e)
        {
            TrackUpdated?.Invoke(this, e);
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
        }
    }
}