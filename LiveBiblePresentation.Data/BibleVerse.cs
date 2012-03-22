using System.ComponentModel;

namespace LiveBiblePresentation.Data
{
    public class BibleVerse : INotifyPropertyChanged
    {
        #region Constructors

        public BibleVerse()
        { }

        #endregion

        #region Public Properties

        public int ID
        {
            get 
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }

        public string Carte
        {
            get
            {
                return m_carte;
            }
            set
            {
                m_carte = value;
            }
        }

        public int Capitol
        {
            get
            {
                return m_capitol;
            }
            set
            {
                m_capitol = value;
            }
        }

        public int Verset
        {
            get
            {
                return m_verset;
            }
            set
            {
                m_verset = value;
            }
        }

        public string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Members

        private int m_id;
        private string m_carte = "Geneza";
        private int m_capitol = 1;
        private int m_verset = 1;
        private string m_text;

        #endregion
    }
}