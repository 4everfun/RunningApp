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

        public static SQLiteConnection NewInstance()
        {
            string docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string path = Path.Combine(docsFolder, "RunningApp.db");
            Database.Connection = new SQLiteConnection(path);
            if (!File.Exists(path))
            {
                Database.Connection.CreateTable<TrackModel>();
            }
            return Database.Connection;
        }
    }
}