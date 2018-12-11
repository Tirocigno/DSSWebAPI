using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSSWebApp.Models.Heuristics
{
    public class TabuParameters
    {
        private string fileName;
        private int maxSteps;
        private int queueSize;

        public TabuParameters(string fileName, int maxSteps, int queueSize)
        {
            this.fileName = fileName;
            this.maxSteps = maxSteps;
            this.queueSize = queueSize;
        }

        public string getFileName()
        {
            return this.fileName;
        }

        public int getMaxSteps()
        {
            return this.maxSteps;
        }

        public int getQueueSize()
        {
            return this.queueSize;
        }
    }
}