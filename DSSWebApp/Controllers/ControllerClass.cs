using DSSWebApp.Models.Database;
using DSSWebApp.Models.Heuristics;
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

       /* [HttpGet] // Metodo http per l'api
        [ActionName("GetAllClients")] // path dell'api.
        public string GetAllClients()
        {
            string res;
            res = "{\"nome\":\"pippo\"}";
            return res;
        }

        [HttpGet] // in esecuzione solo con un get dal client
        [ActionName("GetCustQuantities")] // nome del metodo esposto
        public IHttpActionResult GetCustQuantities(int id)
        {
            string queryText = "select id,req from clienti where id=" + id;
            /*string s = P.readTableViaF(connString, queryText, factory);
            lstClienti = JsonConvert.DeserializeObject<List<Clienti>>(s);
            var user = lstClienti.FirstOrDefault((u) => u.id == id);
            if (user == null)
                return NotFound();
            return Ok(queryText);
        }*/
        
        [HttpPost]
        [ActionName("insertCustomer")]
        public string insertCustomer(Table tableToRead)
        {
            string query = "select * from " + tableToRead.getTableId();
            return dbConnection.readTableViaFactory(query);
        }                [HttpPost]
        [ActionName("simplePost")]
        public string simplePost()
        {
            return "Post Done";
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

        [HttpGet]
        [ActionName("prevision")]
        public string executePrevision()
        {
            //  Thread t = new Thread(rCalculus, 2500000 /*thread stack size of 2.5MB*/);
            //t.Start();
            rCalculus();
            return "Done";
        }

        private void rCalculus()
        {
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
            Console.WriteLine("Arrivato in fondo");
            var y = engine.Evaluate("y <- 4 + 7");
            Console.WriteLine(y);
        }



    }
}