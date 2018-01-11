using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Transactions;
using System.Collections.ObjectModel;

namespace ClassLibrary1
{
    public class leveranciersDBManager
    {
        
        public Int64 LeverancierToevoegen(Leverancier eenLeverancier)
        {
            
            using (var conTuin = TuincentrumDBManager.Getconnection())
            {
                using (var comToevoegen = conTuin.CreateCommand())
                {
                    comToevoegen.CommandType = CommandType.StoredProcedure;
                    comToevoegen.CommandText = "Toevoegen";

                    DbParameter parNaam = comToevoegen.CreateParameter();
                    parNaam.ParameterName = "@Naam";                    
                    parNaam.Value = eenLeverancier.Naam;
                    comToevoegen.Parameters.Add(parNaam);

                    DbParameter parAdres = comToevoegen.CreateParameter();                    
                    parAdres.ParameterName = "@Adres";
                    parAdres.Value = eenLeverancier.Adres;
                    comToevoegen.Parameters.Add(parAdres);

                    DbParameter parPostNr = comToevoegen.CreateParameter();
                    parPostNr.ParameterName = "@PostNr";
                    parPostNr.Value = eenLeverancier.Postcode;
                    comToevoegen.Parameters.Add(parPostNr);

                    DbParameter parWoonplaats = comToevoegen.CreateParameter();
                    parWoonplaats.ParameterName = "@Woonplaats";
                    parWoonplaats.Value = eenLeverancier.Woonplaats;
                    comToevoegen.Parameters.Add(parWoonplaats);
                                                          
                    conTuin.Open();
                    return Convert.ToInt64(comToevoegen.ExecuteScalar());
                }
            }
        }
        public void VervangLeverancier(int Oud,int Nieuw)
        {
            var opties = new TransactionOptions();
            opties.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            using (var traVervangen = new TransactionScope(TransactionScopeOption.Required, opties))
            {
                using (var conTuin = TuincentrumDBManager.Getconnection())
                {
                    using (var comKopieren = conTuin.CreateCommand())
                    {
                        comKopieren.CommandType = CommandType.StoredProcedure;
                        comKopieren.CommandText = "VervangenLeverancier";

                        var parOud = comKopieren.CreateParameter();
                        parOud.ParameterName = "@oud";
                        parOud.Value = Oud;
                        comKopieren.Parameters.Add(parOud);

                        var parNieuw = comKopieren.CreateParameter();
                        parNieuw.ParameterName = "@nieuw";
                        parNieuw.Value = Nieuw;
                        comKopieren.Parameters.Add(parNieuw);

                        conTuin.Open();
                        comKopieren.ExecuteNonQuery();
                        conTuin.Close();
                    }
                    using (var comVerwijderen = conTuin.CreateCommand())
                    {
                        comVerwijderen.CommandType = CommandType.StoredProcedure;
                        comVerwijderen.CommandText = "LeverancierVerwijderen";

                        var parLevNr = comVerwijderen.CreateParameter();
                        parLevNr.ParameterName = "@Levnr";
                        parLevNr.Value = Oud;
                        comVerwijderen.Parameters.Add(parLevNr);

                        conTuin.Open();
                        comVerwijderen.ExecuteNonQuery();
                    }
                }
                traVervangen.Complete();
            }
        }
        public Int32 Eindejaarskorting()
        {
            using (var conTuin = TuincentrumDBManager.Getconnection())
            {
                using (var comKorting = conTuin.CreateCommand())
                {
                    comKorting.CommandType = CommandType.StoredProcedure;
                    comKorting.CommandText = "EindejaarsKorting";
                    conTuin.Open();
                    return comKorting.ExecuteNonQuery();
                }
            }
        }
        public ObservableCollection<Leverancier>LeverancierPerPostcode(string postcode)
        {
            ObservableCollection<Leverancier> Leveranciers = new ObservableCollection<Leverancier>();
            using (var ConTuin = TuincentrumDBManager.Getconnection())
            {
                using (var comLijst = ConTuin.CreateCommand())
                {
                    int Postnr;
                    comLijst.CommandType = CommandType.Text;
                    
                    if (!int.TryParse(postcode, out Postnr))
                        comLijst.CommandText = "select * from leveranciers";
                    else
                    {
                        comLijst.CommandText = "select * from leveranciers where postnr=@postcode";

                        var parPostcode = comLijst.CreateParameter();
                        parPostcode.ParameterName = "@postcode";
                        parPostcode.Value = Postnr;
                        comLijst.Parameters.Add(parPostcode);

                        ConTuin.Open();
                        using (var rdrLeveranciers = comLijst.ExecuteReader())
                        {
                            Int32 levNrPos = rdrLeveranciers.GetOrdinal("LevNr");
                            Int32 naamPos = rdrLeveranciers.GetOrdinal("Naam");
                            Int32 adresPos = rdrLeveranciers.GetOrdinal("Adres");
                            Int32 postNrPos = rdrLeveranciers.GetOrdinal("PostNr");
                            Int32 woonplaatsPos = rdrLeveranciers.GetOrdinal("Woonplaats");

                            while(rdrLeveranciers.Read())
                            {
                                Leverancier leverancier = new Leverancier();
                                leverancier.LevNr = rdrLeveranciers.GetInt32(levNrPos);
                                leverancier.Naam = rdrLeveranciers.GetString(naamPos);
                                leverancier.Adres = rdrLeveranciers.GetString(adresPos);

                            }
                        }
                    }
                }
            }
        }
    }
}
