using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Leverancier
    {
        public Leverancier()
        {
            Changed = false;
        }
        public bool Changed { get; set; }
        private int levnrValue;
        public int LevNr { get { return levnrValue;} set { levnrValue = value; }  }
        private string naamValue;
        public string Naam { get { return naamValue; } set {naamValue =value;Changed = true; } }
        private string adresValue;
        public string Adres { get { return adresValue; } set {adresValue=value;Changed = true; } }
        private string postcodeValue;
        public string Postcode { get { return postcodeValue; } set {postcodeValue=value; Changed = true; } }
        private string woonplaatsValue;
        public string Woonplaats { get { return woonplaatsValue; } set {woonplaatsValue=value;Changed = true; } }
        public object Versie { get; set; }
    }
}
