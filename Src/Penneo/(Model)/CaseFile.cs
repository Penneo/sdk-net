using System;
using System.Collections.Generic;
using System.Linq;

namespace Penneo
{
    public class CaseFile : Entity
    {
        private const string ACTION_SEND = "send";
        private const string ACTION_ACTIVATE = "activate";
        
        public CaseFile()
        {
            MetaData = null;
            VisibilityMode = 0;
        }

        public CaseFile(string title)
            : this()
        {
            Title = title;
        }

        public string Title { get; set; }
        public string MetaData { get; set; }
        public int? Status { get; internal set; }
        public DateTime Created { get; internal set; }
        public int SignIteration { get; internal set; }
        public DateTime? SendAt { get; set; }
        public DateTime? ExpireAt { get; set; }
        public int? VisibilityMode { get; set; }

        public IEnumerable<Document> GetDocuments()
        {
            var documents = GetLinkedEntities<Document>().ToList();
            foreach (var doc in documents)
            {
                doc.CaseFile = this;
            }
            return documents;
        }

        public IEnumerable<Signer> GetSigners()
        {
            var signers = GetLinkedEntities<Signer>().ToList();
            foreach (var s in signers)
            {
                s.CaseFile = this;
            }
            return signers;
        }

        public Signer FindSigner(int id)
        {
            var linked = FindLinkedEntity<Signer>(id);
            linked.CaseFile = this;
            return linked;
        }

        public CaseFileStatus GetStatus()
        {
            if (!Status.HasValue)
            {
                return CaseFileStatus.New;
            }
            return (CaseFileStatus)Status;
        }

        public bool Send()
        {
            return PerformAction(ACTION_SEND);
        }

        public bool Activate()
        {
            return PerformAction(ACTION_ACTIVATE);
        }

        public enum CaseFileStatus
        {
            New = 0,
            Pending = 1,
            Rejected = 2,
            Deleted = 3,
            Signed = 4,
            Completed = 5
        }
    }
}