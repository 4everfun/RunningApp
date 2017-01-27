using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using RunningApp.Views;
using RunningApp.Exceptions;

namespace RunningApp.Fragments
{
    public class TrackerFragment : BaseFragment
    {
        protected MapView Map;
        protected Status Status;
        protected AlertDialog.Builder NoLocationAlert, NotOnMapAlert;
        protected Button btnStartStop, btnPause;
        protected Tracker.Tracker Tracker;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.Tracker, container, false);

            // Bind the MapView to a variable, so it can be used later in the activity
            this.Map = view.FindViewById<MapView>(Resource.Id.mapView);
            this.Status = view.FindViewById<Status>(Resource.Id.statusView);

            // Add the click event to the center button
            view.FindViewById<ImageButton>(Resource.Id.centerButton).Click += this.CenterMapToCurrentLocation;

            // Add the click event to the start and stop button
            this.btnStartStop = view.FindViewById<Button>(Resource.Id.btnStartStop);
            this.btnStartStop.Click += this.StartStopTracking;
            this.btnPause = view.FindViewById<Button>(Resource.Id.btnPause);
            this.btnPause.Click += this.PauseTracking;

            this.Map.SetTracker(this.Tracker);
            this.Status.SetTracker(this.Tracker);

            this.UpdateStartStopPauseButton();

            return view;
        }

        public override void OnAttach(Context c)
        {
            base.OnAttach(c);

            if ((c is MainActivity) == false) return;
            MainActivity a = (MainActivity)c;

            // Initialize the dialog for the NoLocation Exception
            this.NoLocationAlert = new AlertDialog.Builder(c);
            this.NoLocationAlert.SetTitle("Geen locatie beschikbaar");
            this.NoLocationAlert.SetMessage("Momenteel kunnen wij uw locatie niet vaststellen. Mogelijk heeft u uw locatieservices uitstaan of duurt het nog even voordat we uw locatie kunnen ontvangen.");
            this.NoLocationAlert.SetNeutralButton("Oké", (senderAlert, arg) => { });

            // Initialize the dialog for the NotOnMap Exception
            this.NotOnMapAlert = new AlertDialog.Builder(c);
            this.NotOnMapAlert.SetTitle("Buiten Utrecht");
            this.NotOnMapAlert.SetMessage("U bevindt zich momenteel buiten het bereik wat deze app aankan. Ga naar Utrecht en omgeving om deze app te gebruiken en uw positie te centeren op de kaart.");
            this.NotOnMapAlert.SetNeutralButton("Oké", (senderAlert, arg) => { });

            this.Tracker = a.Tracker;
        }

        private void CenterMapToCurrentLocation(object sender, EventArgs e)
        {
            try
            {
                this.Map.CenterMapToCurrentLocation();
            }
            catch (NoLocationException)
            {
                Dialog Dialog = this.NoLocationAlert.Create();
                Dialog.Show();
            }
            catch (NotOnMapException)
            {
                Dialog Dialog = this.NotOnMapAlert.Create();
                Dialog.Show();
            }
        }

        private void UpdateStartStopPauseButton()
        {
            if (this.Tracker.IsTracking())
            {
                this.btnStartStop.Text = "Stop";
                this.btnPause.Enabled = true;
                if (this.Tracker.IsPaused())
                {
                    this.btnPause.Text = "Doorgaan";
                } else
                {
                    this.btnPause.Text = "Pauze";
                }
            }
            else
            {
                this.btnStartStop.Text = "Start";
                this.btnPause.Text = "Pauze";
                this.btnPause.Enabled = false;
            }
        }

        private void StartStopTracking(object sender, EventArgs e)
        {
            if (!this.Tracker.IsTracking())
            {
                this.StartTracking();
            }
            else
            {
                this.Tracker.StopTracking();
            }
            this.Status.Invalidate();
            this.UpdateStartStopPauseButton();
        }

        private void StartTracking()
        {
            try
            {
                this.Map.CheckCurrentLocation();
            }
            catch (NoLocationException)
            {
                Dialog Dialog = this.NoLocationAlert.Create();
                Dialog.Show();
            }
            catch (NotOnMapException)
            {
                Dialog Dialog = this.NotOnMapAlert.Create();
                Dialog.Show();
            }

            this.Tracker.StartNewTrack();
            this.Tracker.StartTracking();
        }

        private void PauseTracking(object sender, EventArgs ea)
        {
            if (this.Tracker.IsPaused())
            {
                try
                {
                    this.Map.CheckCurrentLocation();
                }
                catch (NoLocationException)
                {
                    Dialog Dialog = this.NoLocationAlert.Create();
                    Dialog.Show();
                }
                catch (NotOnMapException)
                {
                    Dialog Dialog = this.NotOnMapAlert.Create();
                    Dialog.Show();
                }

                this.Tracker.StartTracking();
            }
            else
            {
                this.Tracker.PauseTracking();
            }
            this.UpdateStartStopPauseButton();
            this.Status.Invalidate();
        }
    }
}