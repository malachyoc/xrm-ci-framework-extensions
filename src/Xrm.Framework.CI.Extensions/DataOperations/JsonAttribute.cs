using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Xrm.Framework.CI.Extensions.DataOperations
{
    public class JsonAttribute
    {
        private string _logicalName;
        private string _type;
        private object _value;

        public string LogicalName { get => _logicalName; set => _logicalName = value; }
        public string Type { get => _type; set => _type = value; }
        public object Value { get => _value; set => this._value = value; }

        public JsonAttribute()
        {

        }

        public override bool Equals(object obj)
        {
            var item = obj as JsonAttribute;
            if (item == null)
            {
                return false;
            }

            //If the attribute is not for the same field then return false
            if (!_logicalName.Equals(item.LogicalName) || !_type.Equals(Type))
                return false;

            //If the objects are equals return true
            if (_value.Equals(Value))
                return true;

            //I'm being lazy.  Probably more effienent to perform specific comparison based on Value type
            string thisSerialized = JsonConvert.SerializeObject(this);
            string objectSerialized = JsonConvert.SerializeObject(obj);
            return thisSerialized.Equals(objectSerialized);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public static JsonAttribute Create(KeyValuePair<string, object> attKvp)
        {
            string attributeType = (attKvp.Value == null ? "NullValue" : attKvp.Value.GetType().Name);

            switch (attributeType)
            {
                case "NullValue":
                    return new JsonAttribute() {
                        LogicalName = attKvp.Key
                        , Value = null
                    };

                case nameof(String):
                    return new JsonAttribute() {
                        LogicalName = attKvp.Key
                        , Type = nameof(String)
                        , Value = attKvp.Value
                    };

                case nameof(Boolean):
                    return  new JsonAttribute() {
                        LogicalName = attKvp.Key
                        , Type = nameof(Boolean)
                        , Value = attKvp.Value
                    };

                case nameof(OptionSetValue):
                    return new JsonAttribute() {
                        LogicalName = attKvp.Key
                        , Type = nameof(OptionSetValue)
                        , Value = ((OptionSetValue)attKvp.Value).Value
                    };

                case nameof(Decimal):
                    return new JsonAttribute() {
                        LogicalName = attKvp.Key
                        , Type = nameof(Decimal)
                        , Value = attKvp.Value
                    };

                case nameof(EntityReference):
                    return new JsonAttribute() {
                        LogicalName = attKvp.Key
                        , Type = nameof(EntityReference)
                        , Value = new JsonEntityReference() {
                            Id = ((EntityReference)attKvp.Value).Id
                            , LogicalName = ((EntityReference)attKvp.Value).LogicalName
                        }
                    };

                case nameof(Int32):
                    return new JsonAttribute() {
                        LogicalName = attKvp.Key
                        , Type = nameof(Int32)
                        , Value = attKvp.Value
                    };

                case nameof(Guid):
                    return new JsonAttribute()
                    {
                        LogicalName = attKvp.Key
                        , Type = nameof(Guid)
                        , Value = attKvp.Value
                    };

                case nameof(DateTime):
                    return new JsonAttribute()
                    {
                        LogicalName = attKvp.Key
                        , Type = nameof(DateTime)
                        , Value = attKvp.Value
                    };

                default:
                    throw new NotImplementedException($"Cannot convert attributes of type: {attKvp.Value.GetType().FullName}");
            }
        }
    }
}
