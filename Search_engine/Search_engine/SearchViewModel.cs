using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;

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

        public void Search(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (!_ds.IsReady)
                    MessageBox.Show("You need to load documents and keywords first!");

                Console.Write("s");
            }
        }

        public void Quit()
        {
            Application.Current.Shutdown();
        }
    }
}
