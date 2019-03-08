using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Cache
{
    public abstract class CacheBase<TKey, TValue> : ICacheDictionary<TKey, TValue> where TValue : class
    {
        static readonly Logger logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        private ConcurrentDictionary<TKey, CacheValue<TValue>> values = new ConcurrentDictionary<TKey, CacheValue<TValue>>();
        object multipleLoadLock = new object();
        object reloadDurationLoadLock = new object();
        protected Func<Dictionary<TKey, TValue>> getAllValuesFunc;

        private TimeSpan reloadSpan;

        private volatile bool multipleLoaded;
        private volatile bool performingMultipleLoad;
        private volatile int multipleLoadingThreadId;
        private long loadTimeTick;

        protected CacheBase(string cacheName, bool registerCache, Func<Dictionary<TKey, TValue>> getAllValuesFunc)
        {
            LoadTime = DateTime.MinValue;
            this.getAllValuesFunc = getAllValuesFunc;
            this.reloadSpan = MaestroApplication.Instance.ReloadTimeSpan;
        }



        public string CacheId{get { return GetType().FullName; }}

        public int Count
        {
            get
            {
                PerformIfNotMultipleLoad();
                return values.Count;
            }
        }

        protected void ExecuteInitialLoad()
        {
            lock (multipleLoadLock)
            {
                try
                {
                    if (!multipleLoaded)
                    {
                        performingMultipleLoad = true;
                        multipleLoadingThreadId = Thread.CurrentThread.ManagedThreadId;
                        values = LoadAndGetAllValues();
                        LoadTime = ExpiryTime;
                        multipleLoaded = true;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Exception in loading cache {0} for multiple values, ex: {1}", CacheId, ex);
                    throw;
                }
                finally
                {
                    performingMultipleLoad = false;
                }
                CacheRegistry.Register(CacheId, CacheId, this);
            }

        }

        DateTime LoadTime
        {
            get
            {
                return new DateTime(Interlocked.Read(ref loadTimeTick));
            }
            set
            {
                Interlocked.Exchange(ref loadTimeTick, value.Ticks);
            }
        }

        public DateTime ExpiryTime
        {
            get
            {
                return DateTime.Now.Add(reloadSpan);
            }

        }

        private void PerformIfNotMultipleLoad()
        {
            if (!multipleLoaded)
            {

                if (performingMultipleLoad)
                {
                    if (multipleLoadingThreadId == Thread.CurrentThread.ManagedThreadId)
                    {
                        string errorMessage = string.Format("Cyclic Get request in cache:{0}", CacheId);
                        logger.Fatal(errorMessage);
                        throw new Exception(errorMessage);
                    }
                }

                try
                {
                    ExecuteInitialLoad();
                }
                catch (Exception ex)
                {
                    logger.Error("Exception in loading cache {0} for multiple values, ex: {1}", CacheId, ex);
                    throw;
                }
            }
            else if (DateTime.Now > LoadTime)
            {
                PerformReload();
            }
        }

        public Dictionary<object, object> GetAll()
        {
            Dictionary<object, object> result = new Dictionary<object, object>();
            PerformIfNotMultipleLoad();
            foreach (var item in values)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }

        public Dictionary<TKey, TValue> GetDictionary()
        {
            Dictionary<TKey, TValue> res = new Dictionary<TKey, TValue>();
            PerformIfNotMultipleLoad();
            foreach (var item in values)
            {
                res.Add(item.Key, item.Value.Value);
            }
            return res;
        }

        public bool ContainsKey(TKey key)
        {
            return ContainsKey(key, true);
        }

        private bool ContainsKey(TKey key, bool swallowLoadException)
        {
            if (key == null) throw new ArgumentNullException("key");


            try
            {
                GetValueInternal(key);
            }
            catch (Exception ex)
            {
                logger.Error("Error in GetValueInternal for ContainsKey in Cache {0} ex:{1}", CacheId , ex);
                if (!swallowLoadException)
                {
                    throw;
                }
                return false;
            }
            CacheValue<TValue> val;
            if (values.TryGetValue(key, out val))
            {
                if (val.Value == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private TValue GetValueInternal(TKey key)
        {
            //if (reloadDurationFromServerEnabled && !hasReloadDurationLoadedFromServer)
            //{
            //    lock (reloadDurationLoadLock)
            //    {
            //        int val = Configuration.ConfigManager.CacheDuration;
            //        if (val > 0)
            //        {
            //            ReloadDuration = val;
            //        }
            //        hasReloadDurationLoadedFromServer = true;
            //    }
            //}
            PerformIfNotMultipleLoad();
            CacheValue<TValue> value;
            values.TryGetValue(key, out value);
            if (value != null)
            {
                return value.Value;
            }
            else
            {
                //TODO:key not found
                return default(TValue);
            }
        }


        public TValue this[TKey key]
        {
            get
            {
                if (!ContainsKey(key, false))
                {
                    throw new KeyNotFoundException(string.Format("KeyNotFound GlobalCacheName: {0} - Key : {1}", CacheId, key.ToString()));
                }
                CacheValue<TValue> val;
                if (values.TryGetValue(key, out val))
                {
                    return val.Value;
                }
                else
                {
                    throw new KeyNotFoundException(string.Format("KeyNotFound in second attempt. GlobalCacheName: {0} - Key : {1}", CacheId, key.ToString()));
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            try
            {
                GetValueInternal(key);
            }
            catch (Exception ex)
            {
                logger.Error("Exception in GetValueInternal for TryGetValue({0}) ex:{1}",
                    key, ex);
                value = default(TValue);
                return false;
            }
            CacheValue<TValue> val;
            bool keyExists = values.TryGetValue(key, out val);
            if (keyExists)
            {
                value = val.Value;
                if (value == null)
                {
                    return false;
                }
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                PerformIfNotMultipleLoad();
                return values.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                ICollection<TValue> result = new List<TValue>();
                PerformIfNotMultipleLoad();
                foreach (CacheValue<TValue> val in values.Values)
                {
                    if (val != null && val.Value != null)
                    {
                        result.Add(val.Value);
                    }
                }
                return result;
            }
        }

        public void Reload()
        {
            PerformReload();
            //Services.Create<IEsbCacheLoaderService>().TriggerReloadCache(globalCacheName, CacheId);
        }

        public void Reload(bool forceReload)
        {
            multipleLoaded = false;
            LoadTime = DateTime.Now.AddMinutes(-1);
            PerformReload();
        }

        void PerformReload()
        {
            if (!performingMultipleLoad)
            {
                /*
                Task t = Task.Factory.StartNew(() =>
                {
                    */
                    try
                    {

                        lock (multipleLoadLock)
                        {
                            if (DateTime.Now > LoadTime)
                            {
                                performingMultipleLoad = true;
                                values = LoadAndGetAllValues();
                                LoadTime = ExpiryTime;
                                multipleLoaded = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //Swallowing exception here. 
                        logger.Error("Exception in loading cache {0} for multiple values, ex: {1}", CacheId, ex);
                    }
                    finally
                    {
                        performingMultipleLoad = false;
                    }
                /*
                }
                );
            */
                //if (syncReload)t.Wait();

                /*
                foreach (var handler in taskHandlers)
                {
                    handler(t);
                }*/
            }
        }

        ConcurrentDictionary<TKey, CacheValue<TValue>> LoadAndGetAllValues()
        {
            ConcurrentDictionary<TKey, CacheValue<TValue>> newValues =
                new ConcurrentDictionary<TKey, CacheValue<TValue>>();

            Dictionary<TKey, TValue> allValues = GetAllValues();
            if (allValues == null)
            {
                return newValues;
            }

            DateTime expiryTime = ExpiryTime;
            foreach (KeyValuePair<TKey, TValue> item in allValues)
            {
                CacheValue<TValue> val = new CacheValue<TValue>();
                val.ExpiryTime = expiryTime;
                //val.Status = CacheValueStatus.Loaded;
                val.Value = item.Value;
                newValues.TryAdd(item.Key, val);
            }
            return newValues;
        }

        protected abstract Dictionary<TKey, TValue> GetAllValues();


    }
}
