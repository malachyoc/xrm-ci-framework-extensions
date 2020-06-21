using System;
using System.Collections.Generic;
using System.IO;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Xrm.Framework.CI.Common.IntegrationTests;
using Xrm.Framework.CI.Common.IntegrationTests.Logging;
using Xrm.Framework.CI.Extensions.DataOperations;
using Xunit;

namespace Xrm.Framework.CI.Extensions.Tests
{
    public class DataOperationTests 
    {
        [Fact(Skip ="Not a unit test")]
        public void ImportConfiguration()
        {
            //Read sample file
            IOrganizationService organisationService = new TestConnectionManager().CreateConnection("Connection.D365.Uat");
            DataImportManager importer = new DataImportManager(organisationService, new TestLogger());
            importer.LoadDataMappings(@"..solutions\environments\import-map.json");
            var result = importer.ImportFile(@"..\crm-platform\src\solutions\environments\8. systemusers-uat.json");
        }

        [Fact()]
        public void UpsertExistingTest()
        {
            var existingGuid = Guid.Parse("00000000-0ed7-e811-a30f-0050568a2d1a");
            var fakedContext = new XrmFakedContext();
            var newContact = new Entity("contact", existingGuid);
            newContact["fullname"] = "Malachy O'Connor";

            fakedContext.Initialize(new List<Entity>() {
                newContact
            });

            //Read sample file
            IOrganizationService organisationService = fakedContext.GetOrganizationService();

            //Update Data
            DataImportManager importer = new DataImportManager(organisationService, new TestLogger());
            var result = importer.ImportFile(@"..\..\..\Xrm.Framework.CI.Extensions\Schema\CRUD.sample.json");

            //Retrieve upserted record
            Entity updatedContact = organisationService.Retrieve("contact", existingGuid, new ColumnSet(true));
            Assert.Equal("Adrian Test", updatedContact["fullname"]);
        }

        [Fact(Skip = "FakeXrm not returning correct FaultCode")]
        public void UpsertNewTest()
        {
            var existingGuid = Guid.Parse("00000000-0ed7-e811-a30f-0050568a2d1a");
            var fakedContext = new XrmFakedContext();
            var newContact = new Entity("contact", Guid.NewGuid());
            newContact["fullname"] = "Malachy O'Connor";

            fakedContext.Initialize(new List<Entity>() {
                newContact
            });

            //Read sample file
            IOrganizationService organisationService = fakedContext.GetOrganizationService();
            DataImportManager importer = new DataImportManager(organisationService, new TestLogger());
            var result = importer.ImportFile(@"..\..\..\Xrm.Framework.CI.Extensions\Schema\CRUD.sample.json");

            //Retrieve created record   
            Entity updatedContact = organisationService.Retrieve("contact", existingGuid, new ColumnSet(true));
            Assert.Equal("Adrian Test", updatedContact["fullname"]);
        }

        [Fact(Skip ="FakeXrm not returning correct FaultCode")]
        public void DeleteNonExistingTest()
        {
            var json = @"{
                  'schema': 'https://github.com/malachyoc/xrm-ci-framework-extensions/_schema/crmdata.schema.json',
                  'schemaFormat': 'http://json-schema.org/draft-07/schema#',
                  'schemaVersion': '1-0-0',
                  'entities': [
                    { /** LicenceGradeUpdate **/
                      'LogicalName': 'contact',
                      'Id': '00000000-0ed7-e811-a30f-0050568a2d1a',
                      'Operation': 'delete',
                    }
                  ]
                }";

            var existingGuid = Guid.Parse("00000000-0ed7-e811-a30f-0050568a2d1a");
            var fakedContext = new XrmFakedContext();
            var newContact = new Entity("contact", Guid.NewGuid());
            newContact["fullname"] = "Malachy O'Connor";

            fakedContext.Initialize(new List<Entity>() {
                newContact
            });

            //Read sample file
            IOrganizationService organisationService = fakedContext.GetOrganizationService();
            DataImportManager importer = new DataImportManager(organisationService, new TestLogger());
            var result = importer.ImportJson(json);

            //Retrieve created record   
            Entity updatedContact = organisationService.Retrieve("contact", existingGuid, new ColumnSet(true));

        }
    }
}
