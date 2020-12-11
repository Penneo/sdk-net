using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Penneo.Util;
using RestSharp;

namespace Penneo.Connector
{
    internal class ApiConnector : IApiConnector
    {
        /// <summary>
        /// The Penneo connector
        /// </summary>
        private PenneoConnector _con;

        /// <summary>
        /// The Penneo server endpoint
        /// </summary>
        private string _endpoint;

        /// <summary>
        /// The Penneo user
        /// </summary>
        private string _user;

        /// <summary>
        /// The Penneo key
        /// </summary>
        private string _key;

        /// <summary>
        /// The Penneo secret
        /// </summary>
        private string _secret;

        /// <summary>
        /// The Penneo auth type
        /// </summary>
        private AuthType _authType;

        /// <summary>
        /// Use proxy settings from Internet Explorer
        /// </summary>
        private bool _useAutomaticProxy;

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
        private readonly RestResources _restResources;

        /// <summary>
        /// Denotes if the last response received was an error
        /// </summary>
        private bool _wasLastResponseError;

        /// <summary>
        /// The latest server results for each entity processed
        /// </summary>
        private Dictionary<Guid, ServerResult> LatestEntityServerResults { get; set; }

        internal ApiConnector(PenneoConnector con, RestResources restResources, string endpoint, Dictionary<string, string> headers, string user, string key, string secret, AuthType authType)
        {
            _con = con;
            _restResources = restResources;
            _endpoint = endpoint;
            _headers = headers;
            _user = user;
            _key = key;
            _secret = secret;
            _authType = authType;

            Init();
        }

        /*
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
        }*/

        #region IApiConnector Members
        /// <summary>
        /// <see cref="IApiConnector.WriteObject"/>
        /// </summary>
        public bool WriteObject(Entity obj)
        {
            var result = new ServerResult();

            var data = obj.GetRequestData(_con);
            if (data == null)
            {
                _con.Log("Write Failed for " + obj.GetType().Name + ": Unable to get request data", LogSeverity.Error);
                result.Success = false;
                result.ErrorMessage = "Unable to get request data";
                SetLatestEntityServerResult(obj, result);
                return false;
            }
            if (!obj.IsNew)
            {
                var response = CallServer(obj.GetRelativeUrl(_con) + "/" + obj.Id, data, Method.PUT);
                return ExtractResponse(obj, response, result);
            }
            else
            {
                var response = CallServer(obj.GetRelativeUrl(_con), data, Method.POST);
                var successfull = ExtractResponse(obj, response, result);
                if (successfull)
                {
                    //Update id given from server
                    var fromServer = (Entity)JsonConvert.DeserializeObject(response.Content, obj.GetType());
                    obj.Id = fromServer.Id;
                }
                return successfull;
            }
        }

        private bool ExtractResponse(Entity obj, IRestResponse response, ServerResult result)
        {
            result.Success = true;
            if (response == null)
            {
                _con.Log("Write Failed for " + obj.GetType().Name + ": Empty response", LogSeverity.Error);
                result.ErrorMessage = "Empty response";
                result.Success = false;
            }
            else
            {
                result.StatusCode = response.StatusCode;
                result.JsonContent = response.Content;
                if (!_successStatusCodes.Contains(response.StatusCode))
                {
                    _con.Log("Write Failed for " + obj.GetType().Name + ": " + response.Content, LogSeverity.Error);
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
            var response = CallServer(obj.GetRelativeUrl(_con) + '/' + obj.Id, null, Method.DELETE);
            return response != null && (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent);
        }

        public T ReadObject<T>(Entity parent, int? id, out IRestResponse response)
            where T : Entity
        {
            return ReadObject<T>(parent, id, null, out response);
        }

        public T ReadObject<T>(Entity parent, int? id, string relativeUrl, out IRestResponse response)
            where T : Entity
        {
            var url = !string.IsNullOrEmpty(relativeUrl) ? relativeUrl : _restResources.GetResource(typeof(T), parent);
            if (id.HasValue)
            {
                url += "/" + id;
            }
            response = CallServer(url);
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                return default(T);
            }
            var obj = JsonConvert.DeserializeObject<T>(response.Content);
            if (id.HasValue)
            {
                obj.Id = id;
            }
            return obj;
        }



        /// <summary>
        /// <see cref="IApiConnector.LinkEntity"/>
        /// </summary>
        public bool LinkEntity(Entity parent, Entity child)
        {
            var url = parent.GetRelativeUrl(_con) + "/" + parent.Id + "/" + _restResources.GetResource(child.GetType()) + "/" + child.Id;
            var response = CallServer(url, customMethod: "LINK");
            var result = new ServerResult();
            return ExtractResponse(parent, response, result);
        }

        /// <summary>
        /// <see cref="IApiConnector.UnlinkEntity"/>
        /// </summary>
        public bool UnlinkEntity(Entity parent, Entity child)
        {
            var url = parent.GetRelativeUrl(_con) + "/" + parent.Id + "/" + _restResources.GetResource(child.GetType()) + "/" + child.Id;
            var response = CallServer(url, customMethod: "UNLINK");
            var result = new ServerResult();
            return ExtractResponse(parent, response, result);
        }

        /// <summary>
        /// <see cref="IApiConnector.GetLinkedEntities{T}"/>
        /// </summary>
        public QueryResult<T> GetLinkedEntities<T>(Entity obj, string url = null)
            where T: Entity
        {
            string actualUrl;
            if (string.IsNullOrEmpty(url))
            {
                actualUrl = obj.GetRelativeUrl(_con) + "/" + obj.Id + "/" + _restResources.GetResource<T>();
            }
            else
            {
                actualUrl = url;
            }

            var response = CallServer(actualUrl);
            var result = new QueryResult<T>();
            if (ExtractResponse(obj, response, result))
            {
                result.Objects = CreateObjects<T>(response.Content);
            }
            return result;
        }

        /// <summary>
        /// <see cref="IApiConnector.GetLinkedEntity{T}"/>
        /// </summary>
        public QuerySingleObjectResult<T> GetLinkedEntity<T>(Entity obj, string url = null)
            where T : Entity
        {
            string actualUrl;
            if (string.IsNullOrEmpty(url))
            {
                actualUrl = obj.GetRelativeUrl(_con) + "/" + obj.Id + "/" + _restResources.GetResource<T>();
            }
            else
            {
                actualUrl = url;
            }

            var response = CallServer(actualUrl);
            var result = new QuerySingleObjectResult<T>();
            if (ExtractResponse(obj, response, result))
            {
                result.Object = CreateObject<T>(response.Content);
            }
            return result;
        }

        /// <summary>
        /// <see cref="IApiConnector.FindLinkedEntity{T}"/>
        /// </summary>
        public T FindLinkedEntity<T>(Entity obj, int id)
        {
            var url = obj.GetRelativeUrl(_con) + "/" + obj.Id + "/" + _restResources.GetResource<T>() + "/" + id;
            var response = CallServer(url);
            if (response == null || !_successStatusCodes.Contains(response.StatusCode))
            {
                throw new Exception("Penneo: Internal problem encountered");
            }
            return CreateObject<T>(response.Content);
        }

        public T GetAsset<T>(Entity obj, string assetName)
        {
            var url = obj.GetRelativeUrl(_con) + "/" + obj.Id + "/" + assetName;
            var response = CallServer(url);
            if (response == null || string.IsNullOrEmpty(response.Content) || !_successStatusCodes.Contains(response.StatusCode))
            {
                return default(T);
            }
            var json = response.Content;
            return JsonConvert.DeserializeObject<T>(json);
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
            var url = obj.GetRelativeUrl(_con) + "/" + obj.Id + "/" + assetName;
            var response = CallServer(url);
            var result = JsonConvert.DeserializeObject<string[]>(response.Content);
            return result[0];
        }

        /// <summary>
        /// <see cref="IApiConnector.GetStringListAsset"/>
        /// </summary>
        public IEnumerable<string> GetStringListAsset(Entity obj, string assetName)
        {
            var url = obj.GetRelativeUrl(_con) + "/" + obj.Id + "/" + assetName;
            var response = CallServer(url);
            var result = JsonConvert.DeserializeObject<string[]>(response.Content);
            return result;
        }

        /// <summary>
        /// <see cref="IApiConnector.FindBy{T}"/>
        /// </summary>
        public bool FindBy<T>(Dictionary<string, object> query, out IEnumerable<T> objects, out IRestResponse response, int? page = null, int? perPage = null)
            where T : Entity
        {
            var resource = _restResources.GetResource<T>();

            Dictionary<string, Dictionary<string, object>> options = null;
            if (query != null && query.Count > 0)
            {
                options = new Dictionary<string, Dictionary<string, object>>();
                options["query"] = query;
            }
            response = CallServer(resource, null, Method.GET, options, page: page, perPage: perPage);
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
            return PerformComplexAction(obj, Method.PATCH, actionName, null);
        }

        public ServerResult PerformComplexAction(
            Entity obj,
            Method method,
            string action,
            Dictionary<string, object> data
        )
        {
            var result = new ServerResult();
            var url = obj.GetRelativeUrl(_con) + "/" + obj.Id + "/" + action;
            var response = CallServer(url, data, method);
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
            if (string.IsNullOrEmpty(_endpoint))
            {
                _endpoint = "https://sandbox.penneo.com/api/v3";
            }

            // We dropped support for the old endpoints to try to keep more people on the same version.
            if (_endpoint.EndsWith("api/v1") || _endpoint.EndsWith("api/v2"))
            {
                throw new InvalidOperationException(
                $"Trying to use the {_endpoint} API endpoint. Please use /v3 or above."
                );
            }

            _client = new RestClient(_endpoint);

            _headers = _headers ?? new Dictionary<string, string>();
            _headers["Content-type"] = "application/json";

            if (!string.IsNullOrEmpty(_user))
            {
                _headers["penneo-api-user"] = _user;
            }

            if (_authType == AuthType.WSSE)
            {
                _client.Authenticator = new WSSEAuthenticator(_key, _secret);
            }
            else
            {
                throw new NotSupportedException("Unknown authentication type " + _authType);
            }
            LatestEntityServerResults = new Dictionary<Guid, ServerResult>();
        }

        public void ChangeKeySecret(string key, string secret)
        {
            _key = key;
            _secret = secret;
            _client.Authenticator = new WSSEAuthenticator(_key, _secret);
        }

        /*
        /// <summary>
        /// Sets a factory for creating a connector.
        /// </summary>
        public static void SetFactory(Func<IApiConnector> factory)
        {
            _factory = factory;

            //Null instance if a new factory is provided
            ResetInstance();
        }*/

        /// <summary>
        /// Sets whether to use automatic proxy settings from Internet Explorer
        /// </summary>
        public void SetUseProxySettingsFromInternetExplorer(bool use)
        {
            _useAutomaticProxy = use;
        }

        /// <summary>
        /// Prepare a rest request
        /// </summary>
        internal RestRequest PrepareRequest(string url, Dictionary<string, object> data = null, Method method = Method.GET, Dictionary<string, Dictionary<string, object>> options = null, int? page = null, int? perPage = null)
        {
            var request = new RestRequest(url, method);
            if (_headers != null)
            {
                foreach (var h in _headers)
                {
                    request.AddHeader(h.Key, h.Value);
                }
            }

            if (page.HasValue)
            {
                request.AddHeader("x-paginate", "true");
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

            if (perPage.HasValue)
            {
                if (perPage <= 0)
                {
                    throw new NotSupportedException("PerPage must be greater than zero");
                }
                request.AddParameter("per_page", perPage);
            }
            if (page.HasValue)
            {
                if (page <= 0)
                {
                    throw new NotSupportedException("Page must be greater than zero");
                }
                request.AddParameter("page", page);
            }

            if (data != null)
            {
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(data);
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
                        _con.Log("Proxy URL: " + proxy, LogSeverity.Information);
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
        public IRestResponse CallServer(string url, Dictionary<string, object> data = null, Method method = Method.GET, Dictionary<string, Dictionary<string, object>> options = null, string customMethod = null, int? page = null, int? perPage = null)
        {
            SetProxy();
            try
            {
                var request = PrepareRequest(url, data, method, options, page, perPage);
                LogRequest(request, url, customMethod ?? method.ToString());

                IRestResponse response;
                if (string.IsNullOrEmpty(customMethod))
                {
                    response = _client.Execute(request);
                }
                else
                {
                    response = _client.ExecuteAsGet(request, customMethod);
                }
                LogResponse(response, url, customMethod ?? method.ToString());

                _lastResponse = response;
                _wasLastResponseError = !_successStatusCodes.Contains(_lastResponse.StatusCode);
                return response;
            }
            catch (Exception ex)
            {
                _con.Log(ex.ToString(), LogSeverity.Fatal);
                throw;
            }
        }

        private void LogRequest(IRestRequest request, string url, string method)
        {
            _con.Log(string.Format("HTTP REQUEST {0}: {1}{2} ", method, _client.BaseUrl.ToString(), url), LogSeverity.Trace);
            foreach (var p in request.Parameters)
            {
                _con.Log(string.Format("{0} - {1}: {2}", p.Type, p.Name, p.Value), LogSeverity.Trace);
            }
        }

        private void LogResponse(IRestResponse response, string url, string method)
        {
            _con.Log(string.Format("HTTP RESPONSE {0}: {1}{2} ({3} {4}) ", method, _client.BaseUrl.ToString(), url, (int)response.StatusCode, response.StatusCode), LogSeverity.Trace);
            if (!string.IsNullOrEmpty(response.Content))
            {
                _con.Log(string.Format("Content:  {0}", response.Content), LogSeverity.Trace);
            }
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                _con.Log(string.Format("Content:  {0}", response.ErrorMessage), LogSeverity.Trace);
            }
            foreach (var p in response.Headers)
            {
                _con.Log(string.Format("{0} - {1}: {2}", p.Type, p.Name, p.Value), LogSeverity.Trace);
            }
        }

        /// <summary>
        /// Create objects from a json string
        /// </summary>
        private IEnumerable<T> CreateObjects<T>(string json)
            where T: Entity
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
