using System;
using System.Collections.Generic;
using Penneo.Connector;
using Penneo.Mapping;
using Penneo.Util;

namespace Penneo
{
    public class PenneoSetup
    {
        private readonly ServiceLocator _serviceLocator;
        private Dictionary<Type, Func<object, object>> _postProcessors = new Dictionary<Type, Func<object, object>>();

        public PenneoSetup(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public virtual void InitializeRestResources()
        {
            var r = new RestResources();

            r.Add<CaseFile>("casefiles");
            r.Add<Document>("documents");
            r.Add<SignatureLine>("signaturelines");
            r.Add<Signer>("signers");
            r.Add<SignerTypeMap>("signertypes");
            r.Add<SigningRequest>("signingrequests");
            r.Add<Validation>("validations");
            r.Add<Folder>("folders");
            r.Add<SignerType>("signertypes");
            r.Add<DocumentType>("documenttype");
            r.Add<CaseFileTemplate>("casefiletype");
            r.Add<LogEntry>("log");
            r.Add<CopyRecipient>("recipients");
            r.Add<MessageTemplate>("casefile/message/templates");
            r.Add<User>("users");
            r.Add<Customer>("customers");
            r.Add<WebhookSubscription>("webhook/subscriptions");
            r.Add<Contact>("contacts");

            _serviceLocator.RegisterInstance<RestResources>(r);
        }

        internal RestResources GetRestResources()
        {
            return _serviceLocator.GetInstance<RestResources>();
        }

        /// <summary>
        /// Initialize entity mappings
        /// </summary>
        public virtual void InitializeMappings()
        {
            var mappings = new Mappings();
            _serviceLocator.RegisterInstance<Mappings>(mappings);

            mappings.AddMapping(
              new MappingBuilder<CaseFile>()
                  .ForCreate()
                  .Map(x => x.Title)
                  .Map(x => x.Language)
                  .Map(x => x.MetaData)
                  .Map(x => x.SendAt, convert: x => TimeUtil.ToUnixTime((DateTime) x))
                  .Map(x => x.Activated, convert: x => TimeUtil.ToUnixTime((DateTime) x))
                  .Map(x => x.ExpireAt, convert: x => TimeUtil.ToUnixTime((DateTime) x))
                  .Map(x => x.VisibilityMode)
                  .Map(x => x.SensitiveData)
                  .Map(x => x.DisableNotificationsOwner)
                  .Map(x => x.SignOnMeeting)
                  .Map(x => x.Reference)
                  .Map(x => x.DisableEmailAttachments)
                  .Map(x => x.CaseFileTemplate.Id, "caseFileTypeId")
                  .ForUpdate()
                  .Map(x => x.Title)
                  .Map(x => x.Language)
                  .Map(x => x.MetaData)
                  .Map(x => x.SendAt, convert: x => TimeUtil.ToUnixTime((DateTime) x))
                  .Map(x => x.Activated, convert: x => TimeUtil.ToUnixTime((DateTime) x))
                  .Map(x => x.ExpireAt, convert: x => TimeUtil.ToUnixTime((DateTime) x))
                  .Map(x => x.CaseFileTemplate.Id, "caseFileTypeId")
                  .Map(x => x.VisibilityMode)
                  .Map(x => x.SensitiveData)
                  .Map(x => x.DisableNotificationsOwner)
                  .Map(x => x.Reference)
                  .Map(x => x.DisableEmailAttachments)
                  .Map(x => x.SignOnMeeting)
                  .Create()
            );

            mappings.AddMapping(
              new MappingBuilder<Document>()
                  .ForCreate()
                  .Map(x => x.Title)
                  .Map(x => x.CaseFile.Id, "CaseFileId")
                  .Map(x => x.MetaData)
                  .Map(x => x.DocumentOrder, "documentOrder")
                  .Map(x => x.SignType, "type")
                  .Map(x => x.Opts)
                  .MapBase64(x => x.PdfRaw, "PdfFile")
                  .Map(x => x.DocumentType.Id, "documentTypeId")
                  .ForUpdate()
                  .Map(x => x.Title)
                  .Map(x => x.MetaData)
                  .Map(x => x.DocumentOrder, "documentOrder")
                  .Map(x => x.DocumentType.Id, "documentTypeId")
                  .Map(x => x.Opts)
                  .Create()
            );

            mappings.AddMapping(
              new MappingBuilder<SignatureLine>()
                  .ForCreate()
                  .Map(x => x.Role)
                  .Map(x => x.Conditions)
                  .Map(x => x.SignOrder)
                  .Map(x => x.ActiveAt, convert: x => TimeUtil.ToUnixTime((DateTime) x))
                  .Map(x => x.ActivatedAt, convert: x => TimeUtil.ToUnixTime((DateTime) x))
                  .Map(x => x.ExpireAt,convert: x => TimeUtil.ToUnixTime((DateTime) x))
                  .ForUpdate()
                  .Map(x => x.ActiveAt, convert: x => TimeUtil.ToUnixTime((DateTime) x))
                  .Map(x => x.ExpireAt, convert: x => TimeUtil.ToUnixTime((DateTime) x))
                  .Create()
            );

            mappings.AddMapping(
              new MappingBuilder<Signer>()
                  .ForCreate()
                  .Map(x => x.Name)
                  .Map(x => x.SocialSecurityNumber, "SocialSecurityNumberPlain")
                  .Map(x => x.SsnType)
                  .Map(x => x.VATIdentificationNumber, "vatin")
                  .Map(x => x.OnBehalfOf)
                  .Map(x => x.StoreAsContact)
                  .ForUpdate()
                  .Map(x => x.Name)
                  .Map(x => x.SsnType)
                  .Map(x => x.SocialSecurityNumber, "SocialSecurityNumberPlain")
                  .Map(x => x.VATIdentificationNumber, "vatin")
                  .Map(x => x.OnBehalfOf)
                  .Map(x => x.StoreAsContact)
                  .Create()
            );

            mappings.AddMapping(
                new MappingBuilder<SigningRequest>()
                    .ForUpdate()
                    .Map(x => x.Email)
                    .Map(x => x.EmailText)
                    .Map(x => x.EmailSubject)
                    .Map(x => x.ReminderEmailSubject)
                    .Map(x => x.ReminderEmailText)
                    .Map(x => x.CompletedEmailSubject)
                    .Map(x => x.CompletedEmailText)
                    .Map(x => x.EmailFormat)
                    .Map(x => x.SuccessUrl)
                    .Map(x => x.FailUrl)
                    .Map(x => x.ReminderInterval)
                    .Map(x => x.AccessControl)
                    .Map(x => x.EnableInsecureSigning)
                    .Map(x => x.InsecureSigningMethods)
                    .Create()
            );

            mappings.AddMapping(
                new MappingBuilder<Validation>()
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
                    .Create()
            );

            mappings.AddMapping(
                new MappingBuilder<Folder>()
                    .ForCreate()
                    .Map(x => x.Title)
                    .Map(x => x.ParentId, "Parent")
                    .ForUpdate()
                    .Map(x => x.Title)
                    .Map(x => x.ParentId, "Parent")
                    .Create()
            );

            mappings.AddMapping(
                new MappingBuilder<CopyRecipient>()
                    .ForCreate()
                    .Map(x => x.Name)
                    .Map(x => x.Email)
                    .ForUpdate()
                    .Map(x => x.Name)
                    .Map(x => x.Email)
                    .Create()
            );

            mappings.AddMapping(
                new MappingBuilder<MessageTemplate>()
                    .ForCreate()
                    .Map(x => x.Title)
                    .Map(x => x.Subject)
                    .Map(x => x.Message)
                    .ForUpdate()
                    .Map(x => x.Title)
                    .Map(x => x.Subject)
                    .Map(x => x.Message)
                    .Create()
            );

            mappings.AddMapping(
                new MappingBuilder<SignerTypeMap>()
                    .ForCreate()
                    .Map(x => x.SignerTypeId)
                    .Map(x => x.SignerId)
                    .Map(x => x.Role)
                    .Map(x => x.ActiveAt, convert: x => TimeUtil.ToUnixTime((DateTime) x))
                    .Map(x => x.ExpireAt, convert: x => TimeUtil.ToUnixTime((DateTime) x))
                    .Create()
            );

            mappings.AddMapping(
                new MappingBuilder<WebhookSubscription>()
                    .ForCreate()
                    .Map(x => x.Endpoint)
                    .Map(x => x.Topic)
                    .Create()
            );
            mappings.AddMapping(
                new MappingBuilder<Contact>()
                    .ForCreate()
                    .Map(x => x.Name)
                    .Map(x => x.Email)
                    .ForUpdate()
                    .Map(x => x.Name)
                    .Map(x => x.Email)
                    .Create()
            );
        }

        public virtual void InitializePostProcessors()
        {
            _postProcessors[typeof(Folder)] = Folder.QueryPostProcessor;
        }

        public virtual Func<object, object> GetPostProcessor(Type type)
        {
            Func<object, object> processor;
            if (_postProcessors.TryGetValue(type, out processor))
            {
                return processor;
            }
            return null;
        }
    }
}
