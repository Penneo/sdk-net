using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Penneo.Util;
using RestSharp;

namespace Penneo.Connector
{
    internal class ApiConnector : IApiConnector
    {
        /// <summary>
        /// The Penneo server endpoint
        /// </summary>
        private static string _endpoint = "https://sandbox.penneo.com/api/v1";

        /// <summary>
        /// The api connector factory
        /// </summary>
        private static Func<IApiConnector> _factory;

        /// <summary>
        /// The singleton instance
        /// </summary>
        private static IApiConnector _instance;

        /// <summary>
        /// Use proxy settings from Internet Explorer
        /// </summary>
        private static bool _useAutomaticProxy;

        /// <summary>
        /// Success status codes
        /// </summary>
        private readonly List<HttpStatusCode> _successStatusCodes = new List<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.NoContent };

        /// <summary>
        /// The rest http client
        /// </summary>
        private RestClient _client;

        /// <summary>
        /// Http headers
        /// </summary>
        private Dictionary<string, string> _headers;

        /// <summary>
        /// The last Http response
        /// </summary>
        private IRestResponse _lastResponse;

        /// <summary>
        /// Rest resources
        /// </summary>
        private RestResources _restResources;

        /// <summary>
        /// Denotes if the last response received was an error
        /// </summary>
        private bool _wasLastResponseError;

        /// <summary>
        /// The latest server results for each entity processed
        /// </summary>
        private Dictionary<Guid, ServerResult> LatestEntityServerResults { get; set; }

        protected ApiConnector()
        {
            Init();
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static IApiConnector Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                if (!PenneoConnector.IsInitialized)
                {
                    throw new AuthenticationException("The Penneo connector has not been initialized");
                }

                if (_factory != null)
                {
                    _instance = _factory();
                }
                else
                {
                    _instance = new ApiConnector();
                }
                return _instance;
            }
        }

        #region IApiConnector Members

        /// <summary>
        /// <see cref="IApiConnector.WriteObject"/>
        /// </summary>
        public bool WriteObject(Entity obj)
        {
            var result = new ServerResult();

            var data = obj.GetRequestData();
            if (data == null)
            {
                Log.Write("Write Failed for " + obj.GetType().Name + ": Unable to get request data", LogSeverity.Error);
                result.Success = false;
                result.ErrorMessage = "Unable to get request data";
                SetLatestEntityServerResult(obj, result);
                return false;
            }
            if (!obj.IsNew)
            {
                var response = CallServer(obj.RelativeUrl + "/" + obj.Id, data, Method.PUT);
                return ExtractResponse(obj, response, result);
            }
            else
            {
                var response = CallServer(obj.RelativeUrl, data, Method.POST);
                var successfull = ExtractResponse(obj, response, result);
                if (successfull)
                {
                    //Update object with values returned from the API for the created object
                    ReflectionUtil.SetPropertiesFromJson(obj, response.Content);
                }
                return successfull;
            }
        }

        private bool ExtractResponse(Entity obj, IRestResponse response, ServerResult result)
        {
            result.Success = true;
            if (response == null)
            {
                Log.Write("Write Failed for " + obj.GetType().Name + ": Empty response", LogSeverity.Error);
                result.ErrorMessage = "Empty response";
                result.Success = false;
            }
            else
            {
                result.StatusCode = response.StatusCode;
                result.JsonContent = response.Content;
                if (!_successStatusCodes.Contains(response.StatusCode))
                {
                    Log.Write("Write Failed for " + obj.GetType().Name + ": " + response.Content, LogSeverity.Error);
                    result.Success = false;
                }
            }
            SetLatestEntityServerResult(obj, result);
            return result.Success;
        }

        /// <summary>
        /// <see cref="IApiConnector.DeleteObject"/>
        /// </summary>
        public bool DeleteObject(Entity obj)
        {
            var response = CallServer(obj.RelativeUrl + '/' + obj.Id, null, Method.DELETE);
            return response != null && (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent);
        }

        /// <summary>
        /// <see cref="IApiConnector.ReadObject"/>
        /// </summary>
        public bool ReadObject(Entity obj, out IRestResponse response)
        {
            response = CallServer(obj.RelativeUrl + '/' + obj.Id);
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }
            ReflectionUtil.SetPropertiesFromJson(obj, response.Content);
            return true;
        }

        /// <summary>
        /// <see cref="IApiConnector.LinkEntity"/>
        /// </summary>
        public bool LinkEntity(Entity parent, Entity child)
        {
            var url = parent.RelativeUrl + "/" + parent.Id + "/" + _restResources.GetResource(child.GetType()) + "/" + child.Id;
            var response = CallServer(url, customMethod: "LINK");
            var result = new ServerResult();
            return ExtractResponse(parent, response, result);
        }

        /// <summary>
        /// <see cref="IApiConnector.UnlinkEntity"/>
        /// </summary>
        public bool UnlinkEntity(Entity parent, Entity child)
        {
            var url = parent.RelativeUrl + "/" + parent.Id + "/" + _restResources.GetResource(child.GetType()) + "/" + child.Id;
            var response = CallServer(url, customMethod: "UNLINK");
            var result = new ServerResult();
            return ExtractResponse(parent, response, result);
        }

        /// <summary>
        /// <see cref="IApiConnector.GetLinkedEntities{T}"/>
        /// </summary>
        public IEnumerable<T> GetLinkedEntities<T>(Entity obj, string url = null)
        {
            string actualUrl;
            if (string.IsNullOrEmpty(url))
            {
                actualUrl = obj.RelativeUrl + "/" + obj.Id + "/" + _restResources.GetResource<T>();
            }
            else
            {
                actualUrl = url;
            }

            var response = CallServer(actualUrl);
            return CreateObjects<T>(response.Content);
        }

        /// <summary>
        /// <see cref="IApiConnector.FindLinkedEntity{T}"/>
        /// </summary>
        public T FindLinkedEntity<T>(Entity obj, int id)
        {
            var url = obj.RelativeUrl + "/" + obj.Id + "/" + _restResources.GetResource<T>() + "/" + id;
            var response = CallServer(url);
            if (response == null || !_successStatusCodes.Contains(response.StatusCode))
            {
                throw new Exception("Penneo: Internal problem encountered");
            }
            return CreateObject<T>(response.Content);
        }

        /// <summary>
        /// <see cref="IApiConnector.GetFileAssets"/>
        /// </summary>
        public byte[] GetFileAssets(Entity obj, string assetName)
        {
            var encoded = GetTextAssets(obj, assetName);
            return Convert.FromBase64String(encoded);
        }

        /// <summary>
        /// <see cref="IApiConnector.GetTextAssets"/>
        /// </summary>
        public string GetTextAssets(Entity obj, string assetName)
        {
            var url = obj.RelativeUrl + "/" + obj.Id + "/" + assetName;
            var response = CallServer(url);
            var result = JsonConvert.DeserializeObject<string[]>(response.Content);
            return result[0];
        }

        /// <summary>
        /// <see cref="IApiConnector.GetStringListAsset"/>
        /// </summary>
        public IEnumerable<string> GetStringListAsset(Entity obj, string assetName)
        {
            var url = obj.RelativeUrl + "/" + obj.Id + "/" + assetName;
            var response = CallServer(url);
            var result = JsonConvert.DeserializeObject<string[]>(response.Content);
            return result;
        }

        /// <summary>
        /// <see cref="IApiConnector.FindBy{T}"/>
        /// </summary>
        public bool FindBy<T>(Dictionary<string, object> query, out IEnumerable<T> objects, out IRestResponse response)
            where T : Entity
        {
            var resource = _restResources.GetResource<T>();

            Dictionary<string, Dictionary<string, object>> options = null;
            if (query != null && query.Count > 0)
            {
                options = new Dictionary<string, Dictionary<string, object>>();
                options["query"] = query;
            }
            response = CallServer(resource, null, Method.GET, options);
            if (response == null || !_successStatusCodes.Contains(response.StatusCode))
            {
                objects = null;
                return false;
            }

            objects = CreateObjects<T>(response.Content);
            return true;
        }

        /// <summary>
        /// <see cref="IApiConnector.PerformAction"/>
        /// </summary>
        public ServerResult PerformAction(Entity obj, string actionName)
        {
            var result = new ServerResult();
            var url = obj.RelativeUrl + "/" + obj.Id + "/" + actionName;
            var response = CallServer(url, customMethod: "patch");
            if (response == null || !_successStatusCodes.Contains(response.StatusCode))
            {
                result.Success = false;
                result.StatusCode = response.StatusCode;
                result.ErrorMessage = response.ErrorMessage;
                return result;
            }
            result.Success = true;
            return result;
        }

        /// <summary>
        /// Was the last response an error
        /// </summary>
        public bool WasLastResponseError
        {
            get { return _wasLastResponseError; }
        }

        /// <summary>
        /// Get the content of the last response
        /// </summary>
        public string LastResponseContent
        {
            get { return _lastResponse != null ? _lastResponse.Content : null; }
        }

        #endregion

        /// <summary>
        /// Initializes the API connector with rest client, endpoint, headers and authentication
        /// </summary>
        private void Init()
        {
            if (!string.IsNullOrEmpty(PenneoConnector.Endpoint))
            {
                _endpoint = PenneoConnector.Endpoint;
            }
            _client = new RestClient(_endpoint);

            _restResources = ServiceLocator.Instance.GetInstance<RestResources>();

            _headers = PenneoConnector.Headers ?? new Dictionary<string, string>();
            _headers["Content-type"] = "application/json";

            if (!string.IsNullOrEmpty(PenneoConnector.User))
            {
                _headers["penneo-api-user"] = PenneoConnector.User;
            }

            if (PenneoConnector.AuthenticationType == AuthType.WSSE)
            {
                _client.Authenticator = new WSSEAuthenticator(PenneoConnector.Key, PenneoConnector.Secret);
            }
            else
            {
                throw new NotSupportedException("Unknown authentication type " + PenneoConnector.AuthenticationType);
            }
            LatestEntityServerResults = new Dictionary<Guid, ServerResult>();

            PenneoConnector.Reset();
        }

        /// <summary>
        /// Sets a factory for creating a connector.
        /// </summary>
        public static void SetFactory(Func<IApiConnector> factory)
        {
            _factory = factory;

            //Null instance if a new factory is provided
            ResetInstance();
        }

        /// <summary>
        /// Sets whether to use automatic proxy settings from Internet Explorer
        /// </summary>
        public static void SetUseProxySettingsFromInternetExplorer(bool use)
        {
            _useAutomaticProxy = use;
        }

        public static void ResetInstance()
        {
            _instance = null;
        }

        /// <summary>
        /// Prepare a rest request
        /// </summary>
        private RestRequest PrepareRequest(string url, Dictionary<string, object> data = null, Method method = Method.GET, Dictionary<string, Dictionary<string, object>> options = null)
        {
            var request = new RestRequest(url, method);
            foreach (var h in _headers)
            {
                request.AddHeader(h.Key, h.Value);
            }

            if (options != null)
            {
                foreach (var entry in options)
                {
                    var key = entry.Key;
                    var o = entry.Value;
                    if (key == "query")
                    {
                        VisitQuery(request, o);
                    }
                }
            }

            if (data != null)
            {
                request.RequestFormat = DataFormat.Json;
                request.AddBody(data);
            }
            return request;
        }

        /// <summary>
        /// Parse 'query' options into request data
        /// </summary>
        private static void VisitQuery(RestRequest request, Dictionary<string, object> query)
        {
            foreach (var entry in query)
            {
                request.AddParameter(StringUtil.FirstCharacterToLower(entry.Key), entry.Value);
            }
        }

        /// <summary>
        /// Set the proxy information on the rest client
        /// </summary>
        private void SetProxy()
        {
            if (_useAutomaticProxy)
            {
                var proxyWrapper = WebRequest.DefaultWebProxy;
                if (proxyWrapper != null)
                {
                    var proxy = proxyWrapper.GetProxy(new Uri(_endpoint));
                    if (proxy != null && !_endpoint.Equals(proxy.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        Log.Write("Proxy URL: " + proxy, LogSeverity.Information);
                        _client.Proxy = new WebProxy(proxy);
                    }
                }
            }
            else
            {
                _client.Proxy = null;
            }
        }

        /// <summary>
        /// Calls the Penneo server with a rest request
        /// </summary>
        public IRestResponse CallServer(string url, Dictionary<string, object> data = null, Method method = Method.GET, Dictionary<string, Dictionary<string, object>> options = null, string customMethod = null)
        {
            SetProxy();
            try
            {
                var request = PrepareRequest(url, data, method, options);
                IRestResponse response;

                string actualMethod;
                if (string.IsNullOrEmpty(customMethod))
                {
                    actualMethod = method.ToString();
                    response = _client.Execute(request);
                }
                else
                {
                    actualMethod = customMethod;
                    response = _client.ExecuteAsGet(request, customMethod);
                }
                Log.Write("Request " + actualMethod + " " + url + " /  Response '" + response.StatusCode + "'", LogSeverity.Trace);

                _lastResponse = response;
                _wasLastResponseError = !_successStatusCodes.Contains(_lastResponse.StatusCode);
                return response;
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString(), LogSeverity.Fatal);
                throw;
            }
        }

        /// <summary>
        /// Create objects from a json string
        /// </summary>
        private static IEnumerable<T> CreateObjects<T>(string json)
        {
            var direct = JsonConvert.DeserializeObject<List<T>>(json);
            return direct;
        }

        /// <summary>
        /// Creates a single object from a json string
        /// </summary>
        private static T CreateObject<T>(string json)
        {
            var direct = JsonConvert.DeserializeObject<T>(json);
            return direct;
        }

        /// <summary>
        /// Sets the latest server result for a given entity
        /// </summary>
        private void SetLatestEntityServerResult(Entity entity, ServerResult result)
        {
            if (entity == null)
            {
                return;
            }
            lock (LatestEntityServerResults)
            {
                LatestEntityServerResults[entity.InternalIdentifier] = result;
            }
        }

        /// <summary>
        /// Get the latest server result for a given entity
        /// </summary>
        public ServerResult GetLatestEntityServerResult(Entity entity)
        {
            if (entity == null)
            {
                return null;
            }
            lock (LatestEntityServerResults)
            {
                ServerResult result;
                if (LatestEntityServerResults.TryGetValue(entity.InternalIdentifier, out result))
                {
                    return result;
                }
            }
            return null;
        }
    }

    public class PenneoDateConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var unixTime = TimeUtil.ToUnixTime((DateTime)value);
            writer.WriteValue(unixTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var unixTime = Convert.ToInt64(reader.Value);
            return TimeUtil.FromUnixTime(unixTime);
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}