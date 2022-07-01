using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Xrm.Sdk;
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
    [Cmdlet(VerbsData.Export, "XrmData")]
    public class ExportXrmDataCommand : XrmCommandBase
    {
        private string _fetchXmlPath = String.Empty;
        private string _dataFilePath = String.Empty;
        private string _dataMappingFilePath = String.Empty;
        #region Parameters
        /// <summary>
        /// <para type="description">The absolute path to the data file to be imported</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string DataFilePath
        {
            get
            {
                return _dataFilePath;
            }
            set
            {
                if (Path.IsPathRooted(value))
                {
                    _dataFilePath = value;
                }
                else
                {
                    _dataFilePath = Path.GetFullPath(this.SessionState.Path.CurrentLocation.Path + value);
                }
            }
        }

        [Parameter(Mandatory = false)]
        public string DataMappingFile
        {
            get
            {
                return _dataMappingFilePath;
            }
            set
            {
                if (Path.IsPathRooted(value))
                {
                    _dataMappingFilePath = value;
                }
                else
                {
                    _dataMappingFilePath = Path.GetFullPath(this.SessionState.Path.CurrentLocation.Path + value);
                }
            }
        }

        /// <summary>
        /// <para type="description">The absolute path to the data file to be imported</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string FetchXml { get; set; }

        [Parameter(Mandatory = false)]
        public string FetchXmlPath { 
            get
            {
                return _fetchXmlPath;
            }
            set
            {
                if (Path.IsPathRooted(value))
                {
                    _fetchXmlPath = value;
                }
                else
                {
                    _fetchXmlPath = Path.GetFullPath(this.SessionState.Path.CurrentLocation.Path + value);
                }
            }
        }

        public ExportXrmDataCommand()
        {
        }

        #endregion

        #region Process Record
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            XrmConnectionManager xrmConnection = new XrmConnectionManager(Logger);
            IOrganizationService pollingOrganizationService = xrmConnection.Connect(ConnectionString, 120);

            DataExportManager dataManager = new DataExportManager(pollingOrganizationService, Logger);

            Logger.LogVerbose($"Fetch Path: {_fetchXmlPath}");

            //Load External Mappings
            string fetchXml = FetchXml;
            if (File.Exists(FetchXmlPath))
            {
                Logger.LogVerbose("Retrieving FetchXml");
                fetchXml = File.ReadAllText(FetchXmlPath);
            }

            if(string.IsNullOrWhiteSpace(fetchXml))
            {
                Logger.LogWarning("FetchXML must be provided");
                return;
            }

            //Load External Mappings
            if (File.Exists(DataMappingFile))
            {
                Logger.LogVerbose("Loading Data Mappings");
                dataManager.LoadDataMappings(DataMappingFile);
            }

            Logger.LogVerbose("Exporting Data");
            var exportResult = dataManager.ExportData(fetchXml, DataFilePath);
            if (!exportResult.Success)
            {
                throw new Exception(string.Format("Export Data import failed. Error: {0}", exportResult.ErrorMessage));
            }
        }
        #endregion
    }
}