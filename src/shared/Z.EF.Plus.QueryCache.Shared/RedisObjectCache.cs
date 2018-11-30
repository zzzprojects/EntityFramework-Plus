#if !EFCORE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Z.EntityFramework.Plus
{
    /// <summary>The redis object cache.</summary>
    public class RedisObjectCache : ObjectCache
    {
        #region Field
        /// <summary>The deserialize cached object.</summary>
        private readonly Func<Type, string, object> _deserializeCachedObject;

        /// <summary>The redis database.</summary>
        private readonly object _redisDatabase;

        /// <summary>The serialize cached object.</summary>
        private readonly Func<object, string> _serializeCachedObject;

        #endregion

        /// <summary>Constructor.</summary>
        /// <param name="redisDatabase">The redis database.</param>
        /// <param name="serializeCachedObject">The serialize cached object.</param>
        /// <param name="deserializeCachedObject">The deserialize cached object.</param>
        public RedisObjectCache(object redisDatabase, Func<object, string> serializeCachedObject, Func<Type, string, object> deserializeCachedObject)
        {
            _redisDatabase = redisDatabase;
            _serializeCachedObject = serializeCachedObject;
            _deserializeCachedObject = deserializeCachedObject;
        }

        /// <inheritdoc/>
        public override DefaultCacheCapabilities DefaultCacheCapabilities { get; }

        /// <inheritdoc/>
        public override string Name { get; }

        /// <summary>Adds an or get existing.</summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="absoluteExpiration">The absolute expiration.</param>
        /// <param name="regionName">(Optional) Name of the region.</param>
        /// <returns>An object.</returns>
        public override object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            var ts = absoluteExpiration.UtcDateTime.Subtract(DateTime.UtcNow);
            return InternalRedisDatabaseAdd(key, value, ts);
        }

        /// <summary>Adds an or get existing.</summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="policy">The policy.</param>
        /// <param name="regionName">(Optional) Name of the region.</param>
        /// <returns>An object.</returns>
        public override object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            ValidatePolicy(policy);
            var ts = policy.AbsoluteExpiration.UtcDateTime.Subtract(DateTime.UtcNow);
            return InternalRedisDatabaseAdd(key, value, ts);
        }

        /// <summary>Gets.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="key">The key.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="regionName">(Optional) Name of the region.</param>
        /// <returns>An object.</returns>
        public object Get(string key, Type entityType, string regionName = null)
        {
            var t = _redisDatabase.GetType();
            var asm = t.Assembly;

            var redisKeyType = asm.GetType("StackExchange.Redis.RedisKey");
            var redisKey = asm.CreateInstance("StackExchange.Redis.RedisKey");
            var opImplicitKey = redisKeyType.GetMethod("op_Implicit", new[] {typeof(string)});

            if (opImplicitKey != null)
            {
                redisKey = opImplicitKey.Invoke(redisKey, new object[] {key});
            }
            else
            {
                throw new Exception("Method op_Implicit not found for StackExchange.Redis.RedisKey");
            }

            var methodGet = _redisDatabase.GetType().GetMethod("StringGet", new[] {asm.GetType("StackExchange.Redis.RedisKey"), asm.GetType("StackExchange.Redis.CommandFlags")});
            if (methodGet != null)
            {
                var result = methodGet.Invoke(_redisDatabase, new[] {redisKey, Type.Missing});
                if (string.IsNullOrEmpty(result.ToString()))
                {
                    return null;
                }

                var resultObj = _deserializeCachedObject(entityType, result.ToString());
                return resultObj;
            }

            throw new Exception("Method StringGet not found for StackExchange.Redis.Database");
        }

        /// <summary>Internal redis database add.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>An object.</returns>
        private object InternalRedisDatabaseAdd(string key, object value, TimeSpan timeSpan)
        {
            var t = _redisDatabase.GetType();
            var asm = t.Assembly;

            var redisKeyType = asm.GetType("StackExchange.Redis.RedisKey");
            var redisKey = asm.CreateInstance("StackExchange.Redis.RedisKey");
            var opImplicitKey = redisKeyType.GetMethod("op_Implicit", new[] {typeof(string)});
            if (opImplicitKey != null)
            {
                redisKey = opImplicitKey.Invoke(redisKey, new object[] {key});
            }
            else
            {
                throw new Exception("Method op_Implicit not found for StackExchange.Redis.RedisKey");
            }

            var redisValueType = asm.GetType("StackExchange.Redis.RedisValue");
            var redisValue = asm.CreateInstance("StackExchange.Redis.RedisValue");
            var opImplicitValue = redisValueType.GetMethod("op_Implicit", new[] {typeof(string)});
            var serializedValue = _serializeCachedObject(value);
            if (opImplicitValue != null)
            {
                redisValue = opImplicitValue.Invoke(redisValue, new object[] {serializedValue});
            }
            else
            {
                throw new Exception("Method op_Implicit not found for StackExchange.Redis.RedisValue");
            }

            var methodInfo = t.GetMethods().Where(p => p.Name == "StringSet").ToArray();
            var methodSet = _redisDatabase.GetType().GetMethod("StringSet", new[]
            {
                asm.GetType("StackExchange.Redis.RedisKey"),
                asm.GetType("StackExchange.Redis.RedisValue"),
                typeof(TimeSpan),
                asm.GetType("StackExchange.Redis.When"),
                asm.GetType("StackExchange.Redis.CommandFlags")
            });

            if (methodSet != null)
            {
                methodSet.Invoke(_redisDatabase, new[] {redisKey, redisValue, Type.Missing, Type.Missing, Type.Missing});
                return value;
            }

            throw new Exception("Method StringSet not found for StackExchange.Redis.Database");
        }

        /// <summary>Validates the policy described by policy.</summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <exception cref="ArgumentException">
        ///     Thrown when one or more arguments have unsupported or
        ///     illegal values.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when one or more arguments are outside the
        ///     required range.
        /// </exception>
        /// <param name="policy">The policy.</param>
        private void ValidatePolicy(CacheItemPolicy policy)
        {
            if (policy.RemovedCallback != null)
            {
                throw new NotSupportedException("Removed Callback is not supported at this time");
            }

            if (policy.UpdateCallback != null)
            {
                throw new NotSupportedException("Update Callback is not supported at this time");
            }

            if (policy.ChangeMonitors.Count != 0)
            {
                throw new NotSupportedException("Change Monitors are not supported at this time");
            }

            if (policy.AbsoluteExpiration != InfiniteAbsoluteExpiration
                && policy.SlidingExpiration != NoSlidingExpiration)
            {
                throw new ArgumentException("Invalid expiration combination", nameof(policy));
            }

            if (policy.SlidingExpiration < NoSlidingExpiration || new TimeSpan(365, 0, 0, 0) < policy.SlidingExpiration)
            {
                throw new ArgumentOutOfRangeException(nameof(policy));
            }

            if (policy.Priority != CacheItemPriority.Default && policy.Priority != CacheItemPriority.NotRemovable)
            {
                throw new ArgumentOutOfRangeException(nameof(policy));
            }
        }

        #region NotImplementedMethods

        /// <inheritdoc/>
        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override object Get(string key, string regionName = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override CacheItem GetCacheItem(string key, string regionName = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override object Remove(string key, string regionName = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override object this[string key]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override long GetCount(string regionName = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Contains(string key, string regionName = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
#endif