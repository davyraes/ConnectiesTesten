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
        public PlantInfo VraagPlantInfoOp(int plantNummer)
        {
            using (var conTuin = TuincentrumDBManager.Getconnection())
            {
                using (var comInfo = conTuin.CreateCommand())
                {
                    comInfo.CommandType = CommandType.StoredProcedure;
                    comInfo.CommandText = "PlantInfoOpvragen";

                    var parPlantnr = comInfo.CreateParameter();
                    parPlantnr.ParameterName = "@PlantNr";
                    parPlantnr.Value = plantNummer;
                    comInfo.Parameters.Add(parPlantnr);

                    var parPlantNaam = comInfo.CreateParameter();
                    parPlantNaam.ParameterName = "@PlantNaam";
                    parPlantNaam.DbType = DbType.String;
                    parPlantNaam.Size = 45;
                    parPlantNaam.Direction = ParameterDirection.Output;
                    comInfo.Parameters.Add(parPlantNaam);

                    var parSoort = comInfo.CreateParameter();
                    parSoort.ParameterName = "@Soort";
                    parSoort.DbType = DbType.String;
                    parSoort.Size = 20;
                    parSoort.Direction = ParameterDirection.Output;
                    comInfo.Parameters.Add(parSoort);

                    var parLeverancier = comInfo.CreateParameter();
                    parLeverancier.ParameterName = "@Leverancier";
                    parLeverancier.DbType = DbType.String;
                    parLeverancier.Size = 50;
                    parLeverancier.Direction = ParameterDirection.Output;
                    comInfo.Parameters.Add(parLeverancier);

                    var parKleur = comInfo.CreateParameter();
                    parKleur.ParameterName = "@Kleur";
                    parKleur.DbType = DbType.String;
                    parKleur.Size = 20;
                    parKleur.Direction = ParameterDirection.Output;
                    comInfo.Parameters.Add(parKleur);

                    var parKostprijs = comInfo.CreateParameter();
                    parKostprijs.ParameterName = "@Kostprijs";
                    parKostprijs.DbType = DbType.Currency;
                    parKostprijs.Direction = ParameterDirection.Output;
                    comInfo.Parameters.Add(parKostprijs);

                    conTuin.Open();
                    comInfo.ExecuteNonQuery();
                    if (parKostprijs.Value.Equals(DBNull.Value))
                        throw new Exception("Plant bestaat niet");
                    return new PlantInfo((int)parPlantnr.Value, (string)parPlantNaam.Value, (string)parSoort.Value, (string)parLeverancier.Value, (string)parKleur.Value, (decimal)parKostprijs.Value);
                }
            }
        }
        public List<string> GetPantInfoSoort(string soort)
        {
            List<string> planten = new List<string>();
            using (var conTuin = TuincentrumDBManager.Getconnection())
            {
                using (var comInfo = conTuin.CreateCommand())
                {
                    comInfo.CommandType = CommandType.StoredProcedure;
                    comInfo.CommandText = "PlantenPerSoort";

                    var parSoort = comInfo.CreateParameter();
                    parSoort.ParameterName = "@Soort";
                    parSoort.Value = soort;
                    comInfo.Parameters.Add(parSoort);

                    conTuin.Open();
                    using (var rdrplanten = comInfo.ExecuteReader())
                    {
                        while(rdrplanten.Read())
                        {
                            planten.Add(rdrplanten["naam"].ToString());
                        }
                    }
                }
            }
            return planten;
        }
        public static List<string> GetSoorten()
        {
            List<string> soorten = new List<string>();
            using (var conTuin = TuincentrumDBManager.Getconnection())
            {
                using (var comSoort = conTuin.CreateCommand())
                {
                    comSoort.CommandType = CommandType.Text;
                    comSoort.CommandText = "select Soort from soorten order by Soort";

                    conTuin.Open();
                    using (var rdrplanten = comSoort.ExecuteReader())
                    {
                        var soortpos = rdrplanten.GetOrdinal("Soort");
                        while (rdrplanten.Read())
                        {
                            soorten.Add(rdrplanten.GetString(soortpos));
                        }
                    }
                }
            }
            return soorten;
        }
    }
}
