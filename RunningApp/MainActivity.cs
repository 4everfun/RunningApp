using System;

using Android.App;
using Android.Widget;
using Android.OS;

using Android.Support.V4.Widget;
using Android.Views;
using RunningApp.Fragments;

using RunningApp.Database;
using RunningApp.Parcels;
using SQLite;
using Mono.Data.Sqlite;
using RunningApp.Tracker;

namespace RunningApp
{
    // Remove the ActionBar
    [Activity(Label = "RunningApp", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.DesignDemo")]
    public class MainActivity : Activity
    {
        public Tracker.Tracker Tracker { get; protected set; }

        public const int TRACKER = 0;
        public const int MYTRACKS = 1;
        public const int ANALYSE = 2;
        
        protected DrawerLayout drawerLayout;
        protected ListView drawerList;
        protected FrameLayout content;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

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

        public void SwitchView(int item)
        {
            Fragment f;
            Bundle b = new Bundle();
            switch (item)
            {
                case MainActivity.TRACKER:
                    f = new TrackerFragment();
                    b.PutParcelable("Tracker", new TrackerParcel(this.Tracker));
                    f.Arguments = b;
                    break;
                case MainActivity.MYTRACKS:
                    f = new MyTracksFragment();
                    b.PutParcelable("Tracks", new MyTracksParcel(TrackModel.GetAll(Database.Database.GetInstance())));
                    f.Arguments = b;
                    break;
                default:
                    return;
            }
            FragmentTransaction ft = this.FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.content, f);
            ft.Commit();
        }

        public void LoadAnalyse(Track Track)
        {
            Fragment f = new AnalyseFragment();
            Bundle b = new Bundle();
            b.PutParcelable("Track", new TrackParcel(Track));
            f.Arguments = b;

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
    }
}

