using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using static Xrm.Framework.CI.Extensions.DataOperations.JsonEntity;

namespace Xrm.Framework.CI.Extensions.DataOperations
{
    /// <summary>
    /// Created customised serialiser so we can use a cleaner JSON Structure.
    /// </summary>
    public class CrmEntityConverter : JsonConverter
    {
        Type[] supportedTypes = new Type[] { typeof(Entity), typeof(JsonEntity) };

        /// <summary>
        /// Customized Write to include type information and omit redundant information
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Entity entity = (Entity)value;
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(entity.LogicalName));
            writer.WriteValue(entity.LogicalName);
            writer.WritePropertyName(nameof(entity.Id));
            writer.WriteValue(entity.Id);

            writer.WritePropertyName(nameof(entity.Attributes));
            writer.WriteStartArray();

            foreach (var item in entity.Attributes)
            {
                serializer.Serialize(writer, JsonAttribute.Create(item));
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JsonEntity deserialisedEntity = null;
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                // Load JObject from stream
                deserialisedEntity = new JsonEntity();

                JObject jObject = JObject.Load(reader);
                deserialisedEntity.LogicalName = (string)jObject.GetValue(nameof(Entity.LogicalName));
                deserialisedEntity.Id = (Guid)jObject.GetValue(nameof(Entity.Id));
                if(jObject.ContainsKey(nameof(JsonEntity.Operation)))
                {
                    deserialisedEntity.Operation = jObject.GetValue(nameof(JsonEntity.Operation)).ToObject<OperationEnum>();
                }

                if(jObject.ContainsKey(nameof(JsonEntity.Attributes)))
                {
                    IList<JToken> attributes = jObject.SelectToken(nameof(JsonEntity.Attributes)).ToList();
                    
                    foreach(JObject attribute in attributes)
                    {
                        string attributeType = attribute.Property(nameof(JsonAttribute.Type)).ToObject<String>();
                        string attributeName = attribute.Property(nameof(JsonAttribute.LogicalName)).ToObject<String>();
                        JToken attributeValue = attribute.Property(nameof(JsonAttribute.Value)).Value;

                        KeyValuePair<String, Object> newAttribute = new KeyValuePair<string, object>();

                        switch (attributeType)
                        {
                            case null:
                                newAttribute = new KeyValuePair<string, object>(attributeName, null);
                                break;

                            case nameof(String):
                                newAttribute = new KeyValuePair<string, object>(attributeName, attributeValue.ToObject<String>());
                                break;

                            case nameof(Boolean):
                                newAttribute = new KeyValuePair<string, object>(attributeName, attributeValue.ToObject<Boolean>());
                                break;

                            case nameof(Int32):
                                newAttribute = new KeyValuePair<string, object>(attributeName, attributeValue.ToObject<Int32>());
                                break;

                            case nameof(OptionSetValue):
                                //Create new Optionset Value
                                newAttribute = new KeyValuePair<string, object>(attributeName, new OptionSetValue(attributeValue.ToObject<Int32>()));
                                break;

                            case nameof(Decimal):
                                newAttribute = new KeyValuePair<string, object>(attributeName, attributeValue.ToObject<Decimal>());
                                break;

                            case nameof(EntityReference):
                                newAttribute = new KeyValuePair<string, object>(attributeName, attributeValue.ToObject<EntityReference>());
                                break;
                                
                            case nameof(Guid):
                                newAttribute = new KeyValuePair<string, object>(attributeName, attributeValue.ToObject<Guid>());
                                break;

                            case nameof(DateTime):
                                newAttribute = new KeyValuePair<string, object>(attributeName, attributeValue.ToObject<DateTime>());
                                break;

                            default:
                                throw new NotImplementedException($"Cannot deserialise attributes of type: {attributeType}");
                        }

                        deserialisedEntity.Attributes.Add(newAttribute);
                    }
                }
            }
            else
            {
                throw new Exception($"Unable to deserialise entity");
            }

            if (objectType != typeof(JsonEntity))
                return deserialisedEntity.ToEntity<Entity>();
            else
                return deserialisedEntity;
        }

        public override bool CanConvert(Type objectType)
        {
            return supportedTypes.Contains(objectType);
        }
    }
}
