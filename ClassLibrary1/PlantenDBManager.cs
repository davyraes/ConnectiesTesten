using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Data;
using System.Data.Common;
using System.Collections.ObjectModel;
using System.Windows;

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
        public ObservableCollection<PlantInfo> GetPantInfoSoort(string soort)
        {
            ObservableCollection<PlantInfo> planten = new ObservableCollection<PlantInfo>();
            using (var conTuin = TuincentrumDBManager.Getconnection())
            {
                using (var comInfo = conTuin.CreateCommand())
                { 
                        comInfo.CommandType = CommandType.StoredProcedure;
                        comInfo.CommandText = "PlantInfoOpvragenSoort";

                        var parSoort = comInfo.CreateParameter();
                        parSoort.ParameterName = "@Soort";
                        parSoort.Value = soort;
                        comInfo.Parameters.Add(parSoort);
                    
                    conTuin.Open();
                    using (var rdrplanten = comInfo.ExecuteReader())
                    {
                        Int32 PlantnrPos = rdrplanten.GetOrdinal("PlNr");
                        Int32 PlantNaamPos = rdrplanten.GetOrdinal("PlNaam");
                        Int32 SoortPos = rdrplanten.GetOrdinal("Soort");
                        Int32 LeverancierPos = rdrplanten.GetOrdinal("Leverancier");
                        Int32 KleurPos = rdrplanten.GetOrdinal("Kleur");
                        Int32 PrijsPos = rdrplanten.GetOrdinal("VerkoopPrijs");
                        while(rdrplanten.Read())
                        {
                            planten.Add(new PlantInfo(
                            rdrplanten.GetInt32(PlantnrPos),
                            rdrplanten.GetString(PlantNaamPos),
                            rdrplanten.GetString(SoortPos),
                            rdrplanten.GetString(LeverancierPos),
                            rdrplanten.GetString(KleurPos),
                            rdrplanten.GetDecimal(PrijsPos)
                            ));
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
       public List<PlantInfo> SchrijfWijzigingenWeg(List<PlantInfo>planten)
        {
            List<PlantInfo> NietDoorGevoerd = new List<PlantInfo>();
            using (var conTuin = TuincentrumDBManager.Getconnection())
            {
                using (var ComSchrijf = conTuin.CreateCommand())
                {
                    ComSchrijf.CommandType = CommandType.Text;
                    ComSchrijf.CommandText = "update planten set kleur=@kleur,verkoopprijs=@prijs where plantnr=@plantnr";

                    var parKleur = ComSchrijf.CreateParameter();
                    parKleur.ParameterName = "@kleur";
                    ComSchrijf.Parameters.Add(parKleur);

                    var parPrijs = ComSchrijf.CreateParameter();
                    parPrijs.ParameterName = "@prijs";
                    ComSchrijf.Parameters.Add(parPrijs);

                    var parPlant = ComSchrijf.CreateParameter();
                    parPlant.ParameterName = "@plantnr";
                    ComSchrijf.Parameters.Add(parPlant);

                    conTuin.Open();
                    foreach(var plant in planten)
                    {
                        try
                        {
                            parKleur.Value = plant.Kleur;
                            parPrijs.Value = plant.Kostprijs;
                            parPlant.Value = plant.PlantNr;
                            if (ComSchrijf.ExecuteNonQuery() == 0)
                                NietDoorGevoerd.Add(plant);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            NietDoorGevoerd.Add(plant);
                        }
                    }
                }
            }
            return NietDoorGevoerd;
        }
    }
}
