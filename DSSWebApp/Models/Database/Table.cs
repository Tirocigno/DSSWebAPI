using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSSWebApp.Models.Database
{
    public class Table
    {
        private string tableId;

        public Table(string tableId)
        {
            this.tableId = tableId;
        }

        public string getTableId()
        {
            return this.tableId;
        }
    }
}