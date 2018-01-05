using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Common;

namespace ClassLibrary1
{
    public static class TuincentrumDBManager
    {
        private static ConnectionStringSettings conTuinSetting = ConfigurationManager.ConnectionStrings["Tuincentrum"];
        private static DbProviderFactory factory = DbProviderFactories.GetFactory(conTuinSetting.ProviderName);
        public static DbConnection Getconnection()
        {
            var TuinCon = factory.CreateConnection();
            TuinCon.ConnectionString = conTuinSetting.ConnectionString;
            return TuinCon;
        }
    }
}
