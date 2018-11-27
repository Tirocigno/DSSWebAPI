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
    }
}