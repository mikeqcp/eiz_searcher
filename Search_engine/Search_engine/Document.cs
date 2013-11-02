using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PorterStemmerAlgorithm;

namespace Search_engine
{
    public class Document
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string RawContent
        {
            get
            {
                string rawContent = "";
                var source = _bagOdWordsStemmed != null ? _bagOdWordsStemmed : BagOfWords;
                foreach (var s in source)
                {
                    rawContent += s + " ";
                }
                return rawContent;
            }
        }
        public List<string> BagOfWords
        {
            get
            {
                return GetWords();
            }
        }
        private List<string> _bagOdWordsStemmed;
        public List<string> BagOfWordsStemmed
        {
            get
            {
                if(_bagOdWordsStemmed == null)
                    _bagOdWordsStemmed = GetStemmedWords();
                return _bagOdWordsStemmed;
            }
        }

        private string ContentAll
        {
            get
            {
                return Title + " " + Content;
            }
        }

        private List<string> GetWords()
        {
            Regex rgx = new Regex(@"[^a-zA-Z]");
            var cleared = rgx.Replace(ContentAll, " ");
            var splitted = cleared.Split(' ', '.', ',', ':', ';');
            var cleanList = new List<string>();
            foreach (var w in splitted)
            {
                var clean = w.Trim();
                if(!string.IsNullOrEmpty(clean))
                    cleanList.Add(clean.ToLower());
            }
            return cleanList;
        }

        private List<string> GetStemmedWords()
        {
            StemmerInterface stemmer = new PorterStemmer();
            var words = GetWords();
            for (int i = 0; i < words.Count; i++)
            {
                words[i] = stemmer.stemTerm(words[i]);
            }
            return words;
        }

        private bool Contains(string searched, bool useStemming = true)
        {
            var source = useStemming ? BagOfWordsStemmed : BagOfWords;
            foreach (var w in source)
            {
                if (w.Equals(searched))
                    return true;
            }
            return false;
        }

        public int CountOccurrences(string searched, bool useStemming = true)
        {
            int count = 0;
            var source = useStemming ? BagOfWordsStemmed : BagOfWords;
            foreach (var w in source)
            {
                if (w.Equals(searched))
                    count++;
            }
            return count;
        }
    }
}
