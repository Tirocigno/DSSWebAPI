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

        /*Compute stagionality from outside*/
        public int computeStagionality()
        {
            Double[] startArray = this.readSerieFromFile();
            int stagionality = computePearson(startArray);
            if(stagionality == 1)
            {
                startArray = this.readSerieFromFile();
                stagionality = computePearson(this.computeLogScaleArray(startArray));

            }

            return stagionality;
        }

        private int computePearson(Double[] startArray)
        {
            double max = -1;
            int pearsonIndex = -1;
            double currentValue;
            int currentIndex = 1;

            while (currentIndex < MAX_STAGIONALITY)
            {
                currentValue = PearsonWrapper.ComputePearson(startArray, buildShiftedArray(startArray, currentIndex));
                writeOnLog("Pearson " + currentIndex + ": " + currentValue);
                if (currentValue > max)
                {
                    pearsonIndex = currentIndex;
                    max = currentValue;
                }
                currentIndex++;
            }

            return pearsonIndex;
        }

        /// <summary>
        /// Calculate LogScale array, unfortunately does not work as intended.
        /// </summary>
        /// <param name="startArray"></param>
        /// <returns></returns>
        private Double[] computeLogScaleArray(Double[] startArray)
        {
            Double[] logArray = new Double[startArray.Length - 1];
            for (int i = 0; i < startArray.Length; i++)
            {
                startArray[i] = Math.Log(startArray[i]);
                if (i >= 1)
                {
                    logArray[i - 1] = startArray[i] - startArray[i - 1];
                }
            }

            return logArray;
        }

        /*Create an array with value shifted of one position used for correlograms*/
        private Double [] buildShiftedArray(Double[] originArray, int shift)
        { 
            double[] arr = new double[originArray.Length];
            Array.Copy(originArray, 0, arr, shift, arr.Length - shift);
            return arr;
        }

        /*Read a serie from file and return its value.*/
        private Double[] readSerieFromFile()
        {
            List<Double> list = new List<Double>();
            string line;
            StreamReader file = new StreamReader((string)AppDomain.CurrentDomain.GetData("DataDirectory") + "\\"+ fileName);
            writeOnLog(file.ReadLine());
            while ((line = file.ReadLine()) != null)
            {
                list.Add(Convert.ToDouble(line.Replace(",",".")));
                writeOnLog(line);
            }

            file.Close();
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