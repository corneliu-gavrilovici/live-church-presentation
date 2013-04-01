namespace LiveBiblePresentation.Data
{
    public class AttributeNames
    {
        public string Book { get; set; }
        public string Chapter { get; set; }
        public string Verse { get; set; }

        public static AttributeNames GetBibleAttributeNames(BibleLanguage bibleLanguage)
        {
            AttributeNames attrs = new AttributeNames();

            switch (bibleLanguage)
            {
                case BibleLanguage.FrenchLouisSegond:
                case BibleLanguage.GermanLutherBible:
                    attrs.Book = "bname";
                    attrs.Chapter = "cnumber";
                    attrs.Verse = "vnumber";
                    break;
                default:
                    attrs.Book = "n";
                    attrs.Chapter = "n";
                    attrs.Verse = "n";
                    break;
            }

            return attrs;
        }
    }
}