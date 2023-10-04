using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.IO;
using System.Management.Automation;
using Xrm.Framework.CI.Common;
using Xrm.Framework.CI.Extensions.DataOperations;

namespace Xrm.Framework.CI.PowerShell.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Imports a CRM data</para>
    /// <para type="description">This cmdlet imports a JSON file that creates, updates and deleted datareturn AsyncJobId for async imports
    /// </para>
    /// </summary>
    /// <example>
    ///   <code>C:\PS>Import-XrmData -ConnectionString "" -DataFilePath "account"</code>
    ///   <para>Exports the "" managed solution to "" location</para>
    /// </example>
    [Cmdlet(VerbsData.Import, "DataEncryptionKey")]
    public class ImportDataEncryptionKeyCommand : XrmCommandBase
    {
        #region Parameters
        [Parameter(Mandatory = false)]
        public string DataEncryptionKey { get; set; }

        public ImportDataEncryptionKeyCommand()
        {
        }

        #endregion

        #region Process Record
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            XrmConnectionManager xrmConnection = new XrmConnectionManager(Logger);
            IOrganizationService crmService = xrmConnection.Connect(ConnectionString, 120);

            //Load External Mappings
            DataEncryptionManager dataManager = new DataEncryptionManager(crmService, Logger);

            try
            {
                var response = dataManager.SetEncryptionKey(DataEncryptionKey);
                if(response.Success)
                {
                    Logger.LogInformation("Set Encryption Key reported success: {0}", response.ErrorMessage);
                }
                else
                {
                    Logger.LogInformation("Set Encryption Key reported failure: {0}", response.ErrorMessage);
                }

            }
            catch(Exception ex)
            {
                Logger.LogInformation($"Exception of type '{ex.GetType().Name}' thrown while setting Data Encryption Key: {ex.Message}");
            }
        }
        #endregion
    }
}