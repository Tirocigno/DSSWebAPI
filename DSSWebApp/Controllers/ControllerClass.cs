using DSSWebApp.Models.Database;
using DSSWebApp.Models.Heuristics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http; //Necessaria al funzionamento di tutto.

namespace DSSWebApp.Controllers
{
    public class DatabaseController : ApiController
    {
        private static string dataDirectory = (string) AppDomain.CurrentDomain.GetData("DataDirectory");
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
        }
        
        [HttpPost]
        [ActionName("insertCustomer")]
        public string insertCustomer(Table tableToRead)
        {
            string query = "select * from " + tableToRead.getTableId();
            return dbConnection.readTableViaFactory(query);
        }        */        [HttpPost]
        [ActionName("simplePost")]
        public string simplePost()
        {
            return "Post Done";
        }



    }
}