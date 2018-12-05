using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DSSWebApp.Models.Heuristics
{
    public class JSONConverter
    {
        public void deserializeGAP(string path)
        {
            StreamReader fin;
            String jstring;
            try
            {
                fin = new StreamReader(path);
                jstring = fin.ReadToEnd();
                GAPInstance ist = JsonConvert.DeserializeObject<GAPInstance>(jstring);
            }
            catch (Exception ex)
            {

            }
        }
    }
}