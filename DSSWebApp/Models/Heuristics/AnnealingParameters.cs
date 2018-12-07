using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSSWebApp.Models.Heuristics
{
    public class AnnealingParameters
    {
        private string fileName;
        private double temperature;

        public AnnealingParameters(string fileName, double temperature)
        {
            this.fileName = fileName;
            this.temperature = temperature;
        }

        public string getFileName()
        {
            return this.fileName;
        }

        public double getTemperature()
        {
            return this.temperature;
        }
    }
}