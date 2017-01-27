using System;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;

using RunningApp.Dialogs;
using Android.Support.V4.Widget;
using Android.Views;
using RunningApp.Fragments;
using RunningApp.Tracker;

using SQLite;

namespace RunningApp
{
    // Remove the ActionBar
    [Activity(Label = "RunningApp", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.DesignDemo")]
    public class MainActivity : Activity
    {
        public Tracker.Tracker Tracker;

        protected const int TRACKER = 0;
        protected const int MYTRACKS = 1;
        protected const int ANALYSE = 2;
        
        protected DrawerLayout drawerLayout;
        protected ListView drawerList;
        protected FrameLayout content;

        public SQLiteConnection Database;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.Database = RunningApp.Database.Database.NewInstance();

            // Set the content view to the main XML file 
            this.SetContentView(Resource.Layout.Main);

            string[] ListItems = new string[2];
            ListItems[0] = "Tracker";
            ListItems[1] = "Mijn Tracks";

            this.drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            this.drawerList = FindViewById<ListView>(Resource.Id.nav_view);
            this.drawerList.Adapter = new ArrayAdapter<String>(this, Resource.Layout.drawer_list_item, ListItems);
            this.drawerList.ItemClick += this.OpenItem;

            this.Tracker = new Tracker.Tracker(this);

            this.SwitchView(MainActivity.TRACKER);
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

        private void SwitchView(int item)
        {
            Fragment f;
            switch (item)
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
                //Make screenshot, store it and share it when the 'Share' button is pressed.
                dialogBox.OnShareClick += delegate (StopTrackingDialog s, EventArgs ea)
                {
                    ShareImage(StoreScreenShot(TakeScreenShot(this.Map)), this, "RunningApp", "Ik heb " + this.Tracker.GetTotalDistance().ToString() + " meter gelopen in een tijd van " + this.Tracker.GetTimeSpanTracking().ToString() + " met een gemiddelde van " + this.Tracker.GetAvergageSpeed() + "km/h");
                    s.Dismiss();
                };

                this.Started = false;

                this.btnStartStop.Text = "Start";
                this.btnPause.Text = "Pauze";
                this.btnPause.Enabled = false;

                this.Tracker.StopTracking();

                case 0:
                    f = new TrackerFragment();
                    break;
                case 1:
                    f = new MyTracksFragment();
                    break;
                case 2:
                    f = new TrackerFragment();
                    break;
                default:
                    return;
            }
            FragmentTransaction ft = this.FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.content, f);
            ft.Commit();
        }

        private void OpenItem(object sender, AdapterView.ItemClickEventArgs ea)
        {
            switch (ea.Id)
            {
                case 0:
                    this.SwitchView(MainActivity.TRACKER);
                    break;
                case 1:
                    this.SwitchView(MainActivity.MYTRACKS);
                    break;
                default:
                    return;
            }
            this.drawerLayout.CloseDrawer(this.drawerList);
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
            catch (ActivityNotFoundException ex)
            {
                Toast.MakeText(activity.ApplicationContext, "No App Available", ToastLength.Long).Show();
            }
        }
    }
}

