using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSSWebApp.Models.Heuristics
{
    public class GAPInstance
    {
        public string name;
        public int numcli;
        public int numserv;
        public int[,] cost;
        public int[] cap;
        public int[,] req;
        public int[] solBest;
        public int zub;
    }
}