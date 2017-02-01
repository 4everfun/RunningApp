using System;

using RunningApp.Database;

namespace RunningApp.Adapters
{
    class MyTracksEventArgs : EventArgs
    {
        public TrackModel TrackModel { get; set; }

        public MyTracksEventArgs(TrackModel TrackModel)
        {
            this.TrackModel = TrackModel;
        }
    }
}