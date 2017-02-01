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
using Java.Lang;
using RunningApp.Database;

namespace RunningApp.Parcels.Creators
{
    public class MyTracksParcelCreator : Java.Lang.Object, IParcelableCreator
    {
        public Java.Lang.Object CreateFromParcel(Parcel source)
        {
            List<TrackModel> Tracks = new List<TrackModel>();
            source.ReadList(Tracks, Java.Lang.Class.FromType(typeof(List<TrackModel>)).ClassLoader);
            return new MyTracksParcel(Tracks);
        }

        public Java.Lang.Object[] NewArray(int size)
        {
            return new Java.Lang.Object[size];
        }
    }
}