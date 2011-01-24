using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace LiveBiblePresentation.Songs
{
    public class Songs : ObservableCollection<Song>
    {
        public Songs()
            : base()
        { }
    }
}
