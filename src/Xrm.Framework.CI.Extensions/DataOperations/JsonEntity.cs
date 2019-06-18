using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrm.Framework.CI.Extensions.DataOperations
{
    /// <summary>
    /// Entity from JSON File that will be updated in CRM
    /// </summary>
    public class JsonEntity : Entity
    {
        public enum OperationEnum: int {
            Upsert = 0, //Default
            Create = 1,
            Update = 2,
            Delete = 3
        };

        [JsonProperty("operation")]
        public OperationEnum Operation
        {
            get;set;
        }
    }
}
