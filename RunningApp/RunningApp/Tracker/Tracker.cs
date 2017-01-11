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
using Android.Locations;

using RunningApp.Tracker;

namespace RunningApp.Tracker
{
    public class Tracker : Java.Lang.Object, ILocationListener
    {
        public delegate void TrackUpdatedEventHandler(object sender, TrackUpdatedEventArgs e);
        public event TrackUpdatedEventHandler TrackUpdated;

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

            TrackUpdatedEventArgs args = new TrackUpdatedEventArgs(this.track);
            this.OnTrackUpdated(args);
        }

        protected void SetTrack(Track track)
        {
            this.track = track;
        }

        public bool AddToTrack = false;

        public void StartTracking()
        {
            this.AddToTrack = true;
        }

        public void StopTracking()
        {
            this.AddToTrack = false;
        }

        public void PauseTracking()
        {
            this.StopTracking();
            this.track.NewSegment();
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