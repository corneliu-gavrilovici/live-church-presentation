using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

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

        /// <summary> 
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        /// </summary> 
        public void AddRange(IEnumerable<BibleVerse> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            foreach (var i in collection) Items.Add(i);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection.ToList()));
        }
    }
}