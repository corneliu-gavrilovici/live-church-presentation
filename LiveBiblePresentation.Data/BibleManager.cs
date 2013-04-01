using LiveBiblePresentation.Data.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Xml;

namespace LiveBiblePresentation.Data
{
    public enum BibleLanguage
    { 
        RomanianCornilescu,
        EnglishKingJames,
        GermanLutherBible,
        FrenchLouisSegond
    }

    public class BibleManager
    {
        #region Private Members

        private readonly BibleVerses bible = null;
        private readonly BibleVerses bible2 = null;
        private string connectionString = string.Empty;

        #endregion

        #region Constructors

        public BibleManager(BibleLanguage bibleLanguage, BibleLanguage? bibleLanguage2 = null)
        {
            bible = GetBible(bibleLanguage);
            if (bibleLanguage2.HasValue) bible2 = GetBible(bibleLanguage2.Value);
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
            BibleVerses verses = new BibleVerses(new List<BibleVerse>());

            verses.AddRange(from b in bible
                            where b.ID >= id && b.ID < maxId && bible.Count != noOfVerses
                            select b);

            if (bible2 != null)
            {
                verses.AddRange(from b in bible2
                                where b.ID >= id && b.ID < maxId && bible2.Count != noOfVerses
                                select b);
            }

            return verses;
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

            OleDbConnection conn = null; XmlDocument xmlDoc = null;
            try
            {
                if (bibleLanguage == BibleLanguage.RomanianCornilescu)
                {
                    conn = new OleDbConnection(ConnectionString);
                    OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT id, carte, capitol, verset, text FROM bibliaRoCornilescu ORDER BY id", conn);
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
                else
                {
                    string bibleFilePath = Globals.GetBibleFilePath(bibleLanguage);
                    if (File.Exists(bibleFilePath))
                    {
                        xmlDoc = new XmlDocument();
                        // load the xml bible file
                        xmlDoc.Load(bibleFilePath);
                        int index = 0, chapterNumber = 0, verseNumber = 0; string bookName = string.Empty;
                        AttributeNames attrs = AttributeNames.GetBibleAttributeNames(bibleLanguage);
                        // get all bible books
                        XmlNodeList bookNodes = xmlDoc.SelectNodes("//b");
                        foreach (XmlNode bookNode in bookNodes)
                        {
                            bookName = bookNode.AttributeInnerText(attrs.Book);
                            foreach (XmlNode chapterNode in bookNode.ChildNodes)
                            {
                                // get current chapter number
                                chapterNode.TryParseAttributeToInteger(attrs.Chapter, out chapterNumber);
                                foreach (XmlNode verseNode in chapterNode.ChildNodes)
                                {
                                    // get current verse number
                                    verseNode.TryParseAttributeToInteger(attrs.Verse, out verseNumber);

                                    BibleVerse bibleVerse = new BibleVerse();
                                    index++;
                                    bibleVerse.ID = index;
                                    bibleVerse.Carte = bookName;
                                    bibleVerse.Capitol = chapterNumber;
                                    bibleVerse.Verset = verseNumber;
                                    bibleVerse.Text = verseNode.InnerText;
                                    bibleVerses.Add(bibleVerse);
                                }
                            }
                        }
                    }
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
                    string dbPath = Path.Combine(Globals.AppPath, Resources.DbName);

                    connectionString =  Resources.ConnectionProvider + dbPath + Resources.UserCredentials;
                }

                return connectionString;
            }
        }

        #endregion
    }
}