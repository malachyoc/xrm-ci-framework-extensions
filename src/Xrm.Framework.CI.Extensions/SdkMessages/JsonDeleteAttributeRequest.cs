using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Xrm.Framework.CI.Extensions.SdkMessages
{
    /// <summary>
    /// Entity from JSON File that will be updated in CRM
    /// </summary>
    public class JsonDeleteAttributeRequest : JsonSdkMessage
    {
        public JsonDeleteAttributeRequest()
        {

        }

        [JsonProperty("EntityLogicalName")]
        public string EntityLogicalName
        {
            get;set;
        }

        [JsonProperty("LogicalName")]
        public string LogicalName
        {
            get; set;
        }
    }
}
