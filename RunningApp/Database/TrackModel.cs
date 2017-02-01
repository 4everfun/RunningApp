using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using SQLite;

using RunningApp.Tracker;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;

namespace RunningApp.Database
{
    public class TrackModel
    {
        [PrimaryKey, AutoIncrement]
        public int? ID { get; set; }
        public string Name { get; set; }
        public string Track { get; private set; }

        public TrackModel() { }

        public TrackModel(Track Track)
        {
            this.SetTrack(Track);
        }

        public TrackModel(string Name, Track Track)
        {
            this.Name = Name;
            this.SetTrack(Track);
        }

        public static List<TrackModel> GetAll(SQLiteConnection c)
        {
            List<TrackModel> collection = new List<TrackModel>();
            TableQuery<TrackModel> q = c.Table<TrackModel>();
            foreach (TrackModel tm in q)
            {
                collection.Add(tm);
            }
            return collection;
        }

        public void SetTrack(Track Track)
        {
            this.Track = JsonConvert.SerializeObject(Track);
        }

        public Track GetTrack()
        {
            if (this.Track == null) return new Track();
            return JsonConvert.DeserializeObject<Track>(this.Track);
        }

        public DateTime GetDateTime()
        {
            return this.GetTrack().GetFirstDateTime();
        }

        public Bitmap CreateMapImage(View view)
        {
            //Define a bitmap with the same size as the view
            Bitmap returnedBitmap = Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888);
            //Bind a canvas to it
            Canvas canvas = new Canvas(returnedBitmap);
            //Get the view's background
            Drawable bgDrawable = view.Background;
            if (bgDrawable != null)
                //has background drawable, then draw it on the canvas
                bgDrawable.Draw(canvas);
            else
                //does not have background drawable, then draw white background on the canvas
                canvas.DrawColor(Color.White);
            // draw the view on the canvas
            view.Draw(canvas);
            //return the bitmap
            return returnedBitmap;
        }
    }
}