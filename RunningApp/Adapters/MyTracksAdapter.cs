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

using RunningApp.Database;

namespace RunningApp.Adapters
{
    class MyTracksAdapter : ArrayAdapter<TrackModel>
    {
        public delegate void EventHandler(object sender, MyTracksEventArgs e);

        public event EventHandler OnOpenClick;
        public event EventHandler OnAnalyseClick;
        public event EventHandler OnShareClick;
        public event EventHandler OnDeleteClick;

        public MyTracksAdapter(Context context, int resource, List<TrackModel> objects) : base(context, resource, objects) { }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            if (convertView == null)
            {
                LayoutInflater vi = LayoutInflater.From(parent.Context);
                view = vi.Inflate(Resource.Layout.MyTracksRow, parent, false);
            }

            TrackModel track = this.GetItem(position);

            if (track != null)
            {
                TextView name = view.FindViewById<TextView>(Resource.Id.TrackName);
                TextView dateTime = view.FindViewById<TextView>(Resource.Id.TrackDateTime);
                Button open = view.FindViewById<Button>(Resource.Id.btnOpen);
                Button analyse = view.FindViewById<Button>(Resource.Id.btnAnalyse);
                Button share = view.FindViewById<Button>(Resource.Id.btnShare);
                Button delete = view.FindViewById<Button>(Resource.Id.btnDelete);

                name.Text = track.Name;
                dateTime.Text = track.GetDateTime().ToString("dd-MM-yyyy HH:mm");

                MyTracksEventArgs e = new MyTracksEventArgs(track);

                open.Click += (sender, ea) => this.OnOpenClick(sender, e);
                analyse.Click += (sender, ea) => this.OnAnalyseClick(sender, e);
                share.Click += (sender, ea) => this.OnShareClick(sender, e);
                delete.Click += (sender, ea) => this.OnDeleteClick(sender, e);
            }

            return view;
        }
    }
}