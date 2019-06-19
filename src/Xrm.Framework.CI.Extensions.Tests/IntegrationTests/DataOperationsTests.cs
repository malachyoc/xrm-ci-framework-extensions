using System;
using System.Collections.Generic;
using System.IO;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Xrm.Framework.CI.Common.IntegrationTests;
using Xrm.Framework.CI.Common.IntegrationTests.Logging;
using Xrm.Framework.CI.Extensions.DataOperations;
using Xunit;

namespace Xrm.Framework.CI.Extensions.Tests
{
    public class DataOperationTests
    {
        [Fact(Skip = "Not a unit test")]
        public void CrudTest_IntegrationTest()
        {
            //Read sample file
            IOrganizationService organisationService = new TestConnectionManager().CreateConnection();
            DataImportManager importer = new DataImportManager(organisationService, new TestLogger());
            var result = importer.ImportData(@"..\..\..\Xrm.Framework.CI.Extensions\Schema\CRUD.sample.json");
        }

        [Fact]
        public void CrudTest()
        {
            var fakedContext = new XrmFakedContext();
            List<Entity> existingEntities = new List<Entity>();
            Guid existingContactId = Guid.Parse("00000000-0ed7-e811-a30f-0050568a2d1a");
            Entity existingContact = new Entity("contact", existingContactId);
            existingContact.Attributes["fullname"] = "Malachy O'Connor";

            existingEntities.Add(existingContact);
            fakedContext.Initialize(existingEntities);

            //Read sample file
            IOrganizationService crmService = fakedContext.GetOrganizationService();
            DataImportManager importer = new DataImportManager(crmService, new TestLogger());
            var result = importer.ImportData(@"..\..\..\Xrm.Framework.CI.Extensions\Schema\CRUD.sample.json");

            //Assert that the contact was created
            Entity upsertedContact = crmService.Retrieve("contact", Guid.Parse("00000000-0ed7-e811-a30f-0050568a2d1a")
                , new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

            Assert.Equal("Adrian Test", upsertedContact["fullname"]);
        }
    }
}
