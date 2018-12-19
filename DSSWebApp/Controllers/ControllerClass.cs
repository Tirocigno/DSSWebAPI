using DSSWebApp.Models.Database;
using DSSWebApp.Models.Heuristics;
using DSSWebApp.Models.Prevision;
using DSSWebApp.Models.Prevision.wrapper;
using RDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http; //Necessaria al funzionamento di tutto.

namespace DSSWebApp.Controllers
{
    public class DatabaseController : ApiController
    {
        private static string dataDirectory = (string) AppDomain.CurrentDomain.GetData("DataDirectory");
        private static string CLIENTI_DB = "clienti";
        private static string ORDINI_DB = "ordini";
        private static string SERIE_DB = "serie";
        private DBConnection dbConnection = new DBConnection();
     

        private void controllerEventHandler(object sender, string message)
        {
            StreamWriter writeLog = new StreamWriter(dataDirectory + "\\log.txt", true);
            string messageToWrite = "[" + DateTime.Now.ToString() + "]: " + message;
            writeLog.WriteLine(message);
            writeLog.Close();
        }

        [HttpGet]
        [ActionName("readDB")]
        public string readDB(string dbToRead)
        {
            switch(dbToRead)
            {
                case "clienti": return JSONConverter.serialize(dbConnection.readClientiFromDB());
                case "ordini": return JSONConverter.serialize(dbConnection.readOrdiniFromDB());
                case "serie": return JSONConverter.serialize(dbConnection.readSerieFromDB());
                default: return "Error, no db found";
            }
        }

        [HttpPost]
        [ActionName("prevision")]
        public string executePrevision([FromBody]string fileName)
        {
            DataWriter w = new DataWriter(fileName);
            w.toCSVFile();
            PearsonCompute p = new PearsonCompute(fileName);
            int stagionality = p.computeStagionality();
            return new SimpleRScriptsWrapper(stagionality, stagionality*4 ,fileName).forecastComputation();
           // return p.computeStagionality().ToString();
        }

        [HttpGet]
        [ActionName("pearson")]
        public string executePearson()
        {
            return new PearsonCompute("esempio.csv").computeStagionality().ToString();
        }
    }
}