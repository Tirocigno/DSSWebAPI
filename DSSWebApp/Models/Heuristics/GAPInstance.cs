using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSSWebApp.Models.Heuristics
{
    public class GAPInstance
    {
        public string name;
        /*Numero di client*/
        public int numcli;
        /*numero di server*/
        public int numserv;
        /*costo per ogni client a ogni server*/
        public int[,] cost;
        /*Capacità di ogni server*/
        public int[] cap;
        /*Richiesta, mi sfugge perchè sia bidimensionale*/
        public int[,] req;
        public int[] solBest;
        public int zub;
    }
}