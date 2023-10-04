using System;
using System.Collections.Generic;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Xrm.Framework.CI.Common.IntegrationTests;
using Xrm.Framework.CI.Common.IntegrationTests.Logging;
using Xrm.Framework.CI.Extensions.DataOperations;
using Xunit;

namespace Xrm.Framework.CI.Extensions.Tests
{
    public class DataEncryptionTests 
    {
        //[Fact(Skip ="Not a unit test")]
        [Fact]
        public void SetEncryptionKey()
        {
            //Read sample file
            IOrganizationService organisationService = new TestConnectionManager().CreateConnection("Connection.D365.MOC");
            DataEncryptionManager importer = new DataEncryptionManager(organisationService, new TestLogger());

            importer.SetEncryptionKey("This is a test key. 2022");

        }

    }
}
