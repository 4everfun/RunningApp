using System;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;

using RunningApp.Views;
using RunningApp.Exceptions;
using RunningApp.Dialogs;
using Android.Views;
using Android.Graphics;
using System.IO;
using Android.Util;

namespace RunningApp
{
    // Remove the ActionBar
    [Activity(Label = "RunningApp", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Material.NoActionBar")]
    public class MainActivity : Activity
    {
        protected MapView Map;
        protected Status Status;
        protected AlertDialog.Builder NoLocationAlert, NotOnMapAlert;
        protected Tracker.Tracker Tracker;
        protected Button btnStartStop, btnPause; 

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set the content view to the main XML file 
            this.SetContentView(Resource.Layout.Main);

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

        protected bool Started = false;
        protected bool FirstStart = true;
        private void StartStopTracking(object sender, EventArgs e)
        {
            if (!this.Started)
            {
                if (!this.FirstStart)
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
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
                FragmentTransaction ft = FragmentManager.BeginTransaction();
                Fragment prev = FragmentManager.FindFragmentByTag("StopTrackingDialog");
                if (prev != null) ft.Remove(prev);
                ft.AddToBackStack(null);

                // Create and show the dialog.
                StopTrackingDialog dialogBox = StopTrackingDialog.NewInstance(null);
                dialogBox.Show(ft, "StopTrackingDialog");

                dialogBox.OnSaveClick += delegate (StopTrackingDialog s, EventArgs ea)
                {
                    Toast.MakeText(this, "Geklikt op opslaan!", ToastLength.Short).Show();
                    s.Dismiss();
                };
                dialogBox.OnDeleteClick += delegate (StopTrackingDialog s, EventArgs ea)
                {
                    Toast.MakeText(this, "Geklikt op verwijderen!", ToastLength.Short).Show();
                    s.Dismiss();
                };
                dialogBox.OnShareClick += delegate (StopTrackingDialog s, EventArgs ea)
                {
                    Toast.MakeText(this, "Geklikt op delen!", ToastLength.Short).Show();
                    ShareImage(StoreScreenShot(TakeScreenShot(this.Map)), this, "RunningApp", "Test bericht");
                    s.Dismiss();
                };

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
               
        public static Bitmap TakeScreenShot(View view)
        {
            View screenView = view.RootView;
            screenView.DrawingCacheEnabled = true;
            Bitmap bitmap = Bitmap.CreateBitmap(screenView.DrawingCache);
            screenView.DrawingCacheEnabled = false;
            return bitmap;
        }
        public static Java.IO.File StoreScreenShot(Bitmap picture)
        {
            var folder = Android.OS.Environment.ExternalStorageDirectory + Java.IO.File.Separator + "RunningApp";
            var extFileName = Android.OS.Environment.ExternalStorageDirectory +
            Java.IO.File.Separator +
            Guid.NewGuid() + ".jpeg";
            try
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                Java.IO.File file = new Java.IO.File(extFileName);

                using (var fs = new FileStream(extFileName, FileMode.OpenOrCreate))
                {
                    try
                    {
                        picture.Compress(Bitmap.CompressFormat.Jpeg, 100, fs);
                    }
                    finally
                    {
                        fs.Flush();
                        fs.Close();
                    }
                    return file;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error(LogPriority.Error.ToString(), "-------------------" + ex.Message.ToString());
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(LogPriority.Error.ToString(), "-------------------" + ex.Message.ToString());
                return null;
            }
        }
        public static void ShareImage(Java.IO.File file, Activity activity, string subject, string message)
        {
            Android.Net.Uri uri = Android.Net.Uri.FromFile(file);
            Intent i = new Intent(Intent.ActionSendMultiple);
            i.AddFlags(ActivityFlags.GrantReadUriPermission);
            i.PutExtra(Intent.ExtraSubject, subject);
            i.PutExtra(Intent.ExtraText, message);
            i.PutExtra(Intent.ExtraStream, uri);
            i.SetType("image/*");
            try
            {
                activity.StartActivity(Intent.CreateChooser(i, "Share Screenshot"));
            }
            catch (ActivityNotFoundException ex)
            {
                Toast.MakeText(activity.ApplicationContext, "No App Available", ToastLength.Long).Show();
            }
        }
    }
}

