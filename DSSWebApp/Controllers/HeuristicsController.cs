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

        /*Ritorna l'oggetto GAPInstance letto da file*/ 
        [HttpGet] // Metodo http per l'api
        [ActionName("getGAPInstance")] // path dell'api.
        public string getGAPInstance(string fileName)
        {
            return JSONConverter.deserializeGAP(dataDirectory + "\\" + fileName).ToString();
        }

        /*Calcola il costo della soluzione utilizzando un algoritmo di assegnamento random.*/
        [HttpPost] // Metodo http per l'api
        [ActionName("resolveRandomGAPInstance")] // path dell'api.
        public string resolveRandomGAPInstance([FromBody]string fileName)
        {
            GAPInstance Gap = JSONConverter.deserializeGAP(dataDirectory + "\\" + fileName);
            return new BasicHeu(Gap).constructFirstSol().ToString();
        }

        /*Calcola il costo della soluzione utilizzando un algoritmo costruttivo.*/
        [HttpPost] // Metodo http per l'api
        [ActionName("resolveConstructiveGAPInstance")] // path dell'api.
        public string resolveConstructiveGAPInstance([FromBody] string fileName)
        {
            GAPInstance Gap = JSONConverter.deserializeGAP(dataDirectory + "\\" + fileName);
            return new BasicHeu(Gap).constructiveEurFirstSol().ToString();
        }

        /*Calcola il costo della soluzione utilizzando un algoritmo basato sui mimimi locali*/
        [HttpPost] // Metodo http per l'api
        [ActionName("resolveOpt1_0GAPInstance")] // path dell'api.
        public string resolveOpt1_0GAPInstance([FromBody] string fileName)
        {
            GAPInstance Gap = JSONConverter.deserializeGAP(dataDirectory + "\\" + fileName);
            return new BasicHeu(Gap).opt10().ToString();
        }

        /*Calcola il costo della soluzione utilizzando un algoritmo di Simulated Annealing*/
        [HttpPost] // Metodo http per l'api
        [ActionName("resolveSimulatedAnnealingGAPInstance")] // path dell'api.
        public string resolveSimulatedAnnealingGAPInstance(AnnealingParameters paramts)
        {
            GAPInstance Gap = JSONConverter.deserializeGAP(dataDirectory + "\\" + paramts.getFileName());
            return new BasicHeu(Gap).simulatedAnnealing(paramts.getTemperature()).ToString();
        }
    }
}