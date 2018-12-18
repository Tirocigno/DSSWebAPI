using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Linq;
using LinqStatistics;

namespace DSSWebApp.Models.Prevision
{
    public static class PearsonWrapper {
        public static double ComputePearson(this IEnumerable<Double> source, IEnumerable<Double> other)
        {
            return source.Covariance(other) / (source.StandardDeviationP() * other.StandardDeviationP());
        }
    }
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
            Double[] startArray = this.readSerieFromFile();

            while(currentIndex < MAX_STAGIONALITY)
            {
                //currentValue = Correlation(startArray,buildShiftedArray(startArray, currentIndex));
                currentValue = PearsonWrapper.ComputePearson(startArray, buildShiftedArray(startArray, currentIndex));
                writeOnLog("Pearson "+ currentIndex + ": " + currentValue);
                if(currentValue > max)
                {
                    pearsonIndex = currentIndex;
                    max = currentValue;
                }
                currentIndex++;
            }

            return pearsonIndex;
        }

        private Double [] buildShiftedArray(Double[] originArray, int shift)
        { 
            double[] arr = new double[originArray.Length];
            Array.Copy(originArray, 0, arr, shift, arr.Length - shift);
            return arr;
        }

        

        private double Correlation(IEnumerable<Double> xs, IEnumerable<Double> ys)
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
            return cov / (sigmaX * sigmaY);
        }

        private Double[] readSerieFromFile()
        {
            List<Double> list = new List<Double>();
            string line;
            StreamReader file = new StreamReader((string)AppDomain.CurrentDomain.GetData("DataDirectory") + "\\"+ fileName);
            writeOnLog(file.ReadLine());
            while ((line = file.ReadLine()) != null)
            {
                list.Add(Convert.ToDouble(line.Replace(",","")));
                writeOnLog(line);
            }

            file.Close();
            writeOnLog("Data Readed");
            return list.ToArray();
        }

        private void writeOnLog(string message)
        {
            StreamWriter writeLog = new StreamWriter((string)AppDomain.CurrentDomain.GetData("DataDirectory") + "\\pearson.txt", true);
            string messageToWrite = "[" + DateTime.Now.ToString() + "]: " + message;
            writeLog.WriteLine(message);
            writeLog.Close();
        }
    }
}