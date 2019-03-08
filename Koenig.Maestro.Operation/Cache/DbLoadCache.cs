using Koenig.Maestro.Operation.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Operation.Cache
{
    public abstract class DbLoadCache<TKey, TValue> : CacheBase<TKey, TValue> where TValue : class
    {
        private static readonly Logger logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        protected Func<SqlReader, Tuple<TKey, TValue>> getItemFunc;

        private string spName;

        public DbLoadCache(string cacheName, string spName): base(cacheName, true, null)
        {
            this.spName = spName;
            //this.getItemFunc = getItemFunc;

            try
            {
                ExecuteInitialLoad();
            }
            catch (Exception ex)
            {//Swallow exception here
                logger.Error(ex, "PerformFirstLoad exception");
            }
        }


        protected virtual SpCall LoadAllProcedure
        {
            get { return new SpCall(spName); }
        }

        protected virtual Tuple<TKey, TValue> GetItem(SqlReader reader)
        {
            if (getItemFunc == null)
            {
                throw new ArgumentNullException("Override GetItem or pass GetItemFunc");
            }
            return getItemFunc(reader);
        }

        protected sealed override Dictionary<TKey, TValue> GetAllValues()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            using (Database db = new Database())
            {
                SpCall sp = LoadAllProcedure;
                using (SqlReader smartSqlReader = db.ExecuteReader(sp))
                {
                    while (smartSqlReader.Read())
                    {
                        Tuple<TKey, TValue> item = GetItem(smartSqlReader);
                        if (item != null)
                        {
                            dictionary.Add(item.Item1, item.Item2);
                        }
                    }
                    return dictionary;
                }
            }
        }

    }
}
