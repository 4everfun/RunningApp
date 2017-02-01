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
using Android.Locations;
using Newtonsoft.Json;

namespace RunningApp.Tracker
{
    public class SerializableLocation
    {
        [JsonProperty]
        public double Latitude { get; set; }
        [JsonProperty]
        public double Longitude { get; set; }
        [JsonProperty]
        public virtual long Time { get; set; }

        [JsonIgnore]
        public Location Location { get; private set; }

        [JsonConstructor]
        private SerializableLocation(double Latitude, double Longitude, long Time) {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Time = Time;

            this.Location = new Location("JSON");
            this.Location.Latitude = this.Latitude;
            this.Location.Longitude = this.Longitude;
            this.Location.Time = this.Time;
        }

        public SerializableLocation(Location l)
        {
            this.Latitude = l.Latitude;
            this.Longitude = l.Longitude;
            this.Time = l.Time;
            this.Location = l;
        }
    }
}