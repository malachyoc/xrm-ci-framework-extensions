using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;

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

        public enum UpdateHintEnum : int
        {
            ChangedFields = 0, //Default
            AllOrNothing = 1,
            AllFields = 2, 
        };

        [JsonProperty("operation")]
        public OperationEnum Operation
        {
            get;set;
        }

        [JsonProperty("updatehint")]
        public UpdateHintEnum UpdateHint
        {
            get; set;
        }
    }
}
