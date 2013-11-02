using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using PorterStemmerAlgorithm;

namespace Search_engine
{
    public class DataSource
    {
        private enum RequiredFiles {Documents, Keywords};

        private string _keywordsFile;
        public List<Document> Documents { get; private set; }
        public List<string> Keywords { get; private set; }
        public bool IsReady
        {
            get
            {
                foreach (var v in _loaded)
                {
                    if (v == false)
                        return false;
                }
                return true;
            }
        }
        private bool[] _loaded;

        public DataSource()
        {
            Documents = new List<Document>();
            Keywords = new List<string>();
            _loaded = new bool[2];
        }

        public List<Document> LoadDocuments()
        {
            string filename = ChooseFile();
            string line;
            string content = string.Empty;
            string title = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    Documents.Clear();
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == string.Empty)
                        {
                            Documents.Add(new Document { Title = title, Content = content }) ;
                            title = string.Empty;
                            content = string.Empty;
                        } else
                        {
                            if (title == string.Empty)
                                title = line;
                            else
                                content += line;
                        }
                    }
                    Documents.Add(new Document { Title = title, Content = content });
                }
                _loaded[(int)RequiredFiles.Documents] = true;
                return Documents;
            }
            catch (Exception){}
            return Documents;
        }

        public List<string> LoadKeywords(bool useStemming = true)
        {
            if(_keywordsFile == null)
                _keywordsFile = ChooseFile();
            string line;
            try
            {
                using (StreamReader sr = new StreamReader(_keywordsFile))
                {
                    Keywords.Clear();
                    while ((line = sr.ReadLine()) != null)
                        Keywords.Add(line);
                }
                _loaded[(int)RequiredFiles.Keywords] = true;
                
            }
            catch (Exception) {}

            if(useStemming)
                Keywords = StemKeywords(Keywords);

            return Keywords.Distinct().ToList();
        }

        private List<string> StemKeywords(List<string> list)
        {
            var stemmed = new List<string>();
            StemmerInterface stemmer = new PorterStemmer();
            foreach (var t in list)
            {
                stemmed.Add(stemmer.stemTerm(t));
            }

            return stemmed;
        }

        private string ChooseFile()
        {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            dialog.ShowDialog();

            return dialog.FileName;
        }
    }
}
