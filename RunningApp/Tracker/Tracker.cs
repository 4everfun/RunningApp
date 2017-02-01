using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Locations;

using RunningApp.Database;
using RunningApp.Exceptions;

namespace RunningApp.Tracker
{
    public class Tracker : Java.Lang.Object, ILocationListener
    {
        public delegate void TrackUpdatedEventHandler(object sender, TrackUpdatedEventArgs e);
        public event TrackUpdatedEventHandler TrackUpdated;

        protected Stopwatch stopwatch = new Stopwatch();

        public Track track { get; protected set; }
        protected TrackModel TrackModel;

        private LocationManager lm;
        private string lp;

        public Tracker(MainActivity c)
        {
            this.StartNewTrack();
            this.Initialize(c);
        }

        public Tracker(MainActivity c, Track track)
        {
            this.SetTrack(track);
            this.Initialize(c);
        }

        private void Initialize(MainActivity c)
        {
            // Initialize the location listener. Use Fine Accuarcy and receive updates as often as possible.
            this.lm = (LocationManager)c.GetSystemService(Context.LocationService);
            Criteria crit = new Criteria();
            crit.Accuracy = Accuracy.Fine;
            this.lp = this.lm.GetBestProvider(crit, true);
            this.lm.RequestLocationUpdates(this.lp, 0, 0.5f, this);
            this.lm.GetLastKnownLocation(this.lp);
        }

        public void StartNewTrack()
        {
            this.SetTrack(new Track());
            this.stopwatch.Reset();
            this.Extendable = true;

            TrackUpdatedEventArgs args = new TrackUpdatedEventArgs(this.track);
            this.OnTrackUpdated(args);
        }

        public void SetTrack(TrackModel track)
        {
            this.track = track.GetTrack();
            this.TrackModel = track;
            this.Extendable = false;
        }

        protected void SetTrack(Track track)
        {
            this.track = track;
            this.TrackModel = null;
            this.Extendable = false;
        }

        protected bool Extendable = false;
        protected bool AddToTrack = false;
        protected bool Paused = false;

        public bool IsTracking()
        {
            if (!this.Extendable) return false;
            return (this.Paused) ? true : this.AddToTrack;
        }

        public bool IsPaused()
        {
            if (!this.Extendable) return false;
            return this.Paused;
        }

        public void StartTracking()
        {
            if (!this.Extendable)
            {
                throw new NotExtendableException();
            }

            this.Paused = false;
            this.stopwatch.Start();
            this.AddToTrack = true;
            this.track.NewSegment();
            this.TrackLocation(this.lm.GetLastKnownLocation(this.lp));
        }

        public void StopTracking()
        {
            this.Paused = false;
            this.stopwatch.Stop();
            this.AddToTrack = false;
            this.Extendable = false;
        }

        public void PauseTracking()
        {
            this.Paused = true;
            this.stopwatch.Stop();
            this.AddToTrack = false;
            this.track.NewSegment();
        }

        public TrackModel GetOrInstantiateTrackModel()
        {
            if (this.TrackModel == null)
            {
                this.TrackModel = new TrackModel();
            }
            this.TrackModel.SetTrack(this.track);
            return this.TrackModel;
        }

        public TimeSpan GetTimeSpanTracking()
        {
            if (this.Extendable) return this.stopwatch.Elapsed;
            return this.track.GetTotalRunningTimeSpan();
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