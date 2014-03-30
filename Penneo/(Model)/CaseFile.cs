using System;
using System.Collections.Generic;
using System.Linq;

namespace Penneo
{
    public class CaseFile : Entity
    {
        private const string ACTION_SEND = "send";
        
        public CaseFile()
        {
            MetaData = null;
        }

        public CaseFile(string title)
            : this()
        {
            Title = title;
        }

        public string Title { get; set; }
        public string MetaData { get; set; }
        public int Status { get; internal set; }
        public DateTime Created { get; internal set; }
        public int SignIteration { get; internal set; }

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

        public ObjectStatus GetStatus()
        {
            return (ObjectStatus)Status;
        }

        public bool Send()
        {
            return PerformAction(ACTION_SEND);
        }
    }
}