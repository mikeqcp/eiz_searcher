using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Search_engine
{
    public class Ranker
    {
        private List<Document> _documents;
        private List<string> _keywords;
        private bool _useStemming;

        private double[] _queryTFVector;
        private double[] _queryTFVectorNotNormalized;
        private double[] _queryTFIDFVector;
        private double[,] _TFvalues;
        private double[,] _TFvaluesNotNormalized;
        private double[,] _TFIDFvalues;
        private double[] _rankValues;
        private List<ResultItem> _docOrder;
        private List<string> _queryHelp;

        private int _queryHelpersCount = 5;
        private double _firstQueryDocumentsSplitPercentage = 0.05;
        private double _queryDocumentsSplitPercentageDelta = 0.05;

        public List<ResultItem> RankOrder
        {
            get
            {
                if (_docOrder == null)
                    _docOrder = CalculateRanks();
                _queryHelp = GenerateHelpQueries();
                return _docOrder;
            }
        }

        public List<string> QueryHelp
        {
            get
            {
                return _queryHelp;
            }
        }

        public Ranker(DataSource data, Query query, bool useStemming)
        {
            _documents = data.Documents;
            _keywords = data.Keywords;
            _useStemming = useStemming;

            int docCount = _documents.Count;
            int wordsCount = _keywords.Count;

            _TFvalues = new double[wordsCount, docCount];
            _TFvaluesNotNormalized = new double[wordsCount, docCount];
            _TFIDFvalues = new double[wordsCount, docCount];
            _queryTFVector = query.CountQueryTF(_keywords);
            _queryTFVectorNotNormalized = query.CountQueryTFnotNormalized(_keywords);
            _queryTFIDFVector = new double[_queryTFVector.Length];
        }

        public void CalculateTF()
        {
            for (int i = 0; i < _documents.Count; i++)
            {
                var doc = _documents[i];
                var maxCount = 1;
                for (int j = 0; j < _keywords.Count; j++)
                {
                    var count = doc.CountOccurrences(_keywords[j], _useStemming);
                    _TFvalues[j, i] = count;
                    _TFvaluesNotNormalized[j, i] = count;
                    maxCount = count > maxCount ? count : maxCount;
                }

                //normalize
                for (int j = 0; j < _keywords.Count; j++)
                {
                    _TFvalues[j, i] = _TFvalues[j, i] / maxCount;
                }
            }
        }

        public void CalculateIDF()
        {
            for (int i = 0; i < _keywords.Count; i++)
            {
                var idf = CountIDFForTerm(i);
                _queryTFIDFVector[i] = _queryTFVector[i] * idf;
                for (int j = 0; j < _documents.Count; j++)
                {
                    _TFIDFvalues[i, j] = _TFvalues[i, j] * idf;
                }
            }
        }

        private double CountIDFForTerm(int termIndex)
        {
            int allDocsCount = _documents.Count;
            int termCount = 0;
            for (int i = 0; i < allDocsCount; i++)
            {
                termCount += _TFvalues[termIndex, i] > 0 ? 1 : 0;
            }
            if (termCount == 0) termCount = 1;
            return Math.Log((double)allDocsCount / termCount);
        }

        private double[] GetDocVector(int docIndex)
        {
            double[] vec = new double[_keywords.Count];
            for (int i = 0; i < _keywords.Count; i++)
            {
                vec[i] = _TFIDFvalues[i, docIndex];
            }
            return vec;
        }

        private List<ResultItem> CalculateRanks()
        {
            CalculateTF();
            CalculateIDF();
            _rankValues = CalculateRankValues();
            return _documents.Select(d => new ResultItem() { Document = d, RankValue = _rankValues[_documents.IndexOf(d)] }).OrderByDescending(d => d.RankValue).ToList();
        }

        private double[] CalculateRankValues()
        {
            var rankValues = new double[_documents.Count];
            for (int i = 0; i < _documents.Count; i++)
            {
                var docVector = GetDocVector(i);
                rankValues[i] = Vectors.GetSimilarity(docVector, _queryTFIDFVector);
            }
            return rankValues;
        }

        private List<string> GenerateHelpQueries()
        {
            List<string> allHelpQueries = new List<string>();
            double documentPercentageSplit = _firstQueryDocumentsSplitPercentage;

            for (int queryIndex = 0; queryIndex < _queryHelpersCount; queryIndex++)
            {
                double[] newQueryTFnotNormalized = new double[_keywords.Count];
                _queryTFVectorNotNormalized.CopyTo(newQueryTFnotNormalized, 0);

                int bestDocsCount = (int) (documentPercentageSplit * _documents.Count);
                int worstDocsCount = _documents.Count - bestDocsCount;

                for (int docIndex = 0; docIndex < _documents.Count; docIndex++)
                {
                    int indexOfDoc = _documents.IndexOf(_docOrder[docIndex].Document);

                    for (int i = 0; i < newQueryTFnotNormalized.Length; i++)
                    {
                        if (docIndex < bestDocsCount)
                        {
                            newQueryTFnotNormalized[i] += 0.5 * _TFvaluesNotNormalized[i, indexOfDoc] / bestDocsCount;
                        }
                        else
                        {
                            newQueryTFnotNormalized[i] -= 0.25 * _TFvaluesNotNormalized[i, indexOfDoc] / worstDocsCount;
                        }
                    }
                }

                List<Tuple<double, string>> tupleQueryList = new List<Tuple<double, string>>();
                List<string> queryKeywordsList = new List<string>();
                for (int i = 0; i < newQueryTFnotNormalized.Length; i++)
                {
                    if (_queryTFVectorNotNormalized[i] > 0)
                    {
                        queryKeywordsList.Add(_keywords[i]);
                        continue;
                    }
                    tupleQueryList.Add(new Tuple<double, string>(newQueryTFnotNormalized[i], _keywords[i]));
                }

                String newQuery = String.Join(" ", queryKeywordsList.ToArray());
                allHelpQueries.Add(newQuery + " " + String.Join(" ", tupleQueryList.OrderByDescending(t => t.Item1).Take(5).Select(t => t.Item2).ToArray()));
                documentPercentageSplit += _queryDocumentsSplitPercentageDelta;
            }
            return allHelpQueries;
        }
    }
}
