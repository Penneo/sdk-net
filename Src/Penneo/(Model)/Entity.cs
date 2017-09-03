using System;
using System.Collections.Generic;
using Penneo.Connector;
using Penneo.Mapping;

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
        internal string GetRelativeUrl(PenneoConnector con)
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
        public bool Persist(PenneoConnector con)
        {
            con.Log((IsNew ? "Creating" : "Updating") + " " + GetType().Name + " (" + (Id.HasValue ? Id.ToString() : "new") + ")", LogSeverity.Information);
            var success = con.ApiConnector.WriteObject(this);
            if (!success)
            {
                con.Log((IsNew ? "Creating" : "Updating") + " " + GetType().Name + " (" + (Id.HasValue ? Id.ToString() : "new") + ") failed", LogSeverity.Information);
            }
            return success;
        }

        /// <summary>
        /// Delete the entity from the storage
        /// </summary>
        public void Delete(PenneoConnector con)
        {
            con.Log("Deleting " + GetType().Name + " (" + (Id.HasValue ? Id.ToString() : "new") + ")", LogSeverity.Information);
            if (!con.ApiConnector.DeleteObject(this))
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
        protected bool LinkEntity(PenneoConnector con, Entity child)
        {
            con.Log("Linking " +
                      GetType().Name + " (" + (Id.HasValue ? Id.ToString() : "new") + ") TO " +
                      child.GetType().Name + " (" + (child.Id.HasValue ? Id.ToString() : "new") + ")", LogSeverity.Information);
            return con.ApiConnector.LinkEntity(this, child);
        }

        /// <summary>
        /// Unlink this entity with the given child in the storage
        /// </summary>
        protected bool UnlinkEntity(PenneoConnector con, Entity child)
        {
            con.Log("Unlinking " +
                      GetType().Name + " (" + (Id.HasValue ? Id.ToString() : "new") + ") TO " +
                      child.GetType().Name + " (" + (child.Id.HasValue ? Id.ToString() : "new") + ")", LogSeverity.Information);
            return con.ApiConnector.UnlinkEntity(this, child);
        }

        /// <summary>
        /// Get all entities linked with this entity in the storage
        /// </summary>
        protected QueryResult<T> GetLinkedEntities<T>(PenneoConnector con, string url = null)
            where T: Entity
        {
            return con.ApiConnector.GetLinkedEntities<T>(this, url);
        }

        /// <summary>
        /// Find a specific linked entity
        /// </summary>
        protected T FindLinkedEntity<T>(PenneoConnector con, int id)
        {
            return con.ApiConnector.FindLinkedEntity<T>(this, id);
        }

        /// <summary>
        /// Get file assets with the given name for this entity
        /// </summary>
        protected byte[] GetFileAssets(PenneoConnector con, string assetName)
        {
            return con.ApiConnector.GetFileAssets(this, assetName);
        }

        /// <summary>
        /// Get text assets with the given name for this entity
        /// </summary>
        protected string GetTextAssets(PenneoConnector con, string assetName)
        {
            return con.ApiConnector.GetTextAssets(this, assetName);
        }

        protected T GetAsset<T>(PenneoConnector con, string assetName)
        {
            return con.ApiConnector.GetAsset<T>(this, assetName);
        }

        /// <summary>
        /// Get list of string asset with the given name for this entity
        /// </summary>
        protected IEnumerable<string> GetStringListAsset(PenneoConnector con, string assetName)
        {
            return con.ApiConnector.GetStringListAsset(this, assetName);
        }

        /// <summary>
        /// Perform the given action on this entity
        /// </summary>
        protected ServerResult PerformAction(PenneoConnector con, string action)
        {
            return con.ApiConnector.PerformAction(this, action);
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