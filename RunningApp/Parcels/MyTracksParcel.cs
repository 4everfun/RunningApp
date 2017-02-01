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

using RunningApp.Tracker;
using RunningApp.Parcels.Creators;
using Java.Interop;
using RunningApp.Database;

namespace RunningApp.Parcels
{
    public class MyTracksParcel : Java.Lang.Object, IParcelable
    {
        public List<TrackModel> Tracks { get; set; }

        [ExportField("CREATOR")]
        public static MyTracksParcelCreator InitializeCreator()
        {
            return new MyTracksParcelCreator();
        }

        public MyTracksParcel(List<TrackModel> Tracks)
        {
            this.Tracks = Tracks;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteList(this.Tracks);
        }

        public int DescribeContents()
        {
            return 0;
        }
    }
}