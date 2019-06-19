using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Xrm.Framework.CI.Common.Logging;

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

        public DataImportManager(IOrganizationService crmService, ILogger logger)
        {
            _crmService = crmService;
            _logger = logger;
            _jsonSerializer = new JsonSerializer();
            _jsonSerializer.Converters.Add(new CrmEntityConverter());
        }
        #endregion

        #region Public Methods
        public DataImportResult ImportData(string importDataPath)
        {
            string fileContent = File.ReadAllText(importDataPath);
            JObject jobject = JsonParser.ParseCrmData(fileContent);

            return ProcessDataFile(jobject);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Cycle through all entities in the JSON file and perform specified operation
        /// </summary>
        /// <param name="crmData"></param>
        private DataImportResult ProcessDataFile(JObject crmData)
        {
            DataImportResult importResult = new DataImportResult();

            //Retrieve all entities 
            IList<JToken> entities = crmData.SelectToken("entities").ToList();
            foreach (var jsonEntity in entities)
            {
                //Process the entities sequencially
                var crmEntity = jsonEntity.ToObject<JsonEntity>(_jsonSerializer);

                //Will look at using bulk updates in future
                switch (crmEntity.Operation)
                {
                    case JsonEntity.OperationEnum.Delete:
                        DeleteEntity(crmEntity);
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
                            var delta = existingEntity.Delta(crmEntity);
                            UpdateEntity(crmEntity, delta, importResult);
                            break;
                        }
                }
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

                    default:
                        throw;
                }
            }
        }

        private void UpdateEntity(JsonEntity crmEntity, Entity delta, DataImportResult result = null)
        {
            _logger.LogVerbose($"Updating {delta.LogicalName} entity with Id: '{delta.Id}'");
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
                        _logger.LogWarning($"Could not update {fieldsToUpdate.LogicalName} entity with Id: '{fieldsToUpdate.Id}'. Object does not exist.");
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
            _logger.LogInformation($"Retreiving {crmEntity.LogicalName} entity with id: '{crmEntity.Id}'");

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
