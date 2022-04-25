using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_MiniProjekat
{
    public class TableRow
    {
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public string DateString { get; set; }

        public TableRow() { }

        public override string ToString()
        {
            return Open + " " + High + " " + Low + " " + Close + " " + DateString;

        }
    }
}
