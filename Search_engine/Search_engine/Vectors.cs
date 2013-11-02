using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Search_engine
{
    static class Vectors
    {

        public static double GetSimilarity(double[] v1, double[] v2)
        {
            var lengthProduct = Vectors.GetLength(v1) * Vectors.GetLength(v2);
            var vecProduct = Vectors.VecProduct(v1, v2);
            if (lengthProduct != 0)
                return vecProduct / lengthProduct;
            else
                return 0;
        }

        private static double GetLength(double[] vec)
        {
            var sum = vec.Sum(v => v*v);
            return Math.Sqrt(sum);
        }

        private static double VecProduct(double[] v1, double[] v2)
        {
            double sum = 0;
            if (v1.Length != v2.Length) throw new Exception("Lengths not equal.");

            for (int i = 0; i < v1.Length; i++)
            {
                sum += v1[i] * v2[i];
            }
            return sum;
        }
    }
}
