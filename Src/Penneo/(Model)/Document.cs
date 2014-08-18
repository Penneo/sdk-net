using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Penneo
{
    public class Document : Entity
    {
        private const string TYPE_ATTACHMENT = "attachment";
        private const string TYPE_SIGNABLE = "signable";
        private const string ASSET_PDF = "pdf";

        public Document()
        {
            MetaData = null;
            Type = TYPE_ATTACHMENT;
        }

        public Document(CaseFile cf)
            : this()
        {
            CaseFile = cf;
        }

        public Document(CaseFile cf, string title, string pdfFilePath)
            : this(cf)
        {
            CaseFile = cf;
            Title = title;
            PdfFile = pdfFilePath;
        }

        public string DocumentId { get; set; }
        public string Title { get; set; }
        public string MetaData { get; set; }
        public DateTime Created { get; internal set; }
        public DateTime Modified { get; internal set; }
        public DateTime Completed { get; internal set; }
        public int? Status { get; internal set; }
        public string PdfFile { get; set; }        
        public string Type { get; internal set; }
        public string Options { get; set; }
        public DocumentType DocumentType { get; set; }

        private CaseFile _caseFile;
        public CaseFile CaseFile 
        {
            get 
            {
                if (_caseFile == null)
                {
                    _caseFile = GetLinkedEntities<CaseFile>().FirstOrDefault();
                }                
                return _caseFile;                               
            }
            internal set { _caseFile = value; } 
        }

        public void MakeSignable()
        {
            Type = TYPE_SIGNABLE;
        }

        public DocumentStatus GetStatus()
        {
            if (!Status.HasValue)
            {
                return DocumentStatus.New;
            }
            return (DocumentStatus)Status;
        }

        public byte[] GetPdf()
        {
            return GetFileAssets(ASSET_PDF);
        }

        public void SavePdf(string path)
        {
            var data = GetPdf();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllBytes(path, data);
        }

        public IEnumerable<SignatureLine> GetSignatureLines()
        {
            var signatureLines = GetLinkedEntities<SignatureLine>();
            foreach(var sl in signatureLines)
            {
                sl.Document = this; 
            }
            return signatureLines;
        }

        public SignatureLine FindSignatureLine(int id)
        {
            var sl = FindLinkedEntity<SignatureLine>(id);
            sl.Document = this;
            return sl;
        }

        public DocumentType GetDocumentType()
        {
            if (Id.HasValue && DocumentType == null)
            {
                var documentTypes = GetLinkedEntities<DocumentType>();
                DocumentType = documentTypes.FirstOrDefault();
            }
            return DocumentType;
        }

        public void SetDocumentType(DocumentType type)
        {
            DocumentType = type;
        }
    }

    public enum DocumentStatus
    {
        New = 0,
        Pending = 1,
        Rejected = 2,
        Deleted = 3,
        Signed = 4,
        Completed = 5
    }
}