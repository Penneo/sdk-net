using System;
using System.Threading.Tasks;

namespace Penneo
{
    /// <inheritdoc />
    public abstract class EntityWithIntId: Entity
    {
        /// <summary>
        /// The Id of the entity
        /// </summary>
        public int? Id;

        /// <inheritdoc />
        internal override void SetIdFromServer(Entity source)
        {
            if (source is EntityWithIntId typedSrc)
            {
                Id = typedSrc.Id;
            }
        }

        internal override void SetId(object rawId)
        {
            Id = (int?)rawId;
        }
        
        public override bool IsNew
        {
            get { return !Id.HasValue; }
        }

        internal override string GetIdAsString()
        {
            return Id.ToString();
        }
        
        /// <summary>
        /// Delete the entity from the storage
        /// </summary>
        public override async Task DeleteAsync(PenneoConnector con)
        {
            con.Log("Deleting " + GetType().Name + " (" + (IsNew ? GetIdAsString() : "new") + ")", LogSeverity.Information);
            if (!await con.ApiConnector.DeleteObjectAsync(this).ConfigureAwait(false))
            {
                throw new Exception("Penneo: Could not delete the " + GetType().Name);
            }
            Id = null;
        }
        
        /// <summary>
        /// Return a string for debugging with both type and id
        /// </summary>
        public override string ToString()
        {
            return $"{GetType().Name} Id: {Id}";
        }
    }
}