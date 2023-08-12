using System.Collections.Generic;
using Penneo.Connector;

namespace Penneo
{
    /// <summary>
    /// Connection to the Penneo backend. Must be initialized before operations can start.
    /// </summary>
    public class PenneoConnector
    {
        internal Dictionary<string, string> Headers;
        internal PenneoSetup _setup;
        internal ServiceLocator _serviceLocator;
        internal IApiConnector _api;
        private IPenneoLogger _logger = new NullLogger();

        public bool IsInitialized { get; set; }

        /// <summary>
        /// Checks if the last Http response was an error
        /// </summary>
        public bool WasLastResponseError
        {
            get { return _api.WasLastResponseError; }
        }

        /// <summary>
        /// Gets the content of the last response
        /// </summary>
        public string LastResponseContent
        {
            get { return _api.LastResponseContent; }
        }

        /// <summary>
        /// Get the internal service locator
        /// </summary>
        internal ServiceLocator ServiceLocator
        {
            get { return _serviceLocator; }
        }

        /// <summary>
        /// Get the internal setup
        /// </summary>
        internal PenneoSetup Setup
        {
            get { return _setup; }
        }

        /// <summary>
        /// Get the internal api connector
        /// </summary>
        internal IApiConnector ApiConnector
        {
            get { return _api; }
            set { _api = value; }
        }

        /// <summary>
        /// Get/Set the logger used by the Penneo sdk
        /// </summary>
        public IPenneoLogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        /// <summary>
        /// Initialize the connection to Penneo.
        /// </summary>
        public PenneoConnector(string key, string secret, string endpoint = null, string user = null, Dictionary<string, string> headers = null, AuthType authType = AuthType.WSSE)
        {
            _serviceLocator = new ServiceLocator();
            _setup = new PenneoSetup(_serviceLocator);
            _setup.InitializeRestResources();
            _setup.InitializeMappings();
            _setup.InitializePostProcessors();

            _api = new ApiConnector(this, _setup.GetRestResources(), endpoint, headers, user, key, secret, authType);

            IsInitialized = true;
        }

        internal void Init()
        {
            _serviceLocator = new ServiceLocator();
            _setup = new PenneoSetup(_serviceLocator);
            _setup.InitializeRestResources();
            _setup.InitializeMappings();
            _setup.InitializePostProcessors();
        }

        /// <summary>
        /// Change the key/secret
        /// </summary>
        public void ChangeKeySecret(string key, string secret)
        {
            _api.ChangeKeySecret(key, secret);
        }

        /// <summary>
        /// Use proxy settings from Internet Explorer
        /// </summary>
        public void SetUseProxySettingsFromInternetExplorer(bool use)
        {
            _api.SetUseProxySettingsFromInternetExplorer(use);
        }

        /// <summary>
        /// Log a message
        /// </summary>
        public void Log(string message, LogSeverity severity)
        {
            Logger.Log(message, severity);
        }
    }
}
