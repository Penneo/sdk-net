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
        bool WriteObject(Entity obj);

        /// <summary>
        /// Delete the given entity from the backend
        /// </summary>
        bool DeleteObject(Entity obj);

        /// <summary>
        /// Read an object from the backend
        /// </summary>
        T ReadObject<T>(Entity parent, int? id, out RestResponse response)
            where T : Entity;

        /// <summary>
        /// Read an object from the backend
        /// If relative url is specifically provided, then that overrides general resource setup
        /// </summary>
        T ReadObject<T>(Entity parent, int? id, string relativeUrl, out RestResponse response)
            where T : Entity;

        /// <summary>
        /// Link two objects on the backend.
        /// </summary>
        bool LinkEntity(Entity parent, Entity child);

        /// <summary>
        /// Unlink two objects on the backend.
        /// </summary>
        bool UnlinkEntity(Entity parent, Entity child);

        /// <summary>
        /// Gets all entities linked with obj from the backend.
        /// </summary>
        QueryResult<T> GetLinkedEntities<T>(Entity obj, string url = null)
            where T: Entity;

        /// <summary>
        /// Gets single entity linked with obj from the backend (url must be provided).
        /// </summary>
        QuerySingleObjectResult<T> GetLinkedEntity<T>(Entity obj, string url = null)
            where T : Entity;

        /// <summary>
        /// Find a specific linked entity
        /// </summary>
        T FindLinkedEntity<T>(Entity obj, int id);

        /// <summary>
        /// Get file assets for the given obj and asset name
        /// </summary>
        byte[] GetFileAssets(Entity obj, string assetName);

        /// <summary>
        /// Get text assets for the given obj and asset name
        /// </summary>        
        string GetTextAssets(Entity obj, string assetName);

        /// <summary>
        /// Get an asset object
        /// </summary>
        T GetAsset<T>(Entity obj, string assetName);

        /// <summary>
        /// Get list of string asset for the given obj and asset name
        /// </summary>
        IEnumerable<string> GetStringListAsset(Entity obj, string assetName);

        /// <summary>
        /// Find objects on the backend based on query parameters
        /// </summary>
        bool FindBy<T>(Dictionary<string, object> query, out IEnumerable<T> objects, out RestResponse response, int? page = null, int? perPage = null)
            where T : Entity;

        /// <summary>
        /// Performs the named action on the backend for the given object
        /// </summary>
        ServerResult PerformAction(Entity obj, string actionName);

        /// <summary>
        /// Performs the named action on the backend for the given object
        /// </summary>
        ServerResult PerformComplexAction(Entity obj, Method method, string action, Dictionary<string, object> data);

        /// <summary>
        /// Get the latest server result for a given entity
        /// </summary>
        ServerResult GetLatestEntityServerResult(Entity entity);

        /// <summary>
        /// Custom call to the server
        /// </summary>
        Task<RestResponse> CallServer(string url, Dictionary<string, object> data = null, Method method = Method.Get,
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