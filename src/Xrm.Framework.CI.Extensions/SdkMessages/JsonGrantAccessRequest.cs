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
    public class JsonGrantAccessRequest : JsonSdkMessage
    {
        public JsonGrantAccessRequest()
        {

        }

        [JsonProperty("Target")]
        public EntityReference Target
        {
            get; set;
        }

        [JsonProperty("Principal")]
        public EntityReference Principal
        {
            get; set;
        }

        [JsonProperty("ReadAccess")]
        public bool ReadAccess
        {
            get; set;
        }

        [JsonProperty("WriteAccess")]
        public bool WriteAccess
        {
            get; set;
        }

        [JsonProperty("DeleteAccess")]
        public bool DeleteAccess
        {
            get; set;
        }

        [JsonProperty("AssignAccess")]
        public bool AssignAccess
        {
            get; set;
        }

        [JsonProperty("AppendAccess")]
        public bool AppendAccess
        {
            get; set;
        }

        [JsonProperty("AppendToAccess")]
        public bool AppendToAccess
        {
            get; set;
        }

        [JsonProperty("ShareAccess")]
        public bool ShareAccess
        {
            get; set;
        }
    }
}
