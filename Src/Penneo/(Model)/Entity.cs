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
        /// <summary>
        /// The relative url (rest resource) of the entity type
        /// </summary>
        internal string RelativeUrl
        {
            get { return ServiceLocator.Instance.GetInstance<RestResources>().GetResource(GetType(), Parent); }
        }

        /// <summary>
        /// The entity type mapping
        /// </summary>
        internal IMapping Mapping
        {
            get { return ServiceLocator.Instance.GetInstance<Mappings>().GetMapping(GetType()); }
        }

        /// <summary>
        /// The parent of the entity (if any)
        /// </summary>
        internal virtual Entity Parent
        {
            get { return null; }
        }

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
        public Dictionary<string, object> GetRequestData()
        {
            var values = IsNew ? Mapping.GetCreateValues(this) : Mapping.GetUpdateValues(this);
            return values;
        }

        /// <summary>
        /// Persist the entity to the storage
        /// </summary>
        public void Persist()
        {
            Log.Write((IsNew ? "Creating" : "Updating") + " " + GetType().Name + " (" + (Id.HasValue ? Id.ToString() : "new") + ")", LogSeverity.Information);
            if (!ApiConnector.Instance.WriteObject(this))
            {
                throw new Exception("Penneo: Could not persist the " + GetType().Name);
            }
        }

        /// <summary>
        /// Delete the entity from the storage
        /// </summary>
        public void Delete()
        {
            Log.Write("Deleting " + GetType().Name + " (" + (Id.HasValue ? Id.ToString() : "new") + ")", LogSeverity.Information);
            if (!ApiConnector.Instance.DeleteObject(this))
            {
                throw new Exception("Penneo: Could not delete the " + GetType().Name);
            }
            Id = null;
        }

        /// <summary>
        /// Link this entity with the given child in the storage
        /// </summary>
        protected bool LinkEntity(Entity child)
        {
            Log.Write("Linking " +
                      GetType().Name + " (" + (Id.HasValue ? Id.ToString() : "new") + ") TO " +
                      child.GetType().Name + " (" + (child.Id.HasValue ? Id.ToString() : "new") + ")", LogSeverity.Information);
            if (!ApiConnector.Instance.LinkEntity(this, child))
            {
                throw new Exception("Penneo: Could not link the " + GetType().Name + " and the " + child.GetType().Name);
            }
            return true;
        }

        /// <summary>
        /// Unlink this entity with the given child in the storage
        /// </summary>
        protected bool UnlinkEntity(Entity child)
        {
            Log.Write("Unlinking " +
                      GetType().Name + " (" + (Id.HasValue ? Id.ToString() : "new") + ") TO " +
                      child.GetType().Name + " (" + (child.Id.HasValue ? Id.ToString() : "new") + ")", LogSeverity.Information);
            if (!ApiConnector.Instance.UnlinkEntity(this, child))
            {
                throw new Exception("Penneo: Could not unlink the " + GetType().Name + " and the " + child.GetType().Name);
            }
            return true;
        }

        /// <summary>
        /// Get all entities linked with this entity in the storage
        /// </summary>
        protected IEnumerable<T> GetLinkedEntities<T>(string url = null)
        {
            return ApiConnector.Instance.GetLinkedEntities<T>(this, url);
        }

        /// <summary>
        /// Find a specific linked entity
        /// </summary>
        protected T FindLinkedEntity<T>(int id)
        {
            return ApiConnector.Instance.FindLinkedEntity<T>(this, id);
        }

        /// <summary>
        /// Get file assets with the given name for this entity
        /// </summary>
        protected byte[] GetFileAssets(string assetName)
        {
            return ApiConnector.Instance.GetFileAssets(this, assetName);
        }

        /// <summary>
        /// Get text assets with the given name for this entity
        /// </summary>
        protected string GetTextAssets(string assetName)
        {
            return ApiConnector.Instance.GetTextAssets(this, assetName);
        }

        /// <summary>
        /// Get list of string asset with the given name for this entity
        /// </summary>
        protected IEnumerable<string> GetStringListAsset(string assetName)
        {
            return ApiConnector.Instance.GetStringListAsset(this, assetName);
        }

        /// <summary>
        /// Perform the given action on this entity
        /// </summary>
        protected bool PerformAction(string action)
        {
            return ApiConnector.Instance.PerformAction(this, action);
        }

        /// <summary>
        /// Do optional initialization when reading the entity from a source
        /// </summary>
        internal virtual void ReadInit()
        {
        }
    }
}