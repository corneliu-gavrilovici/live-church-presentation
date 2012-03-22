using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LiveBiblePresentation.Data
{
    public class BibleVerses : ObservableCollection<BibleVerse>
    {
        public BibleVerses()
            : this(new List<BibleVerse>())
        {
        }

        public BibleVerses(List<BibleVerse> list) 
		{
            list.ForEach(delegate(BibleVerse verse)
			{
                this.Add(verse);
			});
		}
    }
}