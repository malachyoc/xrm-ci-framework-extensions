using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;

namespace Xrm.Framework.CI.Extensions.DataOperations
{
    /// <summary>
    /// Wrapper class that parses a CRM Data File
    /// </summary>
    public static class JsonParser
    {
        public const string JSON_CRMDATA_SCHEMA = "Xrm.Framework.CI.Extensions.Schema.crmdata.schema.json";

        /// <summary>
        /// Return JSON Schema from assembly
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        private static string GetJsonSchema(string resourceName)
        {
            var assembly = typeof(JsonParser).GetTypeInfo().Assembly;
            string result = string.Empty;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// Validate the content against the schema
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <param name="jsonSchema"></param>
        /// <param name="validationErrors"></param>
        /// <returns></returns>
        public static bool ValidateJson(string jsonContent, string jsonSchema, out string validationErrors)
        {
            bool valid = false;
            validationErrors = String.Empty;

            if (!String.IsNullOrEmpty(jsonContent) && !String.IsNullOrEmpty(jsonSchema))
            {
                JSchema schema = JSchema.Parse(jsonSchema);
                JObject jsonFile = JObject.Parse(jsonContent);

                IList<String> validationMessages = new List<string>();
                valid = jsonFile.IsValid(schema, out validationMessages);
                if (!valid)
                {
                    validationErrors = $"The following validation errors occurred:\n{ String.Join("\n", validationMessages) }";
                }
            }
            else
            {
                validationErrors = "Json or Schema not provided.";
            }

            return valid;
        }

        public static bool ValidateJson(string jsonContent, out string validationErrors)
        {
            var jsonSchema = GetJsonSchema(JSON_CRMDATA_SCHEMA);
            return ValidateJson(jsonContent, jsonSchema, out validationErrors);
        }

        public static JObject ParseCrmData(string fileContents)
        {
            if (String.IsNullOrWhiteSpace(fileContents))
                throw new Exception("DataFile contents cannot be empty.");

            string jsonSchema = GetJsonSchema(JSON_CRMDATA_SCHEMA);
            if (String.IsNullOrWhiteSpace(jsonSchema))
                throw new Exception($"Failed to retrieve schema: { JSON_CRMDATA_SCHEMA }");

            string jsonValidationError = String.Empty;
            if (ValidateJson(fileContents, jsonSchema, out jsonValidationError) == false)
                throw new Exception($"Failed to vaildate DataFile against schema:\n{ jsonValidationError }");

            return JObject.Parse(fileContents);
        }
    }
}
