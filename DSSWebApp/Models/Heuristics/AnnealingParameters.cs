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
        private int steps;
        private double tempDecrease;
        private int cooling_schedule_steps;

        public AnnealingParameters(string fileName, double temperature, int steps, double tempDecrease, int cooling_schedule_steps)
        {
            this.fileName = fileName;
            this.temperature = temperature;
            this.steps = steps;
            this.tempDecrease = tempDecrease;
            this.cooling_schedule_steps = cooling_schedule_steps;
        }

        public string getFileName()
        {
            return this.fileName;
        }

        public double getTemperature()
        {
            return this.temperature;
        }

        public int getSteps()
        {
            return this.steps;
        }

        public double getTempDecrease()
        {
            return this.tempDecrease;
        }

        public int getCoolingScheduleSteps()
        {
            return this.cooling_schedule_steps;
        }
    }
}