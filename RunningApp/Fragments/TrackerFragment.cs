using System;

using Java.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

using RunningApp.Views;
using RunningApp.Exceptions;
using RunningApp.Dialogs;
using Android.Graphics;
using RunningApp.Database;
using RunningApp.Parcels;
using System.IO;
using Android.Support.V4.Content;

namespace RunningApp.Fragments
{
    public class TrackerFragment : BaseFragment
    {
        protected MapView Map;
        protected Status Status;
        protected AlertDialog.Builder NoLocationAlert, NotOnMapAlert;
        protected Button btnStartStop, btnPause;
        protected Tracker.Tracker Tracker;

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

            Bundle b = this.Arguments;
            TrackerParcel tp = (TrackerParcel)b.GetParcelable("Tracker");
            this.Tracker = tp.Tracker;
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
                this.StartNewTrack();
            }
            else
            {
                FragmentTransaction ft = FragmentManager.BeginTransaction();
                Fragment prev = FragmentManager.FindFragmentByTag("StopTrackingDialog");
                if (prev != null) ft.Remove(prev);
                ft.AddToBackStack(null);

                // Create and show the dialog.
                StopTrackingDialog dialogBox = StopTrackingDialog.NewInstance(null);
                dialogBox.Show(ft, "StopTrackingDialog");
                dialogBox.Cancelable = false;

                this.Tracker.PauseTracking();

                dialogBox.OnSaveClick += this.SaveTrack;

                dialogBox.OnDeleteClick += (s, ea) =>
                {
                    this.Tracker.StartNewTrack();
                    this.UpdateStartStopPauseButton();

                    Toast.MakeText(this.Context, "De track is verwijderd.", ToastLength.Short).Show();
                    s.Dismiss();
                };

                // Make screenshot, store it and share it when the 'Share' button is pressed.
                dialogBox.OnShareClick += (s, ea) => 
                {
                    ShareImage(StoreScreenShot(TakeScreenShot(this.Map)), (Activity) this.Context, "RunningApp", "Ik heb " + this.Tracker.track.GetTotalRunningDistance().ToString() + " meter gelopen in een tijd van " + this.Tracker.track.GetTotalRunningTimeSpan().ToString() + " met een gemiddelde van " + this.Tracker.track.GetAvergageSpeed() + "km/h");
                };

                dialogBox.OnClose += (s, ea) => {
                    this.Tracker.StartTracking();
                };
            }
        }

        private void StartNewTrack()
        {
            try
            {
                this.Map.CheckCurrentLocation();
            }
            catch (NoLocationException)
            {
                Dialog Dialog = this.NoLocationAlert.Create();
                Dialog.Show();
                return;
            }
            catch (NotOnMapException)
            {
                Dialog Dialog = this.NotOnMapAlert.Create();
                Dialog.Show();
                return;
            }
            this.Tracker.StartNewTrack();
            this.Tracker.StartTracking();
            this.UpdateStartStopPauseButton();
            this.Status.Invalidate();
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

        private void SaveTrack(StopTrackingDialog s, EventArgs ea)
        {
            FragmentTransaction ft = FragmentManager.BeginTransaction();
            Fragment prev = FragmentManager.FindFragmentByTag("TrackNameDialog");
            if (prev != null) ft.Remove(prev);
            ft.AddToBackStack(null);

            TrackNameDialog dialogBox = TrackNameDialog.NewInstance(null);
            dialogBox.Show(ft, "TrackNameDialog");
            dialogBox.TrackNameDialogSave += (se, e) => {
                TrackModel tm = this.Tracker.GetOrInstantiateTrackModel();
                tm.Name = e.TrackName;
                System.Console.WriteLine("____FAKETRACKGENERATIOSTRING____");
                System.Console.WriteLine(tm.Track);
                Database.Database.GetInstance().InsertOrReplace(tm);
                this.Tracker.StopTracking();
                this.UpdateStartStopPauseButton();

                Toast.MakeText(this.Context, "Uw track is opgeslagen!", ToastLength.Short).Show();
                se.Dismiss();
                s.Dismiss();
            };
        }

        //Take a screenshot of the entire screen.       
        public static Bitmap TakeScreenShot(View view)
        {
            View screenView = view.RootView;
            screenView.DrawingCacheEnabled = true;
            Bitmap bitmap = Bitmap.CreateBitmap(screenView.DrawingCache);
            screenView.DrawingCacheEnabled = false;
            return bitmap;
        }

        //Save the screenshot in /storage/emulated/0/RunningApp with the jpeg extension.
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

        //Share screenshot with an intent.
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
            catch (ActivityNotFoundException)
            {
                Toast.MakeText(activity.ApplicationContext, "No App Available", ToastLength.Long).Show();
            }
        }
    }
}