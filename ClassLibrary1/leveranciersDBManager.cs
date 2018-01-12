using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Transactions;
using System.Collections.ObjectModel;
using System.Windows;

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
                    comLijst.CommandType = CommandType.Text;

                    if ((postcode == string.Empty)||(postcode=="Alles"))
                        comLijst.CommandText = "select * from leveranciers";
                    else
                    {
                        comLijst.CommandText = "select * from leveranciers where postnr=@postcode";

                        var parPostcode = comLijst.CreateParameter();
                        parPostcode.ParameterName = "@postcode";
                        parPostcode.Value = postcode;
                        comLijst.Parameters.Add(parPostcode);
                    }    
                    
                        
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
                            var leverancier =new Leverancier();
                            leverancier.LevNr= rdrLeveranciers.GetInt32(levNrPos);
                            leverancier.Naam= rdrLeveranciers.GetString(naamPos);
                            leverancier.Adres= rdrLeveranciers.GetString(adresPos);
                            leverancier.Postcode= rdrLeveranciers.GetString(postNrPos);
                            leverancier.Woonplaats= rdrLeveranciers.GetString(woonplaatsPos);
                            leverancier.Changed = false;
                            Leveranciers.Add(leverancier);
                        }
                    }                    
                }
            }
            return Leveranciers;
        }
        public List<Leverancier>LeveranciersToevoegen(List<Leverancier>leveranciers)
        {
            List<Leverancier> NietToegevoegd = new List<Leverancier>();
            using (var conTuin = TuincentrumDBManager.Getconnection())
            {
                using (var comToevoegen = conTuin.CreateCommand())
                {
                    comToevoegen.CommandType = CommandType.StoredProcedure;
                    comToevoegen.CommandText = "Toevoegen";

                    var parNaam = comToevoegen.CreateParameter();
                    parNaam.ParameterName = "@Naam";
                    comToevoegen.Parameters.Add(parNaam);

                    var parAdres = comToevoegen.CreateParameter();
                    parAdres.ParameterName = "@Adres";
                    comToevoegen.Parameters.Add(parAdres);

                    var parPostNr = comToevoegen.CreateParameter();
                    parPostNr.ParameterName = "@PostNr";
                    comToevoegen.Parameters.Add(parPostNr);

                    var parWoonplaats = comToevoegen.CreateParameter();
                    parWoonplaats.ParameterName = "@Woonplaats";
                    comToevoegen.Parameters.Add(parWoonplaats);

                    conTuin.Open();
                   
                    foreach (Leverancier leverancier in leveranciers)
                    {
                        try
                        {
                            parNaam.Value = leverancier.Naam;
                            parAdres.Value = leverancier.Adres;
                            parPostNr.Value = leverancier.Postcode;
                            parWoonplaats.Value = leverancier.Woonplaats;
                            if (comToevoegen.ExecuteNonQuery()==0)
                            {
                                NietToegevoegd.Add(leverancier);
                            }
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            NietToegevoegd.Add(leverancier);
                        }
                    }
                    
                }
            }
            return NietToegevoegd;
        }
        public List<Leverancier>LeveranciersVerwijderen(List<Leverancier>Leveranciers)
        {
            List<Leverancier> NietVerwijderd = new List<Leverancier>();
            using (var conTuin = TuincentrumDBManager.Getconnection())
            {
                using (var ComVerwijderen = conTuin.CreateCommand())
                {
                    ComVerwijderen.CommandType = CommandType.StoredProcedure;
                    ComVerwijderen.CommandText = "LeverancierVerwijderen";

                    var parLevnr = ComVerwijderen.CreateParameter();
                    parLevnr.ParameterName = "@Levnr";
                    ComVerwijderen.Parameters.Add(parLevnr);

                    conTuin.Open();
                    foreach(Leverancier leverancier in Leveranciers)
                    {
                        try
                        {
                            parLevnr.Value = leverancier.LevNr;
                            if (ComVerwijderen.ExecuteNonQuery() == 0)
                                NietVerwijderd.Add(leverancier);
                        }
                        catch
                        {
                            NietVerwijderd.Add(leverancier);
                        }
                    }
                }
            }
            return NietVerwijderd;
        }
        public List<Leverancier>LeverancierAanpassen(List<Leverancier> Leveranciers)
        {
            List<Leverancier> NietAangepast = new List<Leverancier>();
            using (var conTuin = TuincentrumDBManager.Getconnection())
            {
                using (var comEdit = conTuin.CreateCommand())
                {
                    comEdit.CommandType = CommandType.StoredProcedure;
                    comEdit.CommandText = "LeveranierGegevensWijzigen";

                    var parNaam = comEdit.CreateParameter();
                    parNaam.ParameterName = "@Naam";
                    comEdit.Parameters.Add(parNaam);

                    var parAdres = comEdit.CreateParameter();
                    parAdres.ParameterName = "@Adres";
                    comEdit.Parameters.Add(parAdres);

                    var parPostNr = comEdit.CreateParameter();
                    parPostNr.ParameterName = "@PostNr";
                    comEdit.Parameters.Add(parPostNr);

                    var parWoonplaats = comEdit.CreateParameter();
                    parWoonplaats.ParameterName = "@Woonplaats";
                    comEdit.Parameters.Add(parWoonplaats);

                    var parLevNr = comEdit.CreateParameter();
                    parLevNr.ParameterName = "@levnr";
                    comEdit.Parameters.Add(parLevNr);

                    conTuin.Open();
                    foreach(Leverancier leverancier in Leveranciers)
                    {
                        try
                        {
                            parNaam.Value = leverancier.Naam;
                            parAdres.Value = leverancier.Adres;
                            parPostNr.Value = leverancier.Postcode;
                            parWoonplaats.Value = leverancier.Woonplaats;
                            parLevNr.Value = leverancier.LevNr;
                            if (comEdit.ExecuteNonQuery() == 0)
                                NietAangepast.Add(leverancier);
                        }
                        catch
                        {
                            NietAangepast.Add(leverancier);
                        }
                    }
                }
            }
            return NietAangepast;
        }
    }
}
