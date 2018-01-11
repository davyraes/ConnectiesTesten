using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class PlantInfo
    {
        public PlantInfo(int nplantnr,string nplantnaam,string nsoort,string nleverancier,string nkleur,decimal nkostprijs)
        {
            PlantNr = nplantnr;
            PlantNaam = nplantnaam;
            Soort = nsoort;
            Leverancier = nleverancier;
            Kleur = nkleur;
            Kostprijs = nkostprijs;
            Changed = false;
        }
        private int plantNummerValue;
        public bool Changed { get; set; }
        public int PlantNr
        {
            get { return plantNummerValue; }
            set { plantNummerValue = value; Changed = true; }
        }

        private string plantNaamValue;

        public string PlantNaam
        {
            get { return plantNaamValue; }
            set { plantNaamValue = value; Changed = true; }
        }

        private string soortValue;

        public string Soort
        {
            get { return soortValue; }
            set { soortValue = value; Changed = true; }
        }

        private string LeverancierValue;

        public string Leverancier
        {
            get { return LeverancierValue; }
            set { LeverancierValue = value; Changed = true; }
        }

        private string kleurValue;

        public string Kleur
        {
            get { return kleurValue; }
            set { kleurValue = value; Changed = true; }
        }

        private decimal kostprijsValue;

        public decimal Kostprijs
        {
            get { return kostprijsValue; }
            set { kostprijsValue = value; Changed = true; }
        }

    }
}
