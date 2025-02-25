using System;
using System.Threading.Tasks;

namespace Penneo
{
    /// <inheritdoc />
    public abstract class EntityWithStringId : Entity
    {
        /// <summary>
        /// Uuid of the entity
        /// </summary>
        public string Id;

        internal override void SetIdFromServer(Entity source)
        {
            if (source is WebhookSubscription ws)
            {
                Id = ws.Id;
            }
        }

        internal override void SetId(object rawId)
        {
            Id = (string)rawId;
        }

        public override bool IsNew => string.IsNullOrEmpty(Id);

        internal override string GetIdAsString() => Id;

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