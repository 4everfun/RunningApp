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

using System.IO;
using SQLite;

namespace RunningApp.Database
{
    class Database
    {
        public static SQLiteConnection Connection;

        public static SQLiteConnection GetInstance()
        {
            if (Database.Connection != null) return Database.Connection;
            string docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string path = Path.Combine(docsFolder, "RunningApp.db");
            bool newDatabase = !File.Exists(path);
            Database.Connection = new SQLiteConnection(path);
            if (newDatabase)
            {
                Database.Connection.CreateTable<TrackModel>();
                TrackModel tm = new TrackModel
                {
                    Name = "Fake Track"
                };
                tm.SetRawTrack("{\"segments\":[{\"points\":[{\"Latitude\":52.083325,\"Longitude\":5.175968,\"Time\":1485987176219},{\"Latitude\":52.083315,\"Longitude\":5.176171,\"Time\":1485987179960},{\"Latitude\":52.083406,\"Longitude\":5.176198,\"Time\":1485987182503},{\"Latitude\":52.083435,\"Longitude\":5.176439,\"Time\":1485987183915},{\"Latitude\":52.083838,\"Longitude\":5.176411,\"Time\":1485987188828},{\"Latitude\":52.084207,\"Longitude\":5.176387,\"Time\":1485987190651},{\"Latitude\":52.084623,\"Longitude\":5.176489,\"Time\":1485987192964},{\"Latitude\":52.085088,\"Longitude\":5.176397,\"Time\":1485987195214},{\"Latitude\":52.085597,\"Longitude\":5.176381,\"Time\":1485987200994},{\"Latitude\":52.085953,\"Longitude\":5.176344,\"Time\":1485987203398},{\"Latitude\":52.086223,\"Longitude\":5.176092,\"Time\":1485987206769},{\"Latitude\":52.086285,\"Longitude\":5.175899,\"Time\":1485987210428},{\"Latitude\":52.086256,\"Longitude\":5.175701,\"Time\":1485987212613},{\"Latitude\":52.086239,\"Longitude\":5.175524,\"Time\":1485987214428},{\"Latitude\":52.086322,\"Longitude\":5.175299,\"Time\":1485987217186}]}, {\"points\":[{\"Latitude\":52.086305,\"Longitude\":5.16413,\"Time\": 1485987309278},{\"Latitude\":52.085844,\"Longitude\":5.163916,\"Time\":1485987312216},{\"Latitude\":52.085761,\"Longitude\":5.163836,\"Time\":1485987314955},{\"Latitude\":52.085613,\"Longitude\":5.163638,\"Time\":1485987317526},{\"Latitude\":52.085451,\"Longitude\":5.163606,\"Time\":1485987320367},{\"Latitude\":52.085319,\"Longitude\":5.163627,\"Time\":1485987323252},{\"Latitude\":52.085224,\"Longitude\":5.163766,\"Time\":1485987325330},{\"Latitude\":52.085065,\"Longitude\":5.163718,\"Time\":1485987327491},{\"Latitude\":52.084976,\"Longitude\":5.163766,\"Time\":1485987331304},{\"Latitude\":52.084795,\"Longitude\":5.163691,\"Time\":1485987333364},{\"Latitude\":52.08468,\"Longitude\":5.163717,\"Time\":1485987335316},{\"Latitude\":52.08461,\"Longitude\":5.163701,\"Time\":1485987343289},{\"Latitude\":52.084528,\"Longitude\":5.163746,\"Time\":1485987344932},{\"Latitude\":52.084467,\"Longitude\":5.163796,\"Time\":1485987347292},{\"Latitude\":52.084402,\"Longitude\":5.163871,\"Time\":1485987350216},{\"Latitude\":52.084323,\"Longitude\":5.163935,\"Time\":1485987353328},{\"Latitude\":52.084309,\"Longitude\":5.164082,\"Time\":1485987355772},{\"Latitude\":52.084294,\"Longitude\":5.16425,\"Time\":1485987357805},{\"Latitude\":52.084285,\"Longitude\":5.164464,\"Time\":1485987360757},{\"Latitude\":52.084243,\"Longitude\":5.164592,\"Time\":1485987362513},{\"Latitude\":52.084193,\"Longitude\":5.164742,\"Time\":1485987364949},{\"Latitude\":52.084154,\"Longitude\":5.164886,\"Time\":1485987366803},{\"Latitude\":52.084149,\"Longitude\":5.165084,\"Time\":1485987369294},{\"Latitude\":52.084117,\"Longitude\":5.165244,\"Time\":1485987370807},{\"Latitude\":52.084112,\"Longitude\":5.165407,\"Time\":1485987373082},{\"Latitude\":52.084056,\"Longitude\":5.165551,\"Time\":1485987374820},{\"Latitude\":52.084048,\"Longitude\":5.165687,\"Time\":1485987377517},{\"Latitude\":52.084002,\"Longitude\":5.165917,\"Time\":1485987379003},{\"Latitude\":52.083976,\"Longitude\":5.166085,\"Time\":1485987381709},{\"Latitude\":52.083952,\"Longitude\":5.166235,\"Time\":1485987384412},{\"Latitude\":52.083938,\"Longitude\":5.166336,\"Time\":1485987387096},{\"Latitude\":52.083868,\"Longitude\":5.166521,\"Time\":1485987389977},{\"Latitude\":52.083784,\"Longitude\":5.166706,\"Time\":1485987393469},{\"Latitude\":52.083722,\"Longitude\":5.166832,\"Time\":1485987398186},{\"Latitude\":52.083712,\"Longitude\":5.166984,\"Time\":1485987400402},{\"Latitude\":52.083669,\"Longitude\":5.167142,\"Time\":1485987402824},{\"Latitude\":52.083649,\"Longitude\":5.167335,\"Time\":1485987405133},{\"Latitude\":52.083603,\"Longitude\":5.167482,\"Time\":1485987407221},{\"Latitude\":52.083586,\"Longitude\":5.16768,\"Time\":1485987409876},{\"Latitude\":52.083568,\"Longitude\":5.167907,\"Time\":1485987413448},{\"Latitude\":52.083575,\"Longitude\":5.168081,\"Time\":1485987416502},{\"Latitude\":52.083558,\"Longitude\":5.168303,\"Time\":1485987418819},{\"Latitude\":52.083527,\"Longitude\":5.16848,\"Time\":1485987421329},{\"Latitude\":52.083514,\"Longitude\":5.168686,\"Time\":1485987423032},{\"Latitude\":52.083438,\"Longitude\":5.168889,\"Time\":1485987425013},{\"Latitude\":52.083421,\"Longitude\":5.16921,\"Time\":1485987426788},{\"Latitude\":52.08339,\"Longitude\":5.169483,\"Time\":1485987428605},{\"Latitude\":52.083372,\"Longitude\":5.169724,\"Time\":1485987431485},{\"Latitude\":52.083354,\"Longitude\":5.169938,\"Time\":1485987433325},{\"Latitude\":52.083351,\"Longitude\":5.170125,\"Time\":1485987435323},{\"Latitude\":52.083367,\"Longitude\":5.170395,\"Time\":1485987437717},{\"Latitude\":52.083385,\"Longitude\":5.170639,\"Time\":1485987439463},{\"Latitude\":52.083382,\"Longitude\":5.170853,\"Time\":1485987441198},{\"Latitude\":52.083336,\"Longitude\":5.171054,\"Time\":1485987443944},{\"Latitude\":52.083398,\"Longitude\":5.171316,\"Time\":1485987446090},{\"Latitude\":52.083382,\"Longitude\":5.171522,\"Time\":1485987447864},{\"Latitude\":52.083356,\"Longitude\":5.171637,\"Time\":1485987450494},{\"Latitude\":52.083392,\"Longitude\":5.171822,\"Time\":1485987453100},{\"Latitude\":52.083385,\"Longitude\":5.172004,\"Time\":1485987455057},{\"Latitude\":52.083398,\"Longitude\":5.172159,\"Time\":1485987456940},{\"Latitude\":52.083375,\"Longitude\":5.172384,\"Time\":1485987459212},{\"Latitude\":52.083421,\"Longitude\":5.172574,\"Time\":1485987461084},{\"Latitude\":52.083423,\"Longitude\":5.17281,\"Time\":1485987464600},{\"Latitude\":52.083445,\"Longitude\":5.172995,\"Time\":1485987466696},{\"Latitude\":52.083458,\"Longitude\":5.173228,\"Time\":1485987469464},{\"Latitude\":52.083408,\"Longitude\":5.173421,\"Time\":1485987471499},{\"Latitude\":52.083441,\"Longitude\":5.173614,\"Time\":1485987473749},{\"Latitude\":52.083441,\"Longitude\":5.173847,\"Time\":1485987475841},{\"Latitude\":52.083446,\"Longitude\":5.174203,\"Time\":1485987479588},{\"Latitude\":52.083458,\"Longitude\":5.174396,\"Time\":1485987483223},{\"Latitude\":52.083464,\"Longitude\":5.174578,\"Time\":1485987485089},{\"Latitude\":52.083445,\"Longitude\":5.174827,\"Time\":1485987487069},{\"Latitude\":52.083431,\"Longitude\":5.175079,\"Time\":1485987489231},{\"Latitude\":52.083375,\"Longitude\":5.175371,\"Time\":1485987492821},{\"Latitude\":52.083375,\"Longitude\":5.175684,\"Time\":1485987495024},{\"Latitude\":52.083392,\"Longitude\":5.176046,\"Time\":1485987496916}]}]}");
                Database.Connection.Insert(tm);
            }
            return Database.Connection;
        }
    }
}