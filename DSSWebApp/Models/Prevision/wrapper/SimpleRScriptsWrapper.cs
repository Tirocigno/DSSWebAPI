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
        static string inputString;
        static string outputString;

        public SimpleRScriptsWrapper(string paramInputString)
        {
            inputString = paramInputString;
        }

        public string simpleRComputation()
        {
            Thread t = new Thread(simpleRComputationExecutor, 2500000 /*thread stack size of 2.5MB*/);
            t.Start();
            t.Join();
            return outputString;
        }

        private void simpleRComputationExecutor()
        {
            string param1 = inputString; // parametro in ingersso
            StartupParameter rinit = new StartupParameter();
            rinit.Quiet = true;
            rinit.RHome = "C:\\Program Files\\R\\R-3.4.4";
            rinit.Interactive = true;
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance(null, true, rinit);
            var x = engine.Evaluate("x <- 1 + 2");
            Console.WriteLine(x);
            engine.Evaluate("library(tseries)");
            engine.Evaluate("library(forecast)");
            var y = engine.Evaluate("y <- 4 + 7");
            outputString = y.ToString();
        }
    }
}