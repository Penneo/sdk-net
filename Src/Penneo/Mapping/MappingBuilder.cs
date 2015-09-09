using System;
using System.Linq.Expressions;

namespace Penneo.Mapping
{
    /// <summary>
    /// Fluent builder for mappings
    /// </summary>    
    internal class MappingBuilder<T>
        where T : Entity
    {
        private readonly Mappings _container;
        private readonly Mapping<T> _mapping;
        private MethodProperties<T> _currentMethodProperties;

        public MappingBuilder(Mappings container = null)
        {
            _container = container ?? ServiceLocator.Instance.GetInstance<Mappings>();
            _mapping = new Mapping<T>();
        }

        /// <summary>
        /// Starts a build for the given method
        /// </summary>
        private MappingBuilder<T> ForMethod(string method)
        {
            _currentMethodProperties = new MethodProperties<T>();
            _mapping.AddMapping(method, _currentMethodProperties);
            return this;
        }

        /// <summary>
        /// Starts a build for method 'create'
        /// </summary>
        public MappingBuilder<T> ForCreate()
        {
            return ForMethod("create");
        }

        /// <summary>
        /// Starts a build for method 'update'
        /// </summary>
        public MappingBuilder<T> ForUpdate()
        {
            return ForMethod("update");
        }

        /// <summary>
        /// Maps a given property (with an optional alias) for the current method
        /// </summary>
        public MappingBuilder<T> Map(Expression<Func<T, object>> property, string alias = null, Func<object, object> convert = null)
        {
            _currentMethodProperties.AddProperty(property, alias, convert);
            return this;
        }

        /// <summary>
        /// Maps a file property (with an optional alias) for the current method
        /// </summary>
        public MappingBuilder<T> MapFile(Expression<Func<T, string>> property, string alias = null)
        {
            _currentMethodProperties.AddFileProperty(property, alias);
            return this;
        }

        public MappingBuilder<T> MapBase64(Expression<Func<T, byte[]>> property, string alias = null)
        {
            _currentMethodProperties.AddBase64Property(property, alias);
            return this;
        }

        /// <summary>
        /// Gets the mapping instance from the build
        /// </summary>
        /// <returns></returns>
        public IMapping GetMapping()
        {
            return _mapping;
        }

        /// <summary>
        /// Create and register the mapping instance in the Mappings container
        /// </summary>
        public void Create()
        {
            _container.AddMapping(GetMapping());
        }
    }
}