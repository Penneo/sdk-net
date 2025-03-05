using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;

namespace Penneo.Connector
{
    /// <summary>
    /// Connection with the Penneo backend
    /// </summary>
    internal interface IApiConnector
    {
        /// <summary>
        /// Did the last response received contain an error
        /// </summary>
        bool WasLastResponseError { get; }

        /// <summary>
        /// Get the content of the last response
        /// </summary>
        string LastResponseContent { get; }

        /// <summary>
        /// Write the given entity to the backend
        /// </summary>
        Task<bool> WriteObjectAsync(Entity obj);

        /// <summary>
        /// Delete the given entity from the backend
        /// </summary>
        Task<bool> DeleteObjectAsync(Entity obj);

        /// <summary>
        /// Read an object from the backend
        /// </summary>
        Task<ReadObjectResult<T>> ReadObjectAsync<T>(Entity parent, int? id)
            where T : Entity;

        /// <summary>
        /// Read an object from the backend
        /// </summary>
        Task<ReadObjectResult<T>> ReadObjectAsync<T>(Entity parent, string id)
            where T : Entity;


        /// <summary>
        /// Read an object from the backend
        /// If relative url is specifically provided, then that overrides general resource setup
        /// </summary>
        Task<ReadObjectResult<T>> ReadObjectAsync<T>(Entity parent, int? id, string relativeUrl)
            where T : Entity;

        /// <summary>
        /// Link two objects on the backend.
        /// </summary>
        Task<bool> LinkEntityAsync(Entity parent, Entity child);

        /// <summary>
        /// Unlink two objects on the backend.
        /// </summary>
        Task<bool> UnlinkEntityAsync(Entity parent, Entity child);

        /// <summary>
        /// Gets all entities linked with obj from the backend.
        /// </summary>
        Task<QueryResult<T>> GetLinkedEntitiesAsync<T>(Entity obj, string url = null)
            where T : Entity;

        /// <summary>
        /// Gets single entity linked with obj from the backend (url must be provided).
        /// </summary>
        Task<QuerySingleObjectResult<T>> GetLinkedEntityAsync<T>(Entity obj, string url = null)
            where T : Entity;

        /// <summary>
        /// Find a specific linked entity
        /// </summary>
        Task<T> FindLinkedEntityAsync<T>(Entity obj, int id);

        /// <summary>
        /// Get file assets for the given obj and asset name
        /// </summary>
        Task<byte[]> GetFileAssetsAsync(Entity obj, string assetName);

        /// <summary>
        /// Get text assets for the given obj and asset name
        /// </summary>        
        Task<string> GetTextAssetsAsync(Entity obj, string assetName);

        /// <summary>
        /// Get an asset object
        /// </summary>
        Task<T> GetAssetAsync<T>(Entity obj, string assetName);

        /// <summary>
        /// Get list of string asset for the given obj and asset name
        /// </summary>
        Task<IEnumerable<string>> GetStringListAssetAsync(Entity obj, string assetName);

        /// <summary>
        /// Find objects on the backend based on query parameters
        /// </summary>
        Task<FindByResult<T>> FindByAsync<T>(Dictionary<string, object> query, int? page = null, int? perPage = null)
            where T : Entity;

        /// <summary>
        /// Performs the named action on the backend for the given object
        /// </summary>
        Task<ServerResult> PerformAction(Entity obj, string actionName);

        /// <summary>
        /// Performs the named action on the backend for the given object
        /// </summary>
        Task<ServerResult> PerformComplexActionAsync(Entity obj, Method method, string action,
            Dictionary<string, object> data);

        /// <summary>
        /// Get the latest server result for a given entity
        /// </summary>
        ServerResult GetLatestEntityServerResult(Entity entity);

        /// <summary>
        /// Custom call to the server
        /// </summary>
        Task<RestResponse> CallServerAsync(string url, Dictionary<string, object> data = null, Method method = Method.Get,
            Dictionary<string, Dictionary<string, object>> options = null, int? page = null, int? perPage = null);

        /// <summary>
        /// Change the key and secret on the api connector
        /// </summary>
        void ChangeKeySecret(string key, string secret);

        /// <summary>
        /// Enable/disable using IE proxy settings
        /// </summary>
        void SetUseProxySettingsFromInternetExplorer(bool use);
    }
}