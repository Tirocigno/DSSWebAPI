using DSSWebApp.Models.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace DSSWebApp.Models.Prevision
{
    public class DataWriter
    {
        
        private List<serie> data;
        private string fileName;
        private static string INTESTATION = "sales";
        public static string BASIC_FILE_PATH = (string)AppDomain.CurrentDomain.GetData("DataDirectory") + "\\";

        public DataWriter(string fileName)
        {
            this.fileName = fileName;
            this.data = new DBConnection().readSerieFromDB(); 
        }

        private List<int> chooseSource(string fileName)
        {
            Debug.Print(fileName);
            List<int> l = new List<int>();
            if(fileName == "esempio.csv")
            {
                data.ForEach(elem =>
                {
                    if (elem.esempio != null)
                        l.Add((int)elem.esempio);
                });
            }

            if (fileName == "esempio2.csv")
            {
                data.ForEach(elem =>
                {
                    if (elem.esempio2 != null)
                        l.Add((int)elem.esempio2);
                });
            }

            if (fileName == "gioiellerie.csv")
            {
                data.ForEach(elem =>
                {
                    if (elem.jewelry != null)
                        l.Add((int)elem.jewelry);
                });
            }

            if (fileName == "passeggeri.csv")
            {
                data.ForEach(elem =>
                {
                    if (elem.Passengers != null)
                        l.Add((int)elem.Passengers);
                });
            }

            return l;
        }

        /*Convert data into csv file*/
        public void toCSVFile()
        {
            //Overwrite the file, if present.
            using (StreamWriter writer = new StreamWriter(BASIC_FILE_PATH + fileName, false))
            {
                writer.WriteLine(INTESTATION);
                writer.Close();
            }
            //Append to the file.
            StreamWriter appender = new StreamWriter(BASIC_FILE_PATH + fileName, true);
            List<int> source = this.chooseSource(fileName);
            source.ForEach(elem => appender.WriteLine(elem));
            appender.Close();

        }
        
    }
}