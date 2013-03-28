using LiveBiblePresentation.Data.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LiveBiblePresentation.Data
{
    public enum BibleLanguage
    { 
        RomanianCornilescu,
        EnglishKingJames,
        EnglishAmerStd
    }

    public class BibleManager
    {
        #region Constructors

        public BibleManager(BibleLanguage bibleLanguage)
        {
            bible = GetBible(bibleLanguage);
        }

        #endregion

        #region Public Properties

        public BibleVerses Bible
        {
            get { return bible; }
        }

        #endregion

        #region Public Methods

        public List<string> GetBibleBooks()
        {
            return (from b in Bible select b.Carte).Distinct().ToList();
        }

        public int GetBookChapters(string book)
        {
            return (from b in Bible
                    where b.Carte == book
                    select new { b.Capitol }).Max(c => c.Capitol);
        }

        public int GetNoOfVerses(string book, int chapter)
        {
            return (from b in Bible
                    where b.Carte == book && b.Capitol == chapter
                    select new { b.Verset }).Max(v => v.Verset);
        }

        public int GetID(string book, int chapter, int verset)
        {
            int nr = 0;
            try
            {
                nr = (from b in Bible
                        where b.Carte == book && b.Capitol == chapter && b.Verset == verset
                        select new { b.ID }).ToList()[0].ID;
            }
            catch
            { }
            return nr;
        }

        public BibleVerses GetVerses(int id, int noOfVerses)
        {
            int maxId = id + noOfVerses;

            return new BibleVerses(from b in Bible
                                   where b.ID >= id && b.ID < maxId && Bible.Count != noOfVerses
                                   select b);
        }

        public BibleVerses Search(string textToSearch)
        {
            return new BibleVerses(from b in Bible
                                   where b.Text.ToLower().Contains(textToSearch.ToLower().Trim())
                                   select b);
        }

        #endregion

        #region Private Methods

        private BibleVerses GetBible(BibleLanguage bibleLanguage)
        {
            BibleVerses bibleVerses = new BibleVerses(new List<BibleVerse>());
            OleDbConnection conn = null;
            try
            {
                conn = new OleDbConnection(ConnectionString);

                string table = string.Empty;

                switch (bibleLanguage)
                {
                    case BibleLanguage.RomanianCornilescu:
                        table = "bibliaRoCornilescu";
                        break;
                    case BibleLanguage.EnglishKingJames:
                        table = "bibliaEnKJ";
                        break;
                    case BibleLanguage.EnglishAmerStd:
                        table = "bibliaEnStd";
                        break;
                }

                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT id, carte, capitol, verset, text FROM " + table + " ORDER BY id", conn);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    BibleVerse bibleVerse = new BibleVerse();
                    bibleVerse.ID = Convert.ToInt32(row[0]);
                    bibleVerse.Carte = row[1].ToString();
                    bibleVerse.Capitol = Convert.ToInt32(row[2]);
                    bibleVerse.Verset = Convert.ToInt32(row[3]);
                    bibleVerse.Text = row[4].ToString();
                    bibleVerses.Add(bibleVerse);
                }
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return bibleVerses;
        }

        #endregion

        #region Private Properties

        private string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    string dbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Resources.DbName);

                    connectionString =  Resources.ConnectionProvider + dbPath + Resources.UserCredentials;
                }

                return connectionString;
            }
        }

        #endregion

        #region Private Members

        private readonly BibleVerses bible = null;
        private string connectionString = string.Empty;

        #endregion
    }
}