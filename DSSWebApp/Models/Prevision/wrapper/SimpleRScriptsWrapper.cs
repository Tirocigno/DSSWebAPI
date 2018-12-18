using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace DSSWebApp.Models.Prevision.wrapper
{
    public class SimpleRScriptsWrapper
    {
        private static string dataDirectory = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
        private static int CUSTOM_HEAP_SIZE = 25000000;
        int frequency; //To be used later
        int nextValuesToCompute; //To be used later
        int[] results;
        string fileName;

        public SimpleRScriptsWrapper(int frequency, int nextValuesToCompute, string fileName)
        {
            this.frequency = frequency;
            this.nextValuesToCompute = nextValuesToCompute;
            this.fileName = fileName;
        }

        

        public string forecastComputation()
        {
            Thread t = new Thread(computeForecast, CUSTOM_HEAP_SIZE);
            t.Start();
            t.Join();
            string s = "Next Values:\n";
            for (int i = 0; i < results.Length; i++)
            {
                s += results[i].ToString() + "\n";
            }
            return s;
        }

        private void computeForecast()
        {
            string filePath = (dataDirectory + "\\" + fileName).Replace("\\", "/");
            StartupParameter rinit = new StartupParameter();
            rinit.Quiet = true;
            rinit.RHome = "C:\\Program Files\\R\\R-3.4.4";
            rinit.Interactive = true;
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance(null, true, rinit);
            engine.Evaluate("");
            engine.Evaluate("library(tseries)");
            engine.Evaluate("library(forecast)");
            engine.Evaluate("data <- read.csv(\"" + filePath + "\")");
            engine.Evaluate("myts <- ts(data[,1], frequency = "+frequency+")");
            engine.Evaluate("ARIMAfit1 <- auto.arima(myts, stepwise = FALSE, approximation = FALSE)");
            engine.Evaluate("myfc <- forecast(ARIMAfit1, h = "+ this.nextValuesToCompute +")");
            engine.Evaluate("intMean <- as.integer(myfc$mean)");
            IntegerVector a1 = engine.GetSymbol("intMean").AsInteger();
            results = a1.ToArray();
        }

        //Execute a previson on esempio on the 8 future values based on stagionality = 4.
        private void esempioForecast()
        {
            string esempioPath = (dataDirectory + "\\esempio.csv").Replace("\\", "/");
            StartupParameter rinit = new StartupParameter();
            rinit.Quiet = true;
            rinit.RHome = "C:\\Program Files\\R\\R-3.4.4";
            rinit.Interactive = true;
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance(null, true, rinit);
            engine.Evaluate("");
            engine.Evaluate("library(tseries)");
            engine.Evaluate("library(forecast)");
            engine.Evaluate("data <- read.csv(\""+ esempioPath +"\")");
            engine.Evaluate("myts <- ts(data[,2], frequency = 4)");
            engine.Evaluate("ARIMAfit1 <- auto.arima(myts, stepwise = FALSE, approximation = FALSE)");
            engine.Evaluate("myfc <- forecast(ARIMAfit1, h = 8)");
            engine.Evaluate("intMean <- as.integer(myfc$mean)");
            IntegerVector a1 = engine.GetSymbol("intMean").AsInteger();
            results = a1.ToArray();
        }

        //Execute a previson on gioiellerie time series on the 24 future values based on stagionality = 12.
        private void gioiellerieForecast()
        {
            string gioielleriePath = (dataDirectory + "\\gioiellerie.csv").Replace("\\", "/");
            StartupParameter rinit = new StartupParameter();
            rinit.Quiet = true;
            rinit.RHome = "C:\\Program Files\\R\\R-3.4.4";
            rinit.Interactive = true;
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance(null, true, rinit);
            engine.Evaluate("");
            engine.Evaluate("library(tseries)");
            engine.Evaluate("library(forecast)");
            engine.Evaluate("data <- read.csv(\"" + gioielleriePath + "\")");
            //THIS 3 must be converted back into two when the data are readed.
            engine.Evaluate("myts <- ts(data[,3], frequency = 12)");
            engine.Evaluate("ARIMAfit1 <- auto.arima(myts, stepwise = FALSE, approximation = FALSE)");
            engine.Evaluate("myfc <- forecast(ARIMAfit1, h = 24)");
            engine.Evaluate("intMean <- as.integer(myfc$mean)");
            IntegerVector a1 = engine.GetSymbol("intMean").AsInteger();
            results = a1.ToArray();
        }
    }
}