using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Penneo.Connector;
using RestSharp;

namespace Penneo
{
    /// <summary>
    /// Utility class to invoke the Penneo backend directly using custom URLs
    /// </summary>
    public class RestConnector
    {
        /// <summary>
        /// Get the url resource string for a given entity type
        /// </summary>
        public string GetUrlFromEntityType<T>(Entity parent = null)
            where T: Entity
        {
            return ServiceLocator.Instance.GetInstance<RestResources>().GetResource(typeof(T), parent);
        }

        /// <summary>
        /// Send a custom request to the Penneo backend
        /// </summary>
        public IRestResponse InvokeRequest(string url, Dictionary<string, object> body = null, Method method = Method.GET, Dictionary<string, Dictionary<string, object>> options = null, string customMethod = null, int? page = null, int? perPage = null)
        {
            return ApiConnector.Instance.CallServer(url, body, method, options, customMethod, page, perPage);
        }

        /// <summary>
        /// Map json to a single entity of type T
        /// </summary>
        public T ToSingle<T>(string json)
            where T: Entity
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Map json to a list of entities of type T
        /// </summary>
        public List<T> ToList<T>(string json)
            where T : Entity
        {
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        /// <summary>
        /// Map json to a dictionary of type T with a key generated using the given key selector
        /// </summary>
        public Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(string json, Func<TValue, TKey> keySelector)
            where TValue : Entity
        {
            var list = ToList<TValue>(json);
            return list.ToDictionary(keySelector);
        }
    }
}
