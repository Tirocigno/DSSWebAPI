using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http; //Necessaria al funzionamento di tutto.

namespace DSSWebApp.Controllers
{
    public class ClientiController : ApiController
    {
        [HttpGet] // Metodo http per l'api
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
                return NotFound();*/
            return Ok(queryText);
        }

        [HttpPost]
        [ActionName("insertCustomer")]
        public string insertCustomer(object obj)
        {
            /* string queryString = "insert into clienti (id, req, mag) values(";
             queryString += obj.id + "," + obj.req + ",'" + obj.mag + "')";
             P.execNonQueryViaF(connString, queryString, factory, true);
             return "Customer inserted"; // oppure dichiararla static*/
            return "fatto";
        }        
    }
}