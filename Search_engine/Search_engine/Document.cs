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
                foreach (var s in _bagOdWordsStemmed)
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
                return Title + Content;
            }
        }

        private List<string> GetWords()
        {
            var splitted = ContentAll.Split();
            var cleanList = new List<string>();
            foreach (var w in splitted)
            {
                var corrected = w.Trim();
                Regex rgx = new Regex(@"[\\\-~!@#$%^\*()_+{}:;|',\./[\]]");
                var clean = rgx.Replace(corrected, "");
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

        public bool Contains(string searched)
        {
            foreach (var w in BagOfWordsStemmed)
            {
                if (w.Equals(searched))
                    return true;
            }
            return false;
        }

        public int CountOccurrences(string searched)
        {
            int count = 0;
            foreach (var w in BagOfWordsStemmed)
            {
                if (w.Equals(searched))
                    count++;
            }
            return count;
        }
    }
}
