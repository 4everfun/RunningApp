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

using Android.Graphics;

namespace RunningApp.Tracker
{
    class Track
    {
        protected List<PointF> track;

        public Track()
        {
            this.track = new List<PointF>();
        }

        public Track(List<PointF> track)
        {

        }
    }
}