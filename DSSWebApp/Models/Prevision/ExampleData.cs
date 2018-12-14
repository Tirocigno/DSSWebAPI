using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DSSWebApp.Models.Prevision
{
    public class ExampleData
    {
        private int currentYear;
        private int stagionality;
        private List<serie> data;
        private static string INTESTATION = "anno,trim,sales";
        public static string EXAMPLE_FILE_PATH = (string)AppDomain.CurrentDomain.GetData("DataDirectory") + "\\esempio.csv";

        public ExampleData(int startYear, int stagionality, List<serie> data)
        {
            this.currentYear = startYear;
            this.stagionality = stagionality;
            this.data = data;
        }

        /*Constructor reference in c#*/
        public ExampleData(List<serie> data): this(2004, 4, data)
        {

        }

        /*Convert data into csv file*/
        public void toCSVFile()
        {
            //Overwrite the file, if present.
            using (StreamWriter writer = new StreamWriter(EXAMPLE_FILE_PATH, false))
            {
                writer.Write(INTESTATION);
                writer.Close();
            }
            //Append to the file.
            StreamWriter appender = new StreamWriter(EXAMPLE_FILE_PATH, true);
            int index = 0;
            while(this.data.ElementAt(index) != null)
            {
                if((index % this.stagionality) == 0)
                {
                    this.currentYear++;
                }
                appender.WriteLine(currentYear +"," + ((index % stagionality) + 1) + "," + this.data.ElementAt(index));
                index++;
            }

        }
        
    }
}