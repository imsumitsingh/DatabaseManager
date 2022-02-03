using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.IdentityModel.Protocols;

namespace DatabaseManager
{
    class Connection
    {
        System.Configuration.Configuration config;
        public Connection()
        {
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public string conString()
        {
            return config.ConnectionStrings.ConnectionStrings["db_connection"].ConnectionString;
        }

        //Save connection string to App.config file
        public void updateConString(string _newConnection)
        {
            config.ConnectionStrings.ConnectionStrings["db_connection"].ConnectionString = _newConnection;
            config.ConnectionStrings.ConnectionStrings["db_connection"].ProviderName = "System.Data.SqlClient";
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
