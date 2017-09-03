using System.Collections.Generic;

namespace Penneo
{
    public class ServiceLocator
    {
        private readonly Dictionary<string, object> _objects;

        public ServiceLocator()
        {
            _objects = new Dictionary<string, object>();
        }

        public void RegisterInstance(string name, object o)
        {
            _objects[name] = o;
        }

        public void RegisterInstance<T>(object o)
        {
            _objects[typeof (T).Name] = o;
        }

        public T GetInstance<T>(string name)
        {
            object val;
            if (!_objects.TryGetValue(name, out val))
            {
                throw new KeyNotFoundException(name + " not found");
            }
            return (T) val;
        }

        public T GetInstance<T>()
        {
            var name = typeof (T).Name;
            return GetInstance<T>(name);
        }
    }
}