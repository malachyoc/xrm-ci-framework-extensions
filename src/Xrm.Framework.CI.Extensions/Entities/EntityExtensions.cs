using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Xrm.Framework.CI.Extensions.DataOperations;

namespace Xrm.Framework.CI.Extensions.Entities
{
    public static class EntityExtensions
    {
        /// <summary>
        /// Updated the base entity with the attributes from the entity to merge
        /// </summary>
        /// <param name="baseEntity"></param>
        /// <param name="entityToMerge"></param>
        public static void Merge(this Entity baseEntity, Entity entityToMerge)
        {
            if (entityToMerge == null)
                return;

            if (entityToMerge.Id != Guid.Empty)
            {
                baseEntity.Id = entityToMerge.Id;
            }

            if (String.IsNullOrWhiteSpace(entityToMerge.LogicalName) == false && String.IsNullOrWhiteSpace(baseEntity.LogicalName))
            {
                baseEntity.LogicalName = entityToMerge.LogicalName;
            }

            foreach (KeyValuePair<String, Object> attribute in entityToMerge.Attributes)
            {
                baseEntity[attribute.Key] = attribute.Value;
            }
        }

        /// <summary>
        /// Compare the entites and return the minimum set of changes that need to be made inorder to make the base entity
        /// the same as the updated entity
        /// </summary>
        /// <param name="baseEntity"></param>
        /// <param name="updatedEntity"></param>
        public static Entity Delta(this Entity baseEntity, Entity updatedEntity)
        {
            //Clone the attribute
            Entity clone = updatedEntity.Clone();

            //Create a delta (do not update fields that have not changed)
            foreach (var att in baseEntity.Attributes)
            {
                //Remove unchanged fields from the delta
                if (clone.Attributes.Contains(att.Key))
                {
                    //TODO: Clean up Attribute comparisons
                    var updatedAttribute = updatedEntity.Attributes.Where(a => a.Key == att.Key).First();
                    if (JsonAttribute.Create(att).Equals(JsonAttribute.Create(updatedAttribute)))
                        clone.Attributes.Remove(att.Key);
                }
            }

            List<String> attributesToRemove = new List<string>();
            foreach (var att in updatedEntity.Attributes)
            {
                //Do not set fields to NULL that are already not present
                if (!baseEntity.Attributes.Contains(att.Key) && att.Value == null)
                    clone.Attributes.Remove(att.Key);
            }

            clone.Id = updatedEntity.Id;
            return clone;
        }

        /// <summary>
        /// Create a clone of the entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseEntity"></param>
        /// <returns></returns>
        public static T Clone<T>(this T baseEntity)
            where T : Entity, new()
        {
            var newEntity = new T { Id = baseEntity.Id , LogicalName = baseEntity.LogicalName };
            foreach (KeyValuePair<String, Object> attribute in baseEntity.Attributes)
            {
                newEntity[attribute.Key] = attribute.Value;
            }

            return newEntity;
        }

        /// <summary>
        /// Create a clone of Entity class
        /// </summary>
        /// <param name="baseEntity"></param>
        /// <returns></returns>
        public static Entity Clone(this Entity baseEntity)
        {
            return baseEntity.Clone<Entity>();
        }
    }
}
