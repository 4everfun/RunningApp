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
                Database.Fill(Connection);
            }
            return Database.Connection;
        }

        public static void Fill(SQLiteConnection c)
        {
            // TODO: Fill the database with a fake track
        }
    }
}