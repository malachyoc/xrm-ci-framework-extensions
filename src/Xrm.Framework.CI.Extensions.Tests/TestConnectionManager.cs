using Microsoft.Xrm.Sdk;
using System;
using System.Configuration;

namespace Xrm.Framework.CI.Common.IntegrationTests
{
    /// <summary>
    /// Copied from: xrm-ci-framework\MSDYNV9\Xrm.Framework.CI\Xrm.Framework.CI.Common.IntegrationTests\TestConnectionManager.cs
    /// </summary>
    public class TestConnectionManager
    {
        #region Constructors
        
        public TestConnectionManager()
        {

        }

        #endregion

        #region Methods

        public IOrganizationService CreateConnection()
        {
            string name = "CrmConnection";

            string connectionString = GetConnectionString(name);

            XrmConnectionManager con
                = new XrmConnectionManager(new Logging.TestLogger());

            return con.Connect(connectionString, 0);
        }

        private string GetConnectionString(string name)
        {
            string value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(value))
                value = ConfigurationManager.ConnectionStrings[name].ConnectionString;
            if (string.IsNullOrEmpty(value))
                throw new Exception($"connection with {name} was not found");
            return value;
        }

        #endregion
    }
}
