using Koenig.Maestro.Operation.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Operation.Cache
{
    internal class CacheRegistry
    {
        private static readonly Logger logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private static Dictionary<string, ICacheDictionary> registry = new Dictionary<string, ICacheDictionary>();

        /// <summary>
        /// Tüm cache'lerin saklandığı merkezi nesne.
        /// </summary>
        public static Dictionary<string, ICacheDictionary> Registry
        {
            get { return registry; }
        }

        public static void Register(string globalCacheName, string name, ICacheDictionary cache)
        {

            try
            {
                InsertCacheRegistryInfo(new CacheRegistryInfo()
                {
                    //ApplicationCode = "MAESTRO",
                    CacheId = name,
                    //GlobalCacheName = name,
                    HostName = Environment.MachineName
                });
            }
            catch (Exception e)
            {
                logger.Fatal(e, "'" + name + "' cache nesnesi register edilemedi.RegisterCache");
            }
            lock (registry)
            {
                try
                {
                    if (!registry.ContainsKey(name))
                    {
                        registry.Add(name, cache);
                    }
                    logger.Info("'" + name + "' cache nesnesi register edildi.");
                }
                catch (Exception e)
                {
                    logger.Fatal(e, "'" + name + "' cache nesnesi register edilemedi.");
                }
            }
        }


        public static void InsertCacheRegistryInfo(CacheRegistryInfo cacheRegistryInfo)
        {
            SpCall sp = new SpCall("CACHE.CACHE_REGISTRY_INSERT");
            sp.SetVarchar("@GLOBAL_CACHE_NAME", cacheRegistryInfo.CacheId);
            sp.SetVarchar("@CACHE_ID", cacheRegistryInfo.CacheId);
            sp.SetVarchar("@APPLICATION_CODE", "MAESTRO");
            sp.SetVarchar("@CACHE_HOST_NAME", cacheRegistryInfo.HostName);
            sp.SetDateTime("@UPDATE_DATE", DateTime.Now);
            sp.SetVarchar("@RECORD_STATUS", "A");

            using (Database db = new Database())
            {
                db.ExecuteNonQuery(sp);
            }
        }

        public static List<CacheInfo> GetCacheInfos()
        {
            return GetCacheInfos(true);
        }

        static List<CacheInfo> GetCacheInfos(bool retryException)
        {
            List<CacheInfo> cacheInfoList = new List<CacheInfo>();
            try
            {
                foreach (KeyValuePair<string, ICacheDictionary> cache in CacheRegistry.registry)
                {
                    CacheInfo ci = new CacheInfo();
                    //ci.GlobalCacheName = cache.Value.CacheId;
                    ci.CacheId = cache.Key.ToString();
                    ci.Count = cache.Value.Count;
                    ci.ReloadDuration = 1000;
                    ci.ReloadTime = DateTime.Now;
                    cacheInfoList.Add(ci);
                }
            }
            catch (Exception ex)
            {
                if (retryException)
                {
                    return GetCacheInfos(false);
                }
                else
                {
                    logger.Error(ex, "Could not get cache information.");
                    throw;
                }
            }
            return cacheInfoList;
        }

        public static CacheInfo GetCacheInfo(string cacheId)
        {
            ICacheDictionary cache = null;
            if (CacheRegistry.registry.TryGetValue(cacheId, out cache))
            {
                CacheInfo ci = new CacheInfo();
                //ci.GlobalCacheName = cache.GlobalCacheName;
                ci.CacheId = cache.CacheId;
                ci.Count = cache.Count;
                ci.ReloadDuration = 1000;
                ci.ReloadTime = DateTime.Now;
                return ci;
            }
            return null;
        }

        public static string Reload(string cacheId)
        {
            ICacheDictionary cache;
            string msg = "";
            if (!CacheRegistry.Registry.TryGetValue(cacheId, out cache))
            {
                msg = string.Format("Cache '{0}' does not exist in the registry, Reload cancelled.", cacheId);
                logger.Info(msg);
                return msg;
            }
            //cache.OnReplicate();
            msg = string.Format("'{0}' reloaded.", cacheId);
            logger.Info(msg);

            return msg;
        }

        public static void ReloadAll()
        {
            foreach (KeyValuePair<string, ICacheDictionary> cache in CacheRegistry.registry)
            {
                cache.Value.Reload();
            }
        }

        public static Dictionary<object, object> GetAllData(string cacheId)
        {
            ICacheDictionary cache;
            if (!CacheRegistry.Registry.TryGetValue(cacheId, out cache))
            {
                logger.Info("Cache '{0}' does not exist in the registry.", new object[] { cacheId });
                return null;
            }

            return cache.GetAll();
        }

    }
}
