using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm.Framework.CI.Common.Logging;

namespace Xrm.Framework.CI.Extensions.DataOperations
{
    public class MetadataManager
    {
        #region Member Variables and Constructors
        private IOrganizationService _crmService;
        private ILogger _logger;
        private Dictionary<string, EntityMetadata> _metadata;

        public MetadataManager(IOrganizationService crmService, ILogger logger)
        {
            _crmService = crmService;
            _logger = logger;
            _metadata = new Dictionary<string, EntityMetadata>();
        }

        public EntityMetadata RetrieveEntityMetadata(string logicalName)
        {
            if (!_metadata.ContainsKey(logicalName))
            {
                RetrieveEntityResponse response = (RetrieveEntityResponse)_crmService.Execute(new RetrieveEntityRequest()
                {
                    LogicalName = logicalName
                    , EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.All
                });

                _metadata.Add(logicalName, response.EntityMetadata);
            }
            return _metadata[logicalName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void ProcessEntity(JsonEntity entity)
        {
            var metadata = this.RetrieveEntityMetadata(entity.LogicalName);
            if (metadata == null)
                return;
            
            if(metadata.IsIntersect == true)
            {
                //Remove Primary attribute for many-to-many
                if (entity.Attributes.Contains(metadata.PrimaryIdAttribute))
                    entity.Attributes.Remove(metadata.PrimaryIdAttribute);

                //Set Id to Empty
                entity.Id = Guid.Empty;
                entity.Operation = JsonEntity.OperationEnum.Associate;
            }
        }
        #endregion
    }
}
