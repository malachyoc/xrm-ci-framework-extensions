using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm.Framework.CI.Common.Logging;

namespace Xrm.Framework.CI.Extensions.DataOperations
{
    public class DataMapper
    {
        public class DataMapping
        {
            public String EntityLogicalName;
            public String SourceQuery;
            public String TargetQuery;
            public Guid? SourceId;
            public Guid? TargetId;
        }

        private IOrganizationService _crmService;
        private ILogger _logger;
        private MetadataManager _metadataManager;

        Dictionary<string, Dictionary<Guid, Guid>> _dataMappings;

        public DataMapper(IOrganizationService crmService, MetadataManager metadataManager, ILogger logger)
        {
            _crmService = crmService;
            _logger = logger;

            _dataMappings = new Dictionary<string, Dictionary<Guid, Guid>>();
            _metadataManager = metadataManager;
        }

        public void LoadMappings(JObject crmData)
        {
            //TODO: if a mapping fails just skip and log warning

            //Retrieve all entities 
            IList<JToken> entities = crmData.SelectToken("mappings").ToList();
            foreach (var jsonEntity in entities)
            {
                //Process the entities sequencially
                var mapping = jsonEntity.ToObject<DataMapping>();

                //Check if we need to dynamically load SourceId
                if(mapping.SourceId == null)
                {
                    //Resolve the source id from the fetch query
                    var retrieveMultipleRequest = new RetrieveMultipleRequest();
                    retrieveMultipleRequest.Query = new FetchExpression(mapping.SourceQuery.Trim());

                    RetrieveMultipleResponse response = (RetrieveMultipleResponse)_crmService.Execute(retrieveMultipleRequest);
                    var match = response.EntityCollection.Entities.FirstOrDefault();

                    if(match != null)
                    {
                        var sourceId = (AliasedValue)match.Attributes["id"];
                        mapping.SourceId = (Guid)sourceId.Value;
                    }
                }

                //Check if we need to dynamically load TargetId
                if (mapping.TargetId == null)
                {
                    //Resolve the source id from the fetch query
                    var retrieveMultipleRequest = new RetrieveMultipleRequest();
                    retrieveMultipleRequest.Query = new FetchExpression(mapping.TargetQuery.Trim());

                    RetrieveMultipleResponse response = (RetrieveMultipleResponse)_crmService.Execute(retrieveMultipleRequest);
                    var match = response.EntityCollection.Entities.FirstOrDefault();

                    if (match != null)
                    {
                        var targetId = (AliasedValue)match.Attributes["id"];
                        mapping.TargetId= (Guid)targetId.Value;
                    }
                }

                if (!String.IsNullOrWhiteSpace(mapping.EntityLogicalName)
                    && mapping.SourceId != null
                    && mapping.TargetId != null)
                {
                    Dictionary<Guid, Guid> entityDictionary;
                    if(_dataMappings.ContainsKey(mapping.EntityLogicalName))
                    {
                        entityDictionary = _dataMappings[mapping.EntityLogicalName];
                    }
                    else
                    {
                        entityDictionary = new Dictionary<Guid, Guid>();
                        _dataMappings.Add(mapping.EntityLogicalName, entityDictionary);
                    }

                    entityDictionary.Add(mapping.SourceId.Value, mapping.TargetId.Value);
                }
            }
        }

        //Update any guids & Entity references with their mapped equivelent
        public void ProcessEntity(JsonEntity entity)
        {
            Guid? newPk = GetMappedValue(entity.LogicalName, entity.Id);
            String recordKey = String.Empty;

            var metadata = _metadataManager.RetrieveEntityMetadata(entity.LogicalName);

            if (metadata.IsIntersect == false)
            {
                //Check if we need to map any attribures
                foreach (KeyValuePair<string, object> attribute in entity.Attributes)
                {
                    if (attribute.Value.GetType() == typeof(Guid))
                    {
                        //Check if attribute is PK
                        if (attribute.Key == metadata.PrimaryIdAttribute && newPk != null)
                        {
                            recordKey = attribute.Key;
                        }
                    }

                    //If the field is an entity reference it may need to be mapped
                    else if (attribute.Value.GetType() == typeof(EntityReference))
                    {
                        var entityReference = (EntityReference)attribute.Value;
                        Guid? newReferenceValue = GetMappedValue(entityReference.LogicalName, entityReference.Id);

                        if (newReferenceValue != null)
                        {
                            entityReference.Id = newReferenceValue.Value;
                        }
                    }
                }

                //Update the Pk if requried
                if (newPk != null)
                {
                    entity.Id = newPk.Value;

                    if (!String.IsNullOrWhiteSpace(recordKey))
                        entity.Attributes[recordKey] = newPk.Value;
                }
            }
            else
            {
                //For Many-to-many check if target attributes need to be mapped
                var manyToManyRelationship = metadata.ManyToManyRelationships.FirstOrDefault();

                if (entity.Attributes.ContainsKey(manyToManyRelationship.Entity1IntersectAttribute))
                {
                    Guid sourceKey = (Guid)entity.Attributes[manyToManyRelationship.Entity1IntersectAttribute];
                    Guid? mappedGuid = GetMappedValue(manyToManyRelationship.Entity1LogicalName, sourceKey);
                    if (mappedGuid != null)
                    {
                        entity.Attributes[manyToManyRelationship.Entity1IntersectAttribute] = mappedGuid.Value;
                    }
                }

                if (entity.Attributes.ContainsKey(manyToManyRelationship.Entity2IntersectAttribute))
                {
                    Guid sourceKey = (Guid)entity.Attributes[manyToManyRelationship.Entity2IntersectAttribute];
                    Guid? mappedGuid = GetMappedValue(manyToManyRelationship.Entity2LogicalName, sourceKey);
                    if (mappedGuid != null)
                    {
                        entity.Attributes[manyToManyRelationship.Entity2IntersectAttribute] = mappedGuid.Value;
                    }
                }
            }
        }

        private Guid? GetMappedValue(string entityLogicalName, Guid guidValue)
        {
            if (_dataMappings.ContainsKey(entityLogicalName))
            {
                Dictionary<Guid, Guid> entityDictionary = _dataMappings[entityLogicalName];
                if (entityDictionary.ContainsKey(guidValue))
                {
                    return entityDictionary[guidValue];
                }
            }

            //No match
            return null;
        }
    }
}
