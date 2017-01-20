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
            string path = System.IO.Path.Combine(docsFolder, "kleuren.db");
            Database.Connection = new SQLiteConnection(pad);
            if (!File.Exists(path))
            {
                Database.Connection.CreateTable<KleurItem>();
                foreach (KleurItem k in defaultKleuren)
                    90 database.Insert(k);
            }
        }
    }
}