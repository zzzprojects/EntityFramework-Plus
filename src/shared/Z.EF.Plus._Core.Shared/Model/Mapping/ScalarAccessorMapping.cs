#if FULL || BATCH_DELETE || BATCH_UPDATE
#if EF5 || EF6
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Z.EF.Plus.BatchUpdate.Shared.Extensions;
using Z.EntityFramework.Plus.Internal.Core.SchemaObjectModel;

namespace Z.EntityFramework.Extensions.Core.Mapping
{
    /// <summary>A scalar accessor mapping.</summary>
    public class ScalarAccessorMapping
    {
        /// <summary>The get value fast gets.</summary>
        internal Lazy<ConcurrentDictionary<Type, Func<object, object>>> GetValueFastGets = new Lazy<ConcurrentDictionary<Type, Func<object, object>>>();
        internal Lazy<ConcurrentDictionary<Type, Func<object, bool>>> GetReferenceValidCompiled = new Lazy<ConcurrentDictionary<Type, Func<object, bool>>>();
        internal string RelationShipEnd;
        internal Lazy<ConcurrentDictionary<Type, Action<object, object>>> SetValueFastSets = new Lazy<ConcurrentDictionary<Type, Action<object, object>>>();
        internal string TargetRoleName;
        internal bool IsConcurrency { get; set; }
        internal bool IsIdentity { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is from association.</summary>
        /// <value>true if this object is from association, false if not.</value>
        internal bool IsFromAssociation { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is computed.</summary>
        /// <value>true if this object is computed, false if not.</value>
        public bool IsComputed { get; set; }

        /// <summary>Gets or sets a value indicating whether this object use is null column.</summary>
        /// <value>true if use is null column, false if not.</value>
        public bool UseIsNullColumn { get; set; }

        /// <summary>Gets or sets the full pathname of the accessor file.</summary>
        /// <value>The full pathname of the accessor file.</value>
        public string AccessorPath { get; set; }

        /// <summary>Gets or sets the name of the column.</summary>
        /// <value>The name of the column.</value>
        public string ColumnName { get; set; }

        /// <summary>Gets or sets the type.</summary>
        /// <value>The type.</value>
        public Type Type { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is storage mapped.</summary>
        /// <value>true if this object is storage mapped, false if not.</value>
        public bool IsStorageMapped { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is key.</summary>
        /// <value>true if this object is key, false if not.</value>
        public bool IsKey { get; set; }

        public bool IsExternalKey { get; set; }

        /// <summary>Gets or sets the constant value.</summary>
        /// <value>The constant value.</value>
        public string ConstantValue { get; set; }

        internal int GetValueAccessorCount { get; set; }

        /// <summary>Gets a value.</summary>
        /// <param name="obj">The object.</param>
        /// <returns>The value.</returns>
        internal object GetValue(object obj)
        {
            // If this is a constant like a discriminator
            if (!string.IsNullOrEmpty(ConstantValue))
            {
                return ConstantValue;
            }

            return GetValueFastGet(obj).To(Type);
        }

        internal bool IsValidReferenceGetValue(object obj)
        {
            // If this is a constant like a discriminator
            if (!string.IsNullOrEmpty(ConstantValue) || GetValueAccessorCount == 1)
            {
                return true;
            }

            return GetIsValidReferenceGetValue(obj);
        }


        /// <summary>Sets a value.</summary>
        /// <param name="obj">The object.</param>
        /// <param name="valueItem">The value item.</param>
        internal void SetValue(object obj, object valueItem)
        {
            SetValueFastSet(obj, valueItem);
        }

        /// <summary>Gets value fast get.</summary>
        /// <param name="obj">The object.</param>
        /// <returns>The value fast get.</returns>
        internal object GetValueFastGet(object obj)
        {
            if (!GetValueFastGets.Value.ContainsKey(obj.GetType()))
            {
                var list = new List<Tuple<Type, PropertyInfo>>();

                var propertyPaths = AccessorPath.Split('.');

                var type = obj.GetType();
                foreach (var path in propertyPaths)
                {
                    PropertyInfo property = null;

                    // Loop until we found a base type supporting the property path
                    while (type != null)
                    {
                        property = type.GetProperty(path, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        if (property != null)
                        {
                            break;
                        }

                        type = type.BaseType;
                    }

                    if (property == null)
                    {
                        throw new Exception("Property could not be resolved: " + path + " FullName:" + propertyPaths);
                    }

                    list.Add(new Tuple<Type, PropertyInfo>(type, property));
                    type = property.PropertyType;
                }

                GetValueFastGets.Value[obj.GetType()] = Utility.CreateGetFunc(list);
            }

            var fastGet = GetValueFastGets.Value[obj.GetType()];
            return fastGet(obj);
        }

        internal bool GetIsValidReferenceGetValue(object obj)
        {
            if (!GetReferenceValidCompiled.Value.ContainsKey(obj.GetType()))
            {
                var list = new List<Tuple<Type, PropertyInfo>>();

                var propertyPaths = AccessorPath.Split('.');

                var type = obj.GetType();
                foreach (var path in propertyPaths)
                {
                    PropertyInfo property = null;

                    // Loop until we found a base type supporting the property path
                    while (type != null)
                    {
                        property = type.GetProperty(path, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        if (property != null)
                        {
                            break;
                        }

                        type = type.BaseType;
                    }

                    if (property == null)
                    {
                        throw new Exception("Property could not be resolved: " + path + " FullName:" + propertyPaths);
                    }

                    list.Add(new Tuple<Type, PropertyInfo>(type, property));
                    type = property.PropertyType;
                }

                GetValueAccessorCount = list.Count;

                if (list.Count == 1)
                {
                    return true;
                }

                GetReferenceValidCompiled.Value[obj.GetType()] = Utility.CreateGetReferenceValidFunc(list);
            }

            var fastGet = GetReferenceValidCompiled.Value[obj.GetType()];
            return fastGet(obj);
        }

        internal void SetValueFastSet(object obj, object valueItem)
        {
            if (!SetValueFastSets.Value.ContainsKey(obj.GetType()))
            {
                var list = new List<Tuple<Type, PropertyInfo>>();

                var propertyPaths = AccessorPath.Split('.');

                var type = obj.GetType();
                foreach (var path in propertyPaths)
                {
                    PropertyInfo property = null;

                    // Loop until we found a base type supporting the property path
                    while (type != null)
                    {
                        property = type.GetProperty(path, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        if (property != null)
                        {
                            break;
                        }

                        type = type.BaseType;
                    }

                    if (property == null)
                    {
                        throw new Exception("Property could not be resolved: " + path + " FullName:" + propertyPaths);
                    }

                    list.Add(new Tuple<Type, PropertyInfo>(type, property));
                    type = property.PropertyType;
                }

                SetValueFastSets.Value[obj.GetType()] = Utility.CreateSetFunc(list);
            }

            var fastSet = SetValueFastSets.Value[obj.GetType()];
            fastSet(obj, valueItem);
        }
    }
}
#endif
#endif