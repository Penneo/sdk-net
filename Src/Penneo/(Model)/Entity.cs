using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Penneo.Connector;
using Penneo.Mapping;
using RestSharp;

namespace Penneo
{
    /// <summary>
    /// Base class for all Penneo business entities
    /// </summary>
    public abstract class Entity
    {
        protected Entity()
        {
            InternalIdentifier = Guid.NewGuid();
        }

        /// <summary>
        /// The relative url (rest resource) of the entity type
        /// </summary>
        internal virtual string GetRelativeUrl(PenneoConnector con)
        {
            return con.ServiceLocator.GetInstance<RestResources>().GetResource(GetType(), Parent);
        }

        /// <summary>
        /// The entity type mapping
        /// </summary>
        internal IMapping GetMapping(PenneoConnector con)
        {
            return con.ServiceLocator.GetInstance<Mappings>().GetMapping(GetType());
        }

        /// <summary>
        /// The parent of the entity (if any)
        /// </summary>
        internal virtual Entity Parent
        {
            get { return null; }
        }

        /// <summary>
        /// Internal guid of the entity
        /// </summary>
        internal Guid InternalIdentifier { get; set; }

        /// <summary>
        /// The Id of the entity
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Denotes if the entity is new (i.e. still unknown to Penneo)
        /// </summary>
        public bool IsNew
        {
            get { return !Id.HasValue; }
        }

        /// <summary>
        /// Get request data from the entity as a property->value dictionary
        /// </summary>
        public Dictionary<string, object> GetRequestData(PenneoConnector con)
        {
            var values = IsNew ? GetMapping(con).GetCreateValues(this) : GetMapping(con).GetUpdateValues(this);
            return values;
        }

        /// <summary>
        /// Persist the entity to the storage
        /// </summary>
        public async Task<bool> PersistAsync(PenneoConnector con)
        {
            con.Log((IsNew ? "Creating" : "Updating") + " " + GetType().Name + " (" + (Id.HasValue ? Id.ToString() : "new") + ")", LogSeverity.Information);
            var success = await con.ApiConnector.WriteObjectAsync(this).ConfigureAwait(false);
            if (!success)
            {
                con.Log((IsNew ? "Creating" : "Updating") + " " + GetType().Name + " (" + (Id.HasValue ? Id.ToString() : "new") + ") failed", LogSeverity.Information);
            }
            return success;
        }

        /// <summary>
        /// Delete the entity from the storage
        /// </summary>
        public async Task DeleteAsync(PenneoConnector con)
        {
            con.Log("Deleting " + GetType().Name + " (" + (Id.HasValue ? Id.ToString() : "new") + ")", LogSeverity.Information);
            if (!await con.ApiConnector.DeleteObjectAsync(this).ConfigureAwait(false))
            {
                throw new Exception("Penneo: Could not delete the " + GetType().Name);
            }
            Id = null;
        }

        /// <summary>
        /// Get the latest server result for the entity
        /// </summary>
        public ServerResult GetLatestServerResult(PenneoConnector con)
        {
            return con.ApiConnector.GetLatestEntityServerResult(this);
        }

        /// <summary>
        /// Link this entity with the given child in the storage
        /// </summary>
        protected Task<bool> LinkEntityAsync(PenneoConnector con, Entity child)
        {
            con.Log("Linking " +
                    GetType().Name + " (" + (Id.HasValue ? Id.ToString() : "new") + ") TO " +
                    child.GetType().Name + " (" + (child.Id.HasValue ? Id.ToString() : "new") + ")", LogSeverity.Information);
            return con.ApiConnector.LinkEntityAsync(this, child);
        }

        /// <summary>
        /// Unlink this entity with the given child in the storage
        /// </summary>
        protected Task<bool> UnlinkEntity(PenneoConnector con, Entity child)
        {
            con.Log("Unlinking " +
                    GetType().Name + " (" + (Id.HasValue ? Id.ToString() : "new") + ") TO " +
                    child.GetType().Name + " (" + (child.Id.HasValue ? Id.ToString() : "new") + ")", LogSeverity.Information);
            return con.ApiConnector.UnlinkEntityAsync(this, child);
        }

        /// <summary>
        /// Get all entities linked with this entity in the storage
        /// </summary>
        protected Task<QueryResult<T>> GetLinkedEntitiesAsync<T>(PenneoConnector con, string url = null)
            where T: Entity
        {
            return con.ApiConnector.GetLinkedEntitiesAsync<T>(this, url);
        }

        /// <summary>
        /// Get entity linked with this entity based on url
        /// </summary>
        protected Task<QuerySingleObjectResult<T>> GetLinkedEntityAsync<T>(PenneoConnector con, string url = null)
            where T: Entity
        {
            return con.ApiConnector.GetLinkedEntityAsync<T>(this, url);
        }

        /// <summary>
        /// Find a specific linked entity
        /// </summary>
        protected Task<T> FindLinkedEntityAsync<T>(PenneoConnector con, int id)
        {
            return con.ApiConnector.FindLinkedEntityAsync<T>(this, id);
        }

        /// <summary>
        /// Get file assets with the given name for this entity
        /// </summary>
        protected Task<byte[]> GetFileAssetsAsync(PenneoConnector con, string assetName)
        {
            return con.ApiConnector.GetFileAssetsAsync(this, assetName);
        }

        /// <summary>
        /// Get text assets with the given name for this entity
        /// </summary>
        protected Task<string> GetTextAssetsAsync(PenneoConnector con, string assetName)
        {
            return con.ApiConnector.GetTextAssetsAsync(this, assetName);
        }

        protected Task<T> GetAssetAsync<T>(PenneoConnector con, string assetName)
        {
            return con.ApiConnector.GetAssetAsync<T>(this, assetName);
        }

        /// <summary>
        /// Get list of string asset with the given name for this entity
        /// </summary>
        protected Task<IEnumerable<string>> GetStringListAssetAsync(PenneoConnector con, string assetName)
        {
            return con.ApiConnector.GetStringListAssetAsync(this, assetName);
        }

        /// <summary>
        /// Perform the given action on this entity
        /// </summary>
        protected Task<ServerResult> PerformActionAsync(PenneoConnector con, string action)
        {
            return con.ApiConnector.PerformAction(this, action);
        }

        protected Task<ServerResult> PerformComplexActionAsync(
            PenneoConnector con,
            Method method,
            string action,
            Dictionary<string, object> data
        )
        {
            return con.ApiConnector.PerformComplexActionAsync(this, method, action, data);
        }

        /// <summary>
        /// Do optional initialization when reading the entity from a source
        /// </summary>
        internal virtual void ReadInit()
        {
        }

        /// <summary>
        /// Return a string for debugging with both type and id
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0} Id: {1}", GetType().Name, Id);
        }
    }
}

