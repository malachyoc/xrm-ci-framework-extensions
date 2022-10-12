using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Xrm.Framework.CI.Extensions.DataOperations
{
    /// <summary>
    /// Entity from JSON File that will be updated in CRM
    /// </summary>
    public class JsonEntity : Entity
    {
        public JsonEntity()
        {

        }

        public JsonEntity(Entity entityToClone)
        {
            this.LogicalName = entityToClone.LogicalName;
            this.Id = entityToClone.Id;

            this.Attributes = new AttributeCollection();
            foreach (var attribute in entityToClone.Attributes.OrderBy(o => o.Key))
            {
                //TODO: Still need to clone value
                this.Attributes.Add(attribute.Key, attribute.Value);
            }
        }

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
