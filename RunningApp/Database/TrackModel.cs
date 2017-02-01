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
        // Nullable int for AutoIncrement
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

        /// <summary>
        /// Only for use in the case of a fake track
        /// </summary>
        public void SetRawTrack (string Track)
        {
            this.Track = Track;
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
    }
}