using SQLite;

using Newtonsoft.Json;

using RunningApp.Tracker;

namespace RunningApp.Database
{
    class TrackModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        private string Track { get; set; }

        public TrackModel(Track Track)
        {
            this.SetTrack(Track);
        }

        public void SetTrack(Track Track)
        {
            this.Track = JsonConvert.SerializeObject(Track);
        }

        public Track GetTrack()
        {
            return JsonConvert.DeserializeObject<Track>(this.Track);
        }
    }
}