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
using RunningApp.Adapters;
using RunningApp.Database;
using RunningApp.Parcels;

namespace RunningApp.Fragments
{
    public class MyTracksFragment : BaseFragment
    {
        protected ListView List;

        public MyTracksFragment() : base()
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.MyTracks, container, false);

            Bundle b = this.Arguments;
            MyTracksParcel tp = (MyTracksParcel)b.GetParcelable("Tracks");

            this.List = view.FindViewById<ListView>(Resource.Id.MyTracksList);
            MyTracksAdapter MyTracksAdapter = new MyTracksAdapter(this.Context, Resource.Layout.MyTracksRow, tp.Tracks);
            this.List.Adapter = MyTracksAdapter;

            MainActivity a = (MainActivity)this.Context;

            MyTracksAdapter.OnOpenClick += (s, ea) =>
            {
                a.Tracker.SetTrack(ea.TrackModel);
                a.SwitchView(MainActivity.TRACKER);
            };

            MyTracksAdapter.OnAnalyseClick += (s, ea) =>
            {
                a.LoadAnalyse(ea.TrackModel.GetTrack());
            };

            MyTracksAdapter.OnDeleteClick += (s, ea) =>
            {
                new AlertDialog.Builder(this.Context)
                    .SetTitle("Track verwijderen")
                    .SetMessage("Weet u zeker dat u deze track wilt verwijderen?")
                    .SetPositiveButton("Ja", delegate
                    {
                        Database.Database.GetInstance().Delete(ea.TrackModel);
                        a.SwitchView(MainActivity.MYTRACKS);
                    })
                    .SetNeutralButton("Nee", delegate { }).Show();
            };

            return view;
        }
    }
}