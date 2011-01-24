using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Windows;
using System.Linq;
using System.IO;
using System.Windows.Forms;

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
            m_bible = GetBible(bibleLanguage);
        }

        #endregion

        #region Public Properties

        public BibleVerses Bible
        {
            get
            {
                return m_bible;
            }
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
            return new BibleVerses((from b in Bible
                    where b.ID >= id && b.ID < maxId && Bible.Count != noOfVerses
                    select b).ToList());
        }

        public BibleVerses Search(string textToSearch)
        {
            return new BibleVerses((from b in Bible
                                    where b.Text.ToLower().Contains(textToSearch.ToLower().Trim())
                                    select b).ToList());
        }

        #endregion

        #region Private Methods

        private BibleVerses GetBible(BibleLanguage bibleLanguage)
        {
            BibleVerses bibleVerses = new BibleVerses();
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                string dbPath = Application.StartupPath + Path.DirectorySeparatorChar + Resources.DbName;
                string connectionString = Resources.ConnectionProvider + dbPath + Resources.UserCredentials;
                return connectionString;
            }
        }

        #endregion

        #region Private Members

        private BibleVerses m_bible = null;

        #endregion
    }
}

