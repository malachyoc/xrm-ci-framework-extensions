﻿using Microsoft.Xrm.Sdk;
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

namespace Xrm.Framework.CI.Extensions.DataOperations
{ 
    public class DataExportManager
    {
        #region Internal Classes
        public class DataExportResult
        {
            #region Properties
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public int RecordsExported { get; set; }
            #endregion

            #region Constructor
            public DataExportResult()
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
        
        public DataExportManager(IOrganizationService crmService, ILogger logger)
        {
            _crmService = crmService;
            _logger = logger;
            _jsonSerializer = new JsonSerializer();
            _jsonSerializer.Converters.Add(new CrmEntityConverter());
            _dataMapper = new DataMapper(crmService, logger);
        }
        #endregion

        #region Public Methods
        public void LoadDataMappings(string filePath)
        {
            string fileContent = File.ReadAllText(filePath);
            JObject jobject = JsonParser.ParseCrmData(fileContent);

            _dataMapper.LoadMappings(jobject);
        }

        public DataExportResult ExportData(string fetchQuery, string filePath)
        {
            using (StreamWriter outputStream = new StreamWriter(filePath, false))
            {
                return ExportToStream(fetchQuery, outputStream);
            }
        }

        /// <summary>
        /// If the import is really large this will cause problems
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public string ExportJsonString(string fetchQuery)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(stream);
                ExportToStream(fetchQuery, writer);

                // convert stream to string
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        private DataExportResult ExportToStream(string fetchQuery, StreamWriter outputStream)
        {
            JsonTextWriter writer = new JsonTextWriter(outputStream);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            writer.WritePropertyName("schema");
            writer.WriteValue("https://github.com/malachyoc/xrm-ci-framework-extensions/_schema/crmdata.schema.json");
            writer.WritePropertyName("schemaFormat");
            writer.WriteValue("http://json-schema.org/draft-07/schema#");
            writer.WritePropertyName("schemaVersion");
            writer.WriteValue("1-0-0");

            //Execute the fetch query
            var retrieveMultipleRequest = new RetrieveMultipleRequest();
            retrieveMultipleRequest.Query = new FetchExpression(fetchQuery);

            RetrieveMultipleResponse queryResponse = (RetrieveMultipleResponse)_crmService.Execute(retrieveMultipleRequest);

            DataExportResult results = new DataExportResult();
            writer.WritePropertyName("entities");
            writer.WriteStartArray();

            //Step 2:Page through
            do
            {
                List<JsonEntity> additionalEntities = 
                    queryResponse.EntityCollection.Entities
                        .Select(r => this.ParseEntity(r))
                        .ToList();

                results.RecordsExported += additionalEntities.Count();

                foreach (var entity in additionalEntities)
                {
                    _jsonSerializer.Serialize(writer, entity);
                }
                writer.Flush();

            }
            while (queryResponse.EntityCollection.MoreRecords == true);
            writer.WriteEndArray();
            writer.WriteEndObject();

            return results;

        }
        #endregion

        private JsonEntity ParseEntity(Entity entity)
        {
            //1. Parse Entity
            JsonEntity jsonEntity = new JsonEntity(entity);

            //2. Process the entity
            _dataMapper.ProcessEntity(jsonEntity);

            return jsonEntity;
        }
    }
}
