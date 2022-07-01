using Microsoft.Xrm.Sdk;
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
    [Cmdlet(VerbsData.Import, "XrmData")]
    public class ImportXrmDataCommand : XrmCommandBase
    {
        #region Parameters
        /// <summary>
        /// <para type="description">The absolute path to the data file to be imported</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string DataFilePath { get; set; }

        [Parameter(Mandatory = false)]
        public string DataMappingFile { get; set; }

        public ImportXrmDataCommand()
        {
        }

        #endregion

        #region Process Record
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            XrmConnectionManager xrmConnection = new XrmConnectionManager(Logger);
            IOrganizationService pollingOrganizationService = xrmConnection.Connect(ConnectionString, 120);

            //Load External Mappings
            DataImportManager dataManager = new DataImportManager(pollingOrganizationService, Logger);
            if (File.Exists(DataMappingFile))
            {
                dataManager.LoadDataMappings(DataMappingFile);
            }

            var importResult = dataManager.ImportFile(DataFilePath);
            if (!importResult.Success)
            {
                throw new Exception(string.Format("Solution import Failed. Error: {0}", importResult.ErrorMessage));
            }

            Logger.LogInformation($"Records Created: {importResult.RecordsCreated}");
            Logger.LogInformation($"Records Updated: {importResult.RecordsUpdated}");
            Logger.LogInformation($"Records Deleted: {importResult.RecordsDeleted}");
            Logger.LogInformation($"Records Skipped: {importResult.RecordsSkipped}");
        }
        #endregion
    }
}