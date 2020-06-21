using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Xrm.Framework.CI.Extensions.SdkMessages
{
    /// <summary>
    /// Entity from JSON File that will be updated in CRM
    /// </summary>
    public class JsonSdkMessage 
    {
        public JsonSdkMessage()
        {

        }

        public enum SdkMessageEnum : int {
            DeleteAttributeRequest = 0,
            SetStateRequest = 1
        };

        [JsonProperty("sdkmessage")]
        public SdkMessageEnum MessageName
        {
            get;set;
        }
    }
}
