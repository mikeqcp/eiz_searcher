using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.Windows;

namespace Search_engine
{
    class SearchViewModel : PropertyChangedBase
    {
        private DataSource _ds = new DataSource();

        public void LoadDocs()
        {
            _ds.LoadDocuments();
        }

        public void LoadWords()
        {
            _ds.LoadKeywords();
        }

        public void Quit()
        {
            Application.Current.Shutdown();
        }
    }
}
