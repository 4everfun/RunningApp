using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using SQLite;

using RunningApp.Tracker;

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
    }
}