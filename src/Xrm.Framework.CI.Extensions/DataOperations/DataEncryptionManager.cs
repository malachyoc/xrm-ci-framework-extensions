using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Xrm.Framework.CI.Common.Logging;
using Xrm.Framework.CI.Extensions.Managers;
using Xrm.Framework.CI.Extensions.SdkMessages;

namespace Xrm.Framework.CI.Extensions.DataOperations
{ 
    public class DataEncryptionManager
    {
        #region Internal Classes
        public class DataEncryptionResult
        {
            #region Properties
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            #endregion

            #region Constructor
            public DataEncryptionResult()
            {

            }
            #endregion
        }
        #endregion

        #region Member Variables and Constructors
        private IOrganizationService _crmService;
        private ILogger _logger;

        public DataEncryptionManager(IOrganizationService crmService, ILogger logger)
        {
            _crmService = crmService;
            _logger = logger;

            _logger.LogInformation($"Connected to: {this.ConnectionDetails}");
        }
        #endregion

        #region Public Methods
        public DataEncryptionResult SetEncryptionKey(string dataEncryptionKey)
        {
            try
            {
                IsDataEncryptionActiveRequest checkRequest = new IsDataEncryptionActiveRequest();
                IsDataEncryptionActiveResponse checkResponse = (IsDataEncryptionActiveResponse)_crmService.Execute(checkRequest);

                if (!checkResponse.IsActive)
                {
                    SetDataEncryptionKeyRequest dekRequest = new SetDataEncryptionKeyRequest();
                    dekRequest.ChangeEncryptionKey = false;
                    dekRequest.EncryptionKey = dataEncryptionKey;

                    var result = _crmService.Execute(dekRequest);
                    return new DataEncryptionResult() { Success = true, ErrorMessage = "Encryption Activated" };
                }
                else
                {
                    return new DataEncryptionResult() { Success = true, ErrorMessage = "Encryption is already active." };
                }
            }
            catch(Exception ex)
            {
                _logger.LogWarning("Exception of type: {0} occurred", ex.GetType().Name);
                return new DataEncryptionResult() { Success = false, ErrorMessage = ex.Message };
            }
        }
        #endregion

        #region Private Methods
        string _connectionDetails;
        private string ConnectionDetails
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_connectionDetails))
                {
                    var response = _crmService.RetrieveMultiple(new QueryExpression("organization")
                    {
                        NoLock = true
                        , ColumnSet = new ColumnSet(new string[] { "name" })
                    });

                    _connectionDetails = (string)response.Entities.First().Attributes["name"];
                }
                return _connectionDetails;
            }
        }
        #endregion
    }
}
