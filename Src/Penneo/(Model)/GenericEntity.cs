using System;
using System.Threading.Tasks;

namespace Penneo
{
    /// <inheritdoc />
    public abstract class GenericEntity<T> : Entity
    {
        /// <summary>
        /// The Id of the entity
        /// </summary>
        public T Id { get; set; }

        /// <inheritdoc />
        internal override void SetIdFromServer(Entity source)
        {
            if (source is GenericEntity<T> entity)
            {
                Id = entity.Id;
            }
        }

        internal override void SetId(object rawId)
        {
            if (rawId == null)
            {
                Id = default;
                return;
            }

            if (typeof(T) == typeof(int?))
            {
                if (rawId is int intVal)
                {
                    Id = (T)(object)(int?)intVal;
                    return;
                }
            }

            if (typeof(T) == typeof(string))
            {
                if (rawId is string strVal)
                {
                    Id = (T)(object)strVal;
                }
            }
        }

        public override bool IsNew
        {
            get
            {
                if (typeof(T) == typeof(int?))
                    return Id == null;
                if (typeof(T) == typeof(string))
                    return string.IsNullOrEmpty(Id as string);
                throw new NotSupportedException("Type not supported");
            }
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
            con.Log(
                "Deleting " + GetType().Name + " (" + (IsNew ? GetIdAsString() : "new") + ")",
                LogSeverity.Information
            );
            if (!await con.ApiConnector.DeleteObjectAsync(this).ConfigureAwait(false))
            {
                throw new Exception("Penneo: Could not delete the " + GetType().Name);
            }
            Id = default;
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

