using System;
using System.IO;
using Microsoft.Xrm.Sdk;
using Xrm.Framework.CI.Common.IntegrationTests;
using Xrm.Framework.CI.Common.IntegrationTests.Logging;
using Xrm.Framework.CI.Extensions.DataOperations;
using Xunit;

namespace Xrm.Framework.CI.Extensions.Tests
{
    public class DataOperationTests
    {
        [Fact]
        public void ValidationTest()
        {
            var fileContent = File.ReadAllText(@"..\..\..\Xrm.Framework.CI.Common\Schema\CRUD.sample.json");

            string validationErrors;
            JsonParser.ValidateJson(fileContent, out validationErrors);

            Assert.True(String.IsNullOrWhiteSpace(validationErrors));
        }

        [Fact]
        public void CrudTest()
        {
            //Read sample file
            IOrganizationService organisationService = new TestConnectionManager().CreateConnection();
            DataImportManager importer = new DataImportManager(organisationService, new TestLogger());
            var result = importer.ImportData(@"..\..\..\Xrm.Framework.CI.Common\Schema\unused-plugin-types.all.json");
            //var result = importer.ImportData(@"..\..\..\Xrm.Framework.CI.Common\Schema\CRUD.sample.json");
            
        }
    }
}
