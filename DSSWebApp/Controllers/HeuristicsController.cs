using DSSWebApp.Models.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DSSWebApp.Controllers
{
    public class HeuristicsController : ApiController
    {
        private static string dataDirectory = (string)AppDomain.CurrentDomain.GetData("DataDirectory");

        /*IMPLEMENTARE UN GETTER PER QUESTA API*/
        [HttpGet] // Metodo http per l'api
        [ActionName("getGAPInstance")] // path dell'api.
        public string getGAPInstance()
        {
            string fileName = "elba.json";
            return JSONConverter.deserializeGAP(dataDirectory + "\\" + fileName).ToString();
        }

        /*IMPLEMENTARE UN GETTER PER QUESTA API*/
        [HttpPost] // Metodo http per l'api
        [ActionName("resolveDumpGAPInstance")] // path dell'api.
        public string resolveDumpGAPInstance()
        {
            string fileName = "elba.json";
            GAPInstance Gap = JSONConverter.deserializeGAP(dataDirectory + "\\" + fileName);
            BasicHeu h = new BasicHeu(Gap);
            h.constructiveEurFirstSol();
            return "Cost of solution is: " + h.opt10().ToString();
        }

        /*IMPLEMENTARE UN GETTER PER QUESTA API*/
        [HttpPost] // Metodo http per l'api
        [ActionName("resolveFirstHeuGAPInstance")] // path dell'api.
        public string resolveFirstHeuGAPInstance(string fileName)
        {
            GAPInstance Gap = JSONConverter.deserializeGAP(dataDirectory + fileName);
            return new BasicHeu(Gap).constructiveEurFirstSol().ToString();
        }


    }
}