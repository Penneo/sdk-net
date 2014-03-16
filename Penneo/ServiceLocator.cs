using System.Collections.Generic;

namespace Penneo
{
    internal class ServiceLocator
    {
        private static ServiceLocator _instance;

        private Dictionary<string, object> _objects;

        protected ServiceLocator()
        {
            _objects = new Dictionary<string, object>();
        }

        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceLocator();
                }
                return _instance;
            }
        }

        public void RegisterInstance(string name, object o)
        {
            _objects.Add(name, o);
        }

        public void RegisterInstance<T>(object o)
        {
            _objects.Add(typeof(T).Name, o);
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
