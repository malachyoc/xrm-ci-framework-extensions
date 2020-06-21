using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xrm.Framework.CI.Extensions.SdkMessages
{
    /// <summary>
    /// Entity from JSON File that will be updated in CRM
    /// </summary>
    public class JsonSetStateRequest : JsonSdkMessage
    {
        public JsonSetStateRequest()
        {

        }

        [JsonProperty("LogicalName")]
        public string LogicalName
        {
            get; set;
        }

        [JsonProperty("statuscode")]
        public int StatusCode
        {
            get; set;
        }

        [JsonProperty("statecode")]
        public int StateCode
        {
            get; set;
        }

        [JsonProperty("Target")]
        public Guid Target
        {
            get; set;
        }
    }
}
