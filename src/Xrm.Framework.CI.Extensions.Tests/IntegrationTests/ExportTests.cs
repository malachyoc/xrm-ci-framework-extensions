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
        public void IntegrationTest_ExportStuff()
        {
            //Read sample file
            IOrganizationService organisationService = new TestConnectionManager().CreateConnection("Connection.D365.UAT");

            //Update Data
            DataExportManager exporter = new DataExportManager(organisationService, new TestLogger());
            String fetchQuery = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
              <entity name='il_priorityrule'>
                <attribute name='il_type' />
                <attribute name='il_name' />
                <attribute name='il_validityperiod' />
                <attribute name='il_priorityruleid' />
                <attribute name='il_minvalue' />
                <attribute name='il_maxvalue' />
                <filter>
                  <condition attribute='il_name' operator='eq' value='Test Rule 3' />
                </filter>
              </entity>
            </fetch>";

            DataExportResult result = exporter.ExportData(fetchQuery, @"C:\svn\eBusiness\Utils\EnvironmentCreation\JSON\il_priorityrule.json");
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
