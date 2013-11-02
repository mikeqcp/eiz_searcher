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
        private double[] _queryTFIDFVector;
        private double[,] _TFvalues;
        private double[,] _TFIDFvalues;
        private double[] _rankValues;
        private List<ResultItem> _docOrder;

        public List<ResultItem> RankOrder
        {
            get
            {
                if (_docOrder == null)
                    _docOrder = CalculateRanks();
                return _docOrder;
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
            _TFIDFvalues = new double[wordsCount, docCount];
            _queryTFVector = query.CountQueryTF(_keywords);
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
            return _rankValues.OrderByDescending(r => r).Select(r => new ResultItem() { Document = _documents[Array.IndexOf(_rankValues, r)], RankValue = r }).ToList();
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
    }
}
