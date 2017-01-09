using System;

using Android.App;
using Android.Widget;
using Android.OS;

using RunningApp.Views;
using RunningApp.Tracker;
using RunningApp.Exceptions;

namespace RunningApp
{
    // Remove the ActionBar
    [Activity(Label = "RunningApp", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Material.NoActionBar")]
    public class MainActivity : Activity
    {
        protected MapView Map;
        protected AlertDialog.Builder NoLocationAlert, NotOnMapAlert;
        protected Tracker.Tracker Tracker;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set the content view to the main XML file 
            this.SetContentView(Resource.Layout.Main);

            // Bind the MapView to a variable, so it can be used later in the activity
            this.Map = FindViewById<MapView>(Resource.Id.mapView);

            this.Tracker = new Tracker.Tracker(this);

            this.Map.SetTracker(this.Tracker);

            // Add the click event to the center button
            FindViewById<Button>(Resource.Id.centerButton).Click += this.CenterMapToCurrentLocation;

            // Add the click event to the start and stop button
            FindViewById<Button>(Resource.Id.startButton).Click += this.StartTracking;
            FindViewById<Button>(Resource.Id.stopButton).Click += this.StopTracking;

            // Initialize the dialog for the NoLocation Exception
            this.NoLocationAlert = new AlertDialog.Builder(this);
            this.NoLocationAlert.SetTitle("Geen locatie beschikbaar");
            this.NoLocationAlert.SetMessage("Momenteel kunnen wij uw locatie niet vaststellen. Mogelijk heeft u uw locatieservices uitstaan of duurt het nog even voordat we uw locatie kunnen ontvangen.");
            this.NoLocationAlert.SetNeutralButton("Oké", (senderAlert, arg) => { });

            // Initialize the dialog for the NotOnMap Exception
            this.NotOnMapAlert = new AlertDialog.Builder(this);
            this.NotOnMapAlert.SetTitle("Buiten Utrecht");
            this.NotOnMapAlert.SetMessage("U bevindt zich momenteel buiten het bereik wat deze app aankan. Ga naar Utrecht en omgeving om deze app te gebruiken en uw positie te centeren op de kaart.");
            this.NotOnMapAlert.SetNeutralButton("Oké", (senderAlert, arg) => { });
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

        private void StartTracking(object sender, EventArgs e)
        {
            try
            {
                this.Map.CheckCurrentLocation();
                this.Tracker.StartTracking();
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

        private void StopTracking(object sender, EventArgs e)
        {
            this.Tracker.StopTracking();
        }
    }
}

