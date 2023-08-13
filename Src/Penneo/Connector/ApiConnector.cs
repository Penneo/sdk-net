using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Penneo.Util;
using RestSharp;

namespace Penneo.Connector
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FindByResult<T>
        where T : Entity
    {
        public bool Success { get; set; }
        public IEnumerable<T> Objects { get; set; }
        public RestResponse Response { get; set; }
    }

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
        /// The options for the RestClient
        /// </summary>
        private RestClientOptions _clientOptions;

        /// <summary>
        /// Http headers
        /// </summary>
        private Dictionary<string, string> _headers;

        /// <summary>
        /// The last Http response
        /// </summary>
        private RestResponse _lastResponse;

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

        #region IApiConnector Members
        /// <summary>
        /// <see cref="IApiConnector.WriteObjectAsync"/>
        /// </summary>
        public async Task<bool> WriteObjectAsync(Entity obj)
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
                var response = await CallServerAsync(obj.GetRelativeUrl(_con) + "/" + obj.Id, data, Method.Put).ConfigureAwait(false);
                return ExtractResponse(obj, response, result);
            }
            else
            {
                var response = await CallServerAsync(obj.GetRelativeUrl(_con), data, Method.Post).ConfigureAwait(false);
                var successful = ExtractResponse(obj, response, result);
                if (successful)
                {
                    //Update id given from server
                    var fromServer = (Entity)JsonConvert.DeserializeObject(response.Content, obj.GetType());
                    if (fromServer != null) obj.Id = fromServer.Id;
                }
                return successful;
            }
        }

        private bool ExtractResponse(Entity obj, RestResponse response, ServerResult result)
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
        /// <see cref="IApiConnector.DeleteObjectAsync"/>
        /// </summary>
        public async Task<bool> DeleteObjectAsync(Entity obj)
        {
            var response = await CallServerAsync(obj.GetRelativeUrl(_con) + '/' + obj.Id, null, Method.Delete).ConfigureAwait(false);
            return response != null && (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent);
        }

        public Task<ReadObjectResult<T>> ReadObjectAsync<T>(Entity parent, int? id) where T : Entity
        {
            return ReadObjectAsync<T>(parent, id, null);
        }

        public async Task<ReadObjectResult<T>> ReadObjectAsync<T>(Entity parent, int? id, string relativeUrl) where T : Entity
        {
            var url = !string.IsNullOrEmpty(relativeUrl) ? relativeUrl : _restResources.GetResource(typeof(T), parent);
            if (id.HasValue)
            {
                url += "/" + id;
            }
            var response = await CallServerAsync(url).ConfigureAwait(false);
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                return new ReadObjectResult<T> { Response = response, Result = default };
            }
            var obj = JsonConvert.DeserializeObject<T>(response.Content);
            if (id.HasValue)
            {
                obj.Id = id;
            }
            return new ReadObjectResult<T> { Response = response, Result = obj };
        }


        /// <summary>
        /// <see cref="IApiConnector.LinkEntityAsync"/>
        /// </summary>
        public async Task<bool> LinkEntityAsync(Entity parent, Entity child)
        {
            var url = parent.GetRelativeUrl(_con) + "/" + parent.Id + "/" + _restResources.GetResource(child.GetType()) + "/" + child.Id;
            var response = await CallServerAsync(url, method: Method.Post).ConfigureAwait(false);
            var result = new ServerResult();
            return ExtractResponse(parent, response, result);
        }

        /// <summary>
        /// <see cref="IApiConnector.UnlinkEntityAsync"/>
        /// </summary>
        public async Task<bool> UnlinkEntityAsync(Entity parent, Entity child)
        {
            var url = parent.GetRelativeUrl(_con) + "/" + parent.Id + "/" + _restResources.GetResource(child.GetType()) + "/" + child.Id;
            var response = await CallServerAsync(url, method: Method.Delete).ConfigureAwait(false);
            var result = new ServerResult();
            return ExtractResponse(parent, response, result);
        }

        /// <summary>
        /// <see cref="IApiConnector.GetLinkedEntitiesAsyncAsync{T}"/>
        /// </summary>
        public async Task<QueryResult<T>> GetLinkedEntitiesAsync<T>(Entity obj, string url = null)
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

            var response = await CallServerAsync(actualUrl).ConfigureAwait(false);
            var result = new QueryResult<T>();
            if (ExtractResponse(obj, response, result))
            {
                result.Objects = CreateObjects<T>(response.Content);
            }
            return result;
        }

        /// <summary>
        /// <see cref="IApiConnector.GetLinkedEntityAsyncAsync{T}"/>
        /// </summary>
        public async Task<QuerySingleObjectResult<T>> GetLinkedEntityAsync<T>(Entity obj, string url = null)
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

            var response = await CallServerAsync(actualUrl).ConfigureAwait(false);
            var result = new QuerySingleObjectResult<T>();
            if (ExtractResponse(obj, response, result))
            {
                result.Object = CreateObject<T>(response.Content);
            }
            return result;
        }

        /// <summary>
        /// <see cref="IApiConnector.FindLinkedEntityAsync{T}"/>
        /// </summary>
        public async Task<T> FindLinkedEntityAsync<T>(Entity obj, int id)
        {
            var url = obj.GetRelativeUrl(_con) + "/" + obj.Id + "/" + _restResources.GetResource<T>() + "/" + id;
            var response = await CallServerAsync(url).ConfigureAwait(false);
            if (response == null || !_successStatusCodes.Contains(response.StatusCode))
            {
                throw new Exception("Penneo: Internal problem encountered");
            }
            return CreateObject<T>(response.Content);
        }

        public async Task<T> GetAssetAsync<T>(Entity obj, string assetName)
        {
            var url = obj.GetRelativeUrl(_con) + "/" + obj.Id + "/" + assetName;
            var response = await CallServerAsync(url).ConfigureAwait(false);
            if (response == null || string.IsNullOrEmpty(response.Content) || !_successStatusCodes.Contains(response.StatusCode))
            {
                return default(T);
            }
            var json = response.Content;
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// <see cref="IApiConnector.GetFileAssetsAsync"/>
        /// </summary>
        public async Task<byte[]> GetFileAssetsAsync(Entity obj, string assetName)
        {
            var encoded = await GetTextAssetsAsync(obj, assetName).ConfigureAwait(false);
            return Convert.FromBase64String(encoded);
        }

        /// <summary>
        /// <see cref="IApiConnector.GetTextAssetsAsync"/>
        /// </summary>
        public async Task<string> GetTextAssetsAsync(Entity obj, string assetName)
        {
            var url = obj.GetRelativeUrl(_con) + "/" + obj.Id + "/" + assetName;
            var response = await CallServerAsync(url).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<string[]>(response.Content);
            return result[0];
        }

        /// <summary>
        /// <see cref="IApiConnector.GetStringListAssetAsync"/>
        /// </summary>
        public async Task<IEnumerable<string>> GetStringListAssetAsync(Entity obj, string assetName)
        {
            var url = obj.GetRelativeUrl(_con) + "/" + obj.Id + "/" + assetName;
            var response = await CallServerAsync(url).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<string[]>(response.Content);
            return result;
        }

        /// <summary>
        /// <see cref="IApiConnector.FindByAsync{T}"/>
        /// </summary>
        public async Task<FindByResult<T>> FindByAsync<T>(Dictionary<string, object> query, int? page = null, int? perPage = null)
            where T : Entity
        {
            var resource = _restResources.GetResource<T>();

            Dictionary<string, Dictionary<string, object>> options = null;
            if (query != null && query.Count > 0)
            {
                options = new Dictionary<string, Dictionary<string, object>>();
                options["query"] = query;
            }
            var response = await CallServerAsync(resource, null, Method.Get, options, page: page, perPage: perPage).ConfigureAwait(false);
            if (response == null || !_successStatusCodes.Contains(response.StatusCode))
            {
                return new FindByResult<T> { Success = false };
            }

            var objects = CreateObjects<T>(response.Content);
            return new FindByResult<T> { Success = true, Objects = objects, Response = response };
        }

        /// <summary>
        /// <see cref="IApiConnector.PerformAction"/>
        /// </summary>
        public Task<ServerResult> PerformAction(Entity obj, string actionName)
        {
            return PerformComplexActionAsync(obj, Method.Patch, actionName, null);
        }

        public async Task<ServerResult> PerformComplexActionAsync(Entity obj,
            Method method,
            string action,
            Dictionary<string, object> data)
        {
            var result = new ServerResult();
            var url = obj.GetRelativeUrl(_con) + "/" + obj.Id + "/" + action;
            var response = await CallServerAsync(url, data, method).ConfigureAwait(false);
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

            _clientOptions = new RestClientOptions(baseUrl: _endpoint)
            {
                UserAgent = "Penneo%2Fsdk-net%40" + Info.Version
            };

            _client = new RestClient(_clientOptions);

            _headers = _headers ?? new Dictionary<string, string>();

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
        internal RestRequest PrepareRequest(string url, Dictionary<string, object> data = null, Method method = Method.Get, Dictionary<string, Dictionary<string, object>> options = null, int? page = null, int? perPage = null)
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
                request.AddParameter("per_page", (int) perPage);
            }
            if (page.HasValue)
            {
                if (page <= 0)
                {
                    throw new NotSupportedException("Page must be greater than zero");
                }
                request.AddParameter("page", (int) page);
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
                request.AddParameter(StringUtil.FirstCharacterToLower(entry.Key), entry.Value.ToString());
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
                        _clientOptions.Proxy = new WebProxy(proxy);
                    }
                }
            }
            else
            {
                _clientOptions.Proxy = null;
            }
        }

        /// <summary>
        /// Calls the Penneo server with a rest request
        /// </summary>
        public async Task<RestResponse> CallServerAsync(string url, Dictionary<string, object> data = null,
            Method method = Method.Get, Dictionary<string, Dictionary<string, object>> options = null, int? page = null,
            int? perPage = null)
        {
            SetProxy();
            try
            {
                var request = PrepareRequest(url, data, method, options, page, perPage);
                LogRequest(request, url, method.ToString());

                var response = await _client.ExecuteAsync(request).ConfigureAwait(false);

                LogResponse(response, url, method.ToString());

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

        private void LogRequest(RestRequest request, string url, string method)
        {
            _con.Log(string.Format("HTTP REQUEST {0}: {1}{2} ", method, _clientOptions.BaseUrl.ToString(), url), LogSeverity.Trace);
            foreach (var p in request.Parameters)
            {
                _con.Log(string.Format("{0} - {1}: {2}", p.Type, p.Name, p.Value), LogSeverity.Trace);
            }
        }

        private void LogResponse(RestResponse response, string url, string method)
        {
            _con.Log(string.Format("HTTP RESPONSE {0}: {1}{2} ({3} {4}) ", method, _clientOptions.BaseUrl.ToString(), url, (int)response.StatusCode, response.StatusCode), LogSeverity.Trace);
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

    public class ReadObjectResult<T> where T : Entity
    {
        public RestResponse Response { get; set; }
        public T Result { get; set; }
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
