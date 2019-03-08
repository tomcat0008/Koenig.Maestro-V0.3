using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Operation.Cache
{

    public interface ICacheDictionary<TKey, TValue> : ICacheDictionary
    {
        ICollection<TKey> Keys
        {
            get;
        }

        ICollection<TValue> Values
        {
            get;
        }

        TValue this[TKey key]
        {
            get;
        }

        bool ContainsKey(TKey key);

        bool TryGetValue(TKey key, out TValue value);
    }


    public interface ICacheDictionary
    {
        int Count
        {
            get;
        }

        string CacheId
        {
            get;
        }
        

        void Reload();

        Dictionary<object, object> GetAll();
    }
}
