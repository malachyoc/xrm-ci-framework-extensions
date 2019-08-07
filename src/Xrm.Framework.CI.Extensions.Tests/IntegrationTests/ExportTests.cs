using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Xrm.Framework.CI.Common.IntegrationTests;
using Xrm.Framework.CI.Common.IntegrationTests.Logging;
using Xrm.Framework.CI.Extensions.DataOperations;
using Xunit;
using static Xrm.Framework.CI.Extensions.DataOperations.DataExportManager;

namespace Xrm.Framework.CI.Extensions.Tests
{
    public class ExportTests
    {
        //[Fact(Skip = "Not a unit test")]
        [Fact()]
        public void IntegrationTest_ExportBUs()
        {
            //Read sample file
            IOrganizationService organisationService = new TestConnectionManager().CreateConnection();

            //Update Data
            DataExportManager exporter = new DataExportManager(organisationService, new TestLogger());
            exporter.LoadDataMappings(@"..\..\..\Xrm.Framework.CI.Extensions\Schema\entitymap.json");

            String fetchQuery = @"<fetch top='50' ><entity name='businessunit' ><order attribute='createdon' /></entity></fetch>";

            DataExportResult result = exporter.ExportData(fetchQuery, @"..\..\..\Xrm.Framework.CI.Extensions\Schema\test.json");
        }

        [Fact()]
        public void FetchXmlExportTest()
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
            DataExportManager exporter = new DataExportManager(organisationService, new TestLogger());

            String fetchQuery = @"
                <fetch mapping='logical'>   
                    <entity name='contact'>  
                        <attribute name='contactid'/>   
                        <attribute name='fullname'/>   
                    </entity>  
                </fetch>";

            string result = exporter.ExportJsonString(fetchQuery);
        }
    }
}
