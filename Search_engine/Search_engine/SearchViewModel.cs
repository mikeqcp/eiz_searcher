using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Search_engine
{
    class SearchViewModel : PropertyChangedBase
    {
        private DataSource _ds = new DataSource();
        private Ranker _ranker;
        public string SearchQuery { get; set; }
        private bool _keywordsLoaded = false;

        private bool _useStemming;
        public bool UseStemming
        {
            get 
            {
                return _useStemming;
            }
            set 
            { 
                _useStemming = value;
                if(_keywordsLoaded)
                    _ds.LoadKeywords(_useStemming);
                NotifyOfPropertyChange(() => UseStemming);
            }
        }
        public ObservableCollection<ResultItem> Results { get; set; }


        public void LoadDocs()
        {
            _ds.LoadDocuments();
        }

        public void LoadWords()
        {
            _ds.LoadKeywords(_useStemming);
        }

        public void Search(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && !string.IsNullOrEmpty(SearchQuery))
            {
                if (!_ds.IsReady)
                {
                    MessageBox.Show("You need to load documents and keywords first!");
                    return;
                }

                var query = new Query(SearchQuery, _useStemming);
                _ranker = new Ranker(_ds, query, _useStemming);

                Results = new ObservableCollection<ResultItem>(_ranker.RankOrder);
                NotifyOfPropertyChange(() => Results);
            }
        }

        public void Quit()
        {
            Application.Current.Shutdown();
        }
    }
}
