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
    public class DataImportManager
    {
        #region Internal Classes
        public class DataImportResult
        {
            #region Properties
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public int RecordsCreated { get; set; }
            public int RecordsUpdated { get; set; }
            public int RecordsDeleted { get; set; }
            public int RecordsSkipped { get; set; }
            public int RecordsFailed { get; set; }
            #endregion

            #region Constructor
            public DataImportResult()
            {

            }
            #endregion
        }
        #endregion

        #region Member Variables and Constructors
        private IOrganizationService _crmService;
        private ILogger _logger;

        private JsonSerializer _jsonSerializer;
        private DataMapper _dataMapper;
        private MetadataManager _metadataManager;
        private RolePrivilegeManager _rolePrivilegeManager;

        public DataImportManager(IOrganizationService crmService, ILogger logger)
        {
            _crmService = crmService;
            _logger = logger;

            _jsonSerializer = new JsonSerializer();
            _jsonSerializer.Converters.Add(new CrmEntityConverter());
            _metadataManager = new MetadataManager(crmService, logger);
            _dataMapper = new DataMapper(crmService, _metadataManager, logger);
            _rolePrivilegeManager = new RolePrivilegeManager(crmService, logger);
            _logger.LogInformation($"Connected to: {this.ConnectionDetails}");
        }
        #endregion

        #region Public Methods
        public void LoadDataMappings(string filePath)
        {
            string fileContent = File.ReadAllText(filePath);
            JObject jobject = JsonParser.ParseCrmData(fileContent);

            _dataMapper.LoadMappings(jobject);
        }

        public DataImportResult ImportFile(string importDataPath)
        {
            string fileContent = File.ReadAllText(importDataPath);
            JObject jobject = JsonParser.ParseCrmData(fileContent);

            return ProcessDataFile(jobject);
        }

        public DataImportResult ImportJson(string jsonData)
        {
            JObject jobject = JsonParser.ParseCrmData(jsonData);
            return ProcessDataFile(jobject);
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

        /// <summary>
        /// Cycle through all entities in the JSON file and perform specified operation
        /// </summary>
        /// <param name="crmData"></param>
        private DataImportResult ProcessDataFile(JObject crmData)
        {
            DataImportResult importResult = new DataImportResult()
            {
                Success = true
            };

            //Retrieve all entities 
            IList<JToken> entities = crmData.SelectToken("entities").ToList();
            foreach (var jsonEntity in entities)
            {
                if (jsonEntity.SelectToken("SdkMessage") != null)
                {
                    //Process the entities sequencially
                    var sdkMessage = jsonEntity.ToObject<JsonSdkMessage>(_jsonSerializer);

                    //Will look at using bulk updates in future
                    switch (sdkMessage.MessageName)
                    {
                        case JsonSdkMessage.SdkMessageEnum.DeleteAttributeRequest:
                            {
                                var deleteAttributeRequest = jsonEntity.ToObject<JsonDeleteAttributeRequest>(_jsonSerializer);
                                DeleteAttributeRequest(deleteAttributeRequest, importResult);
                                break;
                            }

                        case JsonSdkMessage.SdkMessageEnum.SetStateRequest:
                            {
                                var setStateRequest = jsonEntity.ToObject<JsonSetStateRequest>(_jsonSerializer);
                                SetStateRequest(setStateRequest, importResult);
                                break;
                            }

                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    //Process the entities sequencially
                    var crmEntity = jsonEntity.ToObject<JsonEntity>(_jsonSerializer);
                    _dataMapper.ProcessEntity(crmEntity);

                    //Will look at using bulk updates in future
                    switch (crmEntity.Operation)
                    {
                        case JsonEntity.OperationEnum.Delete:
                            {
                                var existingEntity = RetrieveEntity(crmEntity);
                                if (existingEntity != null)
                                    DeleteEntity(crmEntity, importResult);
                                else
                                    importResult.RecordsSkipped++;
                            }
                            break;

                        case JsonEntity.OperationEnum.Upsert:
                            {
                                //If the record exists update, otherwise create
                                var existingEntity = RetrieveEntity(crmEntity);
                                if (existingEntity != null)
                                {
                                    //only update if fields have changed
                                    var delta = existingEntity.Delta(crmEntity);
                                    UpdateEntity(crmEntity, delta, importResult);
                                }
                                else
                                {
                                    CreateEntity(crmEntity, importResult);
                                }
                            }
                            break;

                        case JsonEntity.OperationEnum.Create:
                            CreateEntity(crmEntity, importResult);
                            break;

                        case JsonEntity.OperationEnum.Update:
                            {
                                //Calculate the Update Delta
                                var existingEntity = RetrieveEntity(crmEntity);

                                //TODO: Check for null
                                var delta = existingEntity.Delta(crmEntity);
                                UpdateEntity(crmEntity, delta, importResult);
                                break;
                            }

                        case JsonEntity.OperationEnum.AddPrivilege:
                            AddPrivilege(crmEntity, importResult);
                            break;

                        case JsonEntity.OperationEnum.Associate:
                            AssociateEntity(crmEntity, importResult);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            //Check for unsubmitted Add Privilege Request
            if(_rolePrivilegeManager.UnsubmittedRequests == true)
            {
                _rolePrivilegeManager.SubmitAddPrivilegesRequests(importResult);
            }

            return importResult;
        }

        private void DeleteEntity(Entity crmEntity, DataImportResult result = null)
        {
            _logger.LogVerbose($"Deleting {crmEntity.LogicalName} entity with Id: '{crmEntity.Id}'");

            try
            {
                _crmService.Delete(crmEntity.LogicalName, crmEntity.Id);
                if (result != null)
                {
                    result.RecordsDeleted++;
                }
            }
            catch(FaultException<OrganizationServiceFault> fex)
            {
                switch((uint)fex.Detail.ErrorCode)
                {
                    //Continue if object does not exist
                    //TODO: Behavior should be configurable
                    case (uint)0x80040217:
                        _logger.LogWarning($"Could not delete {crmEntity.LogicalName} entity with Id: '{crmEntity.Id}'. Object does not exist.");
                        if (result != null)
                        {
                            result.RecordsSkipped++;
                        }
                        break;

                    case (uint)0x8004500F:
                        _logger.LogWarning($"Could not delete {crmEntity.LogicalName} entity with Id: '{crmEntity.Id}'. Workflow is Active.");
                        if (result != null)
                        {
                            result.RecordsFailed++;
                        }
                        break;

                    default:
                        throw;
                }
            }
        }

        private void UpdateEntity(JsonEntity crmEntity, Entity delta, DataImportResult result = null)
        {
            
            Entity fieldsToUpdate = null;
            switch (crmEntity.UpdateHint)
            {
                //Update all the fields regardless of whether or not they have changed
                case JsonEntity.UpdateHintEnum.AllFields:
                    fieldsToUpdate = crmEntity.ToEntity<Entity>();
                    break;

                //If there is a change update all fields; otherwise update none
                case JsonEntity.UpdateHintEnum.AllOrNothing:
                    fieldsToUpdate = delta.Attributes.Count == 0 ? delta : crmEntity.ToEntity<Entity>();
                    break;

                //Only update fields that have changed
                case JsonEntity.UpdateHintEnum.ChangedFields:
                    fieldsToUpdate = delta;
                    break;
            }

            try
            {
                if (fieldsToUpdate.Attributes.Count > 0)
                {
                    _logger.LogVerbose($"Updating {delta.LogicalName} entity with Id: '{delta.Id}'");
                    _crmService.Update(fieldsToUpdate);
                    if (result != null)
                    {   
                        result.RecordsUpdated++;
                    }
                }
                else
                {
                    result.RecordsSkipped++;
                    _logger.LogVerbose($"{fieldsToUpdate.LogicalName} entity with Id: '{fieldsToUpdate.Id} not updated'. No Attribute changes.");
                }
            }
            catch (FaultException<OrganizationServiceFault> fex)
            {
                switch ((uint)fex.Detail.ErrorCode)
                {
                    //Continue if object does not exist
                    //TODO: Behavior should be configurable
                    case (uint)0x80040217:
                        _logger.LogWarning($"Could not update {fieldsToUpdate.LogicalName} entity with Id: '{fieldsToUpdate.Id}'. {fex.Message}");
                        if (result != null)
                        {
                            result.RecordsSkipped++;
                        }
                        break;

                    case (uint)0x80044150:
                        _logger.LogError($"Could not update {fieldsToUpdate.LogicalName} entity with Id: '{fieldsToUpdate.Id}'. SQL Error");
                        _logger.LogVerbose(fex.Detail.TraceText);
                        if (result != null)
                        {
                            result.RecordsFailed++;
                        }
                        break;

                    case (uint)0x8004830A:
                        _logger.LogError($"Could not update {fieldsToUpdate.LogicalName} entity with Id: '{fieldsToUpdate.Id}'. {fex.Message}");
                        if (result != null)
                        {
                            result.RecordsFailed++;
                        }
                        break;


                    default:
                        throw;
                }
            }
        }

        private void CreateEntity(Entity crmEntity, DataImportResult result = null)
        {
            _logger.LogVerbose($"Creating {crmEntity.LogicalName} entity with Id: '{crmEntity.Id}'");

            try
            {
                _crmService.Create(crmEntity.ToEntity<Entity>());
                if (result != null)
                {
                    result.RecordsCreated++;
                }
            }
            catch (FaultException<OrganizationServiceFault> fex)
            {
                switch ((uint)fex.Detail.ErrorCode)
                {
                    //Continue if key is duplicate
                    //TODO: Behavior should be configurable
                    case (uint)0x80040237:
                        _logger.LogWarning($"Could not create {crmEntity.LogicalName} entity.  Record with id: '{crmEntity.Id}' already exists.");
                        if (result != null)
                        {
                            result.RecordsSkipped++;
                        }
                        break;

                    case (uint)0x8004022A:
                        _logger.LogError($"Could not create {crmEntity.LogicalName} entity. {fex.Message}");
                        if (result != null)
                        {
                            result.RecordsFailed++;
                        }
                        break;

                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Hardcoded 
        /// </summary>
        /// <param name="crmEntity"></param>
        /// <param name="result"></param>
        private void AddPrivilege(JsonEntity crmEntity, DataImportResult result = null)
        {
            try
            {
                //Stage the privilege
                _rolePrivilegeManager.StagePrivilege(crmEntity);
            }
            catch (FaultException<OrganizationServiceFault> fex)
            {
                switch ((uint)fex.Detail.ErrorCode)
                {
                    //Continue if key is duplicate
                    //TODO: Behavior should be configurable
                    case (uint)0x80040237:
                        _logger.LogWarning($"Could not create {crmEntity.LogicalName} entity.  Relationship already exists.");
                        if (result != null)
                        {
                            result.RecordsSkipped++;
                        }
                        break;

                    case (uint)0x80040217:
                        _logger.LogWarning($"Failed to create {crmEntity.LogicalName} entity.  {fex.Detail.Message}");
                        if (result != null)
                        {
                            result.RecordsSkipped++;
                        }
                        break;

                    default:
                        throw;
                }
            }
        }

        private void AssociateEntity(JsonEntity crmEntity, DataImportResult result = null)
        {
            try
            {
                var metadata = _metadataManager.RetrieveEntityMetadata(crmEntity.LogicalName);
                var manyToManyRelationship = metadata.ManyToManyRelationships.FirstOrDefault();

                var entity1Reference = new EntityReference(manyToManyRelationship.Entity1LogicalName, (Guid)crmEntity[manyToManyRelationship.Entity1IntersectAttribute]);
                var entity2Reference = new EntityReference(manyToManyRelationship.Entity2LogicalName, (Guid)crmEntity[manyToManyRelationship.Entity2IntersectAttribute]);

                // Use AssociateRequest
                AssociateRequest request = new AssociateRequest()
                {
                    RelatedEntities = new EntityReferenceCollection(new List<EntityReference>(new EntityReference[]{ entity1Reference })),
                    Relationship = new Relationship(manyToManyRelationship.SchemaName),
                    Target = entity2Reference
                };

                _crmService.Execute(request);
                if (result != null)
                {
                    result.RecordsCreated++;
                }
            }
            catch (FaultException<OrganizationServiceFault> fex)
            {
                switch ((uint)fex.Detail.ErrorCode)
                {
                    //Continue if key is duplicate
                    //TODO: Behavior should be configurable
                    case (uint)0x80040237:
                        _logger.LogWarning($"Could not create {crmEntity.LogicalName} entity.  Relationship already exists.");
                        if (result != null)
                        {
                            result.RecordsSkipped++;
                        }
                        break;

                    case (uint)0x80040217:
                        _logger.LogWarning($"Failed to create {crmEntity.LogicalName} entity.  {fex.Detail.Message}");
                        if (result != null)
                        {
                            result.RecordsSkipped++;
                        }
                        break;

                    default:
                        throw;
                }
            }
        }

        private void DeleteAttributeRequest(JsonDeleteAttributeRequest sdkMessage, DataImportResult result = null)
        {
            try
            {
                DeleteAttributeRequest request = new DeleteAttributeRequest();
                request.EntityLogicalName = sdkMessage.EntityLogicalName;
                request.LogicalName = sdkMessage.LogicalName;

                var response = _crmService.Execute(request);

                if (result != null)
                {
                    result.RecordsCreated++;
                }
            }
            catch (FaultException<OrganizationServiceFault> fex)
            {
                switch ((uint)fex.Detail.ErrorCode)
                {
                    //Continue if key is duplicate
                    //TODO: Behavior should be configurable
                    case (uint)0x80040217:
                        if (result != null)
                        {
                            result.RecordsSkipped++;
                        }
                        break;

                    default:
                        throw;
                }
            }
        }

        private void SetStateRequest(JsonSetStateRequest sdkMessage, DataImportResult result = null)
        {
            try
            {
                SetStateRequest request = new SetStateRequest();
                request.EntityMoniker = new EntityReference(sdkMessage.LogicalName, sdkMessage.Target);
                request.Status = new OptionSetValue(sdkMessage.StatusCode);
                request.State = new OptionSetValue(sdkMessage.StateCode);

                var response = _crmService.Execute(request);

                if (result != null)
                {
                    result.RecordsCreated++;
                }
            }
            catch (FaultException<OrganizationServiceFault> fex)
            {
                switch ((uint)fex.Detail.ErrorCode)
                {
                    //Continue if key is duplicate
                    //TODO: Behavior should be configurable
                    case (uint)0x80040217:
                        _logger.LogWarning($"Failed to Set State on {sdkMessage.LogicalName}.  {fex.Detail.Message}");
                        if (result != null)
                        {
                            result.RecordsSkipped++;
                        }
                        break;

                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Retrieve the named entity
        /// </summary>
        /// <param name="crmEntity"></param>
        /// <returns></returns>
        private Entity RetrieveEntity(Entity crmEntity)
        {
            //_logger.LogVerbose($"Retrieving {crmEntity.LogicalName} entity with id: '{crmEntity.Id}'");

            try
            {
                //Get the current value of the attributes to be set
                string[] columns = crmEntity.Attributes == null
                    ? new string[0]
                    : crmEntity.Attributes.Select(a => a.Key).ToArray();

                return _crmService.Retrieve(crmEntity.LogicalName, crmEntity.Id
                    , new Microsoft.Xrm.Sdk.Query.ColumnSet(columns));
            }
            catch (FaultException<OrganizationServiceFault> fex)
            {
                switch ((uint)fex.Detail.ErrorCode)
                {
                    //Continue if object does not exist
                    case (uint)0x80040217:
                        return null;

                    default:
                        throw;
                }
            }
        }
        #endregion
    }
}
