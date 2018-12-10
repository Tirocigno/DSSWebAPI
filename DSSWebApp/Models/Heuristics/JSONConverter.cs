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
        public static GAPInstance deserializeGAP(string path)
        {
            StreamReader fin;
            String jstring;
            GAPInstance ist;
            try
            {
                fin = new StreamReader(path);
                jstring = fin.ReadToEnd();
                ist = JsonConvert.DeserializeObject<GAPInstance>(jstring);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                ist = new GAPInstance();
            }

            return ist;
        }

        public static string serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }

    
}