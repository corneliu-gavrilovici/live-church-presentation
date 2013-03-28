using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LiveBiblePresentation.Data
{
    public class BibleVerses : ObservableCollection<BibleVerse>
    {
        public BibleVerses(List<BibleVerse> list)
            : base(list)
        {
        }

        public BibleVerses(IEnumerable<BibleVerse> collection)
            : base(collection)
        {
        }
    }
}