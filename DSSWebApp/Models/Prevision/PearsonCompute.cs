using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DSSWebApp.Models.Prevision
{
    public class PearsonCompute
    {
        private string fileName;
        private static int MAX_STAGIONALITY = 13;
        
        public PearsonCompute(string fileName)
        {
            this.fileName = fileName;
        }

        public int computeStagionality()
        {
            double max = -1;
            int pearsonIndex = -1;
            double currentValue;
            int currentIndex = 1;
            int[] startArray = this.readSerieFromFile();

            while(currentIndex < MAX_STAGIONALITY)
            {
                currentValue = Correlation(startArray, buildShiftedArray(startArray, currentIndex));
                if(currentValue > max)
                {
                    pearsonIndex = currentIndex;
                    max = currentValue;
                }
                currentIndex++;
            }

            return pearsonIndex;
        }

        private int [] buildShiftedArray(int[] originArray, int shift)
        {
            int[] newArray = new int[originArray.Length];

            for(int i = 0; i < originArray.Length; i++)
            {
                if((i + shift) < originArray.Length)
                {
                    newArray[i] = originArray[i + shift];
                } else
                {
                    newArray[i] = 0;
                }
            }

            return newArray;
        }

        private double Correlation(IEnumerable<int> xs, IEnumerable<int> ys)
        {
            // sums of x, y, x squared etc.
            double sx = 0.0;
            double sy = 0.0;
            double sxx = 0.0;
            double syy = 0.0;
            double sxy = 0.0;

            int n = 0;

            using (var enX = xs.GetEnumerator())
            {
                using (var enY = ys.GetEnumerator())
                {
                    while (enX.MoveNext() && enY.MoveNext())
                    {
                        double x = enX.Current;
                        double y = enY.Current;

                        n += 1;
                        sx += x;
                        sy += y;
                        sxx += x * x;
                        syy += y * y;
                        sxy += x * y;
                    }
                }
            }

            // covariation
            double cov = sxy / n - sx * sy / n / n;
            // standard error of x
            double sigmaX = Math.Sqrt(sxx / n - sx * sx / n / n);
            // standard error of y
            double sigmaY = Math.Sqrt(syy / n - sy * sy / n / n);

            // correlation is just a normalized covariation
            return cov / sigmaX / sigmaY;
        }

        private int[] readSerieFromFile()
        {
            List<int> list = new List<int>();
            string line;
            StreamReader file = new StreamReader((string)AppDomain.CurrentDomain.GetData("DataDirectory") + "\\"+ fileName);
            file.ReadLine();
            while ((line = file.ReadLine()) != null)
            {
                list.Add(Convert.ToInt32(line.Replace(",",""))); 
            }

            file.Close();
            return list.ToArray();
        }
    }
}