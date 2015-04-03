﻿using System;
using System.Collections.Generic;
using Penneo.Connector;
using Penneo.Mapping;
using Penneo.Util;

namespace Penneo
{
    /// <summary>
    /// Connection to the Penneo backend. Must be initialized before operations can start.
    /// </summary>
    public class PenneoConnector
    {
        public static bool IsInitialized;
        internal static AuthType AuthenticationType;
        internal static string Key;
        internal static string Secret;
        internal static string Endpoint;
        internal static string User;
        internal static Dictionary<string, string> Headers;

        /// <summary>
        /// Checks if the last Http response was an error
        /// </summary>
        public static bool WasLastResponseError
        {
            get { return ApiConnector.Instance.WasLastResponseError; }
        }

        /// <summary>
        /// Gets the content of the last response
        /// </summary>
        public static string LastResponseContent
        {
            get { return ApiConnector.Instance.LastResponseContent; }
        }

        /// <summary>
        /// Initialize the connection to Penneo.
        /// </summary>        
        public static void Initialize(string key, string secret, string endpoint = null, string user = null, Dictionary<string, string> headers = null, AuthType authType = AuthType.WSSE)
        {
            Key = key;
            Secret = secret;
            AuthenticationType = authType;
            Endpoint = endpoint;
            User = user;
            Headers = headers;

            InitializeRestResources();
            InitializeMappings();

            //Reset the api-connector if it was created earlier (to reset any cached authentication)
            ApiConnector.ResetInstance();

            IsInitialized = true;
        }

        /// <summary>
        /// Use proxy settings from Internet Explorer
        /// </summary>
        public static void SetUseProxySettingsFromInternetExplorer(bool use)
        {
            ApiConnector.SetUseProxySettingsFromInternetExplorer(use);
        }

        /// <summary>
        /// Set a logger to get log information from the Penneo connection.
        /// </summary>
        public static void SetLogger(ILogger logger)
        {
            Log.SetLogger(logger);
        }

        /// <summary>
        /// Resets all stored values on this class, when they have been consumed and are no longer needed
        /// </summary>
        internal static void Reset()
        {
            Key = null;
            Secret = null;
            Endpoint = null;
            User = null;
            Headers = null;
        }

        private static void InitializeRestResources()
        {
            var r = new RestResources();

            r.Add<CaseFile>("casefiles");
            r.Add<Document>("documents");
            r.Add<SignatureLine>("signaturelines");
            r.Add<Signer>("signers");
            r.Add<SigningRequest>("signingrequests");
            r.Add<Validation>("validations");
            r.Add<Folder>("folders");
            r.Add<SignerType>("signertypes");
            r.Add<DocumentType>("documenttype");
            r.Add<CaseFileTemplate>("casefiletype");
            r.Add<LogEntry>("log");

            ServiceLocator.Instance.RegisterInstance<RestResources>(r);
        }

        /// <summary>
        /// Initialize entity mappings
        /// </summary>
        private static void InitializeMappings()
        {
            var mappings = new Mappings();
            ServiceLocator.Instance.RegisterInstance<Mappings>(mappings);

            new MappingBuilder<CaseFile>(mappings)
                .ForCreate()
                .Map(x => x.Title)
                .Map(x => x.MetaData)
                .Map(x => x.SendAt, convert: x => TimeUtil.ToUnixTime((DateTime) x))
                .Map(x => x.ExpireAt, convert: x => TimeUtil.ToUnixTime((DateTime) x))
                .Map(x => x.VisibilityMode)
                .Map(x => x.CaseFileTemplate.Id, "caseFileTypeId")
                .ForUpdate()
                .Map(x => x.Title)
                .Map(x => x.MetaData)
                .Map(x => x.CaseFileTemplate.Id, "caseFileTypeId")
                .Create();

            new MappingBuilder<Document>(mappings)
                .ForCreate()
                .Map(x => x.Title)
                .Map(x => x.CaseFile.Id, "CaseFileId")
                .Map(x => x.MetaData)
                .Map(x => x.Type)
                .Map(x => x.Options)
                .MapFile(x => x.PdfFile)
                .Map(x => x.DocumentType.Id, "documentTypeId")
                .ForUpdate()
                .Map(x => x.Title)
                .Map(x => x.MetaData)
                .Map(x => x.Options)
                .Create();

            new MappingBuilder<SignatureLine>(mappings)
                .ForCreate()
                .Map(x => x.Role)
                .Map(x => x.Conditions)
                .Map(x => x.SignOrder)
                .ForUpdate()
                .Map(x => x.Role)
                .Map(x => x.Conditions)
                .Map(x => x.SignOrder)
                .Create();

            new MappingBuilder<Signer>(mappings)
                .ForCreate()
                .Map(x => x.Name)
                .Map(x => x.SocialSecurityNumber, "SocialSecurityNumberPlain")
                .Map(x => x.VATIdentificationNumber, "vatin")
                .Map(x => x.OnBehalfOf)
                .ForUpdate()
                .Map(x => x.Name)
                .Map(x => x.SocialSecurityNumber, "SocialSecurityNumberPlain")
                .Map(x => x.VATIdentificationNumber, "vatin")
                .Map(x => x.OnBehalfOf)
                .Create();

            new MappingBuilder<SigningRequest>(mappings)
                .ForUpdate()
                .Map(x => x.Email)
                .Map(x => x.EmailText)
                .Map(x => x.EmailSubject)
                .Map(x => x.SuccessUrl)
                .Map(x => x.FailUrl)
                .Map(x => x.ReminderInterval)
                .Map(x => x.AccessControl)
                .Create();

            new MappingBuilder<Validation>(mappings)
                .ForCreate()
                .Map(x => x.Name)
                .Map(x => x.Title)
                .Map(x => x.Email)
                .Map(x => x.EmailSubject)
                .Map(x => x.EmailText)
                .Map(x => x.SuccessUrl)
                .Map(x => x.CustomText)
                .Map(x => x.ReminderInterval)
                .ForUpdate()
                .Map(x => x.Name)
                .Map(x => x.Title)
                .Map(x => x.Email)
                .Map(x => x.EmailSubject)
                .Map(x => x.EmailText)
                .Map(x => x.SuccessUrl)
                .Map(x => x.CustomText)
                .Map(x => x.ReminderInterval)
                .Create();

            new MappingBuilder<Folder>(mappings)
                .ForCreate()
                .Map(x => x.Title)
                .ForUpdate()
                .Map(x => x.Title)
                .Create();
        }
    }
}