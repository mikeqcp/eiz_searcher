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
        private bool _useStemming;
        PorterStemmer _stemmer = new PorterStemmer();

        public Query(string queryString, bool useStemming)
        {
            _useStemming = useStemming;
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
                    if(_useStemming)
                        clean = _stemmer.stemTerm(clean);
                    cleanList.Add(clean.ToLower());
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

        public double[] CountQueryTFnotNormalized(List<string> keywords)
        {
            double[] tf = new double[keywords.Count];
            for (int j = 0; j < keywords.Count; j++)
            {
                var count = CountOccurrences(keywords[j]);
                tf[j] = count;
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
