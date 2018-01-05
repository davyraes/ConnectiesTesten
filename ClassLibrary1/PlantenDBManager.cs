using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Data;
using System.Data.Common;

namespace ClassLibrary1
{
    public class PlantenDBManager
    {
        public decimal BerekenGemiddeldePrijsPerSoort(string soort)
        {            
            using (var conTuin = TuincentrumDBManager.Getconnection())
            {
                using (var comBereken = conTuin.CreateCommand())
                {
                    comBereken.CommandType = CommandType.StoredProcedure;
                    comBereken.CommandText = "BerekenGemiddeldePrijsPerSoort";

                    var parSoort = comBereken.CreateParameter();
                    parSoort.ParameterName = "@soort";
                    parSoort.Value = soort;
                    comBereken.Parameters.Add(parSoort);
                    decimal bedrag;
                    conTuin.Open();
                    decimal.TryParse(comBereken.ExecuteScalar().ToString(), out bedrag);
                    return bedrag;
                }
            } 
        }
    }
}
