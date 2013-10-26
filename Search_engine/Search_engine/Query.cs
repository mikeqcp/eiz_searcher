using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PorterStemmerAlgorithm;

namespace Search_engine
{
    public class Query
    {
        private string _query;
        private List<string> _queryWords;
        PorterStemmer _stemmer = new PorterStemmer();

        public Query(string queryString)
        {
            _query = queryString;
            _queryWords = SplitQuery();
        }

        private List<string> SplitQuery()
        {
            var splitted = _query.Split();
            var cleanList = new List<string>();
            foreach (var w in splitted)
            {
                var corrected = w.Trim();
                Regex rgx = new Regex(@"[\\\-~!@#$%^\*()_+{}:;|',\./[\]]");
                var clean = rgx.Replace(corrected, "");
                if (!string.IsNullOrEmpty(clean))
                {
                    var stemmed = _stemmer.stemTerm(clean.ToLower());
                    cleanList.Add(stemmed);
                }
            }
            return cleanList;
        }

        public double[] CountQueryTF(List<string> keywords)
        {
            double[] tf = new double[keywords.Count];
            var maxCount = 1;
            for (int j = 0; j < keywords.Count; j++)
            {
                var count = CountOccurrences(keywords[j]);
                tf[j] = count;
                maxCount = count > maxCount ? count : maxCount;
            }

            //normalize
            for (int j = 0; j < tf.Length; j++)
            {
                tf[j] = tf[j] / maxCount;
            }
            return tf;
        }


        private int CountOccurrences(string term)
        {
            var count = 0;
            foreach (var w in _queryWords)
            {
                count += term.Equals(w) ? 1 : 0;
            }
            return count;
        }
    }
}
