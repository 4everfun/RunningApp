using System;
using System.Timers;

using Android.App;
using Android.Widget;
using Android.OS;

using RunningApp.Views;
using RunningApp.Tracker;
using RunningApp.Exceptions;

using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Views;

namespace RunningApp
{
    // Remove the ActionBar
    [Activity(Label = "RunningApp", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {
        protected MapView Map;
        protected Status Status;
        protected Android.App.AlertDialog.Builder NoLocationAlert, NotOnMapAlert;
        protected Tracker.Tracker Tracker;
        protected Button btnStartStop, btnPause; 
		protected DrawerLayout drawerLayout;
		protected NavigationView navigationView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set the content view to the main XML file 
            this.SetContentView(Resource.Layout.Main);

           var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar);
           SetSupportActionBar(toolbar);
           SupportActionBar.SetDisplayHomeAsUpEnabled(true);
           SupportActionBar.SetDisplayShowTitleEnabled(false);
           SupportActionBar.SetHomeButtonEnabled(true);
          // SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
           drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
           navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            // Bind the MapView to a variable, so it can be used later in the activity
            this.Map = FindViewById<MapView>(Resource.Id.mapView);
            this.Status = FindViewById<Status>(Resource.Id.statusView);

            this.Tracker = new Tracker.Tracker(this);

            this.Map.SetTracker(this.Tracker);
            this.Status.SetTracker(this.Tracker);

            // Add the click event to the center button
            FindViewById<ImageButton>(Resource.Id.centerButton).Click += this.CenterMapToCurrentLocation;

            // Add the click event to the start and stop button
            this.btnStartStop = FindViewById<Button>(Resource.Id.btnStartStop);
            this.btnStartStop.Click += this.StartStopTracking;
            this.btnPause = FindViewById<Button>(Resource.Id.btnPause);
            this.btnPause.Click += this.PauseTracking;

            // Initialize the dialog for the NoLocation Exception
            this.NoLocationAlert = new Android.App.AlertDialog.Builder(this);
            this.NoLocationAlert.SetTitle("Geen locatie beschikbaar");
            this.NoLocationAlert.SetMessage("Momenteel kunnen wij uw locatie niet vaststellen. Mogelijk heeft u uw locatieservices uitstaan of duurt het nog even voordat we uw locatie kunnen ontvangen.");
            this.NoLocationAlert.SetNeutralButton("Oké", (senderAlert, arg) => { });

            // Initialize the dialog for the NotOnMap Exception
            this.NotOnMapAlert = new Android.App.AlertDialog.Builder(this);
            this.NotOnMapAlert.SetTitle("Buiten Utrecht");
            this.NotOnMapAlert.SetMessage("U bevindt zich momenteel buiten het bereik wat deze app aankan. Ga naar Utrecht en omgeving om deze app te gebruiken en uw positie te centeren op de kaart.");
            this.NotOnMapAlert.SetNeutralButton("Oké", (senderAlert, arg) => { });
        }

       public override bool OnOptionsItemSelected(IMenuItem item)
       {
           switch (item.ItemId)
            {
               case Android.Resource.Id.Home:
                   drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
               return true;
            }
            return base.OnOptionsItemSelected(item);
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

        protected bool Started = false;
        protected bool FirstStart = true;
        private void StartStopTracking(object sender, EventArgs e)
        {
            if (!this.Started)
            {
                if (!this.FirstStart)
                {
                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    alert.SetTitle("Bevestiging verwijdering track");
                    alert.SetMessage("De track zal worden verwijderd als je een nieuwe run start. Weet je dit zeker?");
                    alert.SetPositiveButton("Ja", (senderAlert, args) => {
                        this.StartTracking();
                        Toast.MakeText(this, "De oude track is verwijderd", ToastLength.Short).Show();
                    });

                    alert.SetNegativeButton("Nee", (senderAlert, args) => {
                        Toast.MakeText(this, "Je track is bewaard gebleven", ToastLength.Short).Show();
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                } else
                {
                    this.StartTracking();
                }
            } else
            {
                this.Started = false;

                this.btnStartStop.Text = "Start";
                this.btnPause.Text = "Pauze";
                this.btnPause.Enabled = false;

                this.Tracker.StopTracking();
            }
            this.Status.Invalidate();
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

            this.Started = true;
            this.FirstStart = false;

            this.btnStartStop.Text = "Stop";
            this.btnPause.Enabled = true;
        }

        protected bool Paused = false;
        private void PauseTracking(object sender, EventArgs ea)
        {
            if (this.Paused)
            {
                this.btnPause.Text = "Pauze";
                this.Paused = false;

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
            } else
            {
                this.btnPause.Text = "Doorgaan";
                this.Paused = true;
                this.Tracker.PauseTracking();
            }
            this.Status.Invalidate();
        }
    }
}

