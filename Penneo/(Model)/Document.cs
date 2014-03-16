using System;
using System.Collections.Generic;
using System.IO;

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
        public int Status { get; internal set; }
        public string PdfFile { get; set; }
        public CaseFile CaseFile { get; internal set; }
        public string Type { get; internal set; }
        public string Options { get; set; }

        public void MakeSignable()
        {
            Type = TYPE_SIGNABLE;
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
            return GetLinkedEntities<SignatureLine>();
        }

        public SignatureLine FindSignatureLine(int id)
        {
            return FindLinkedEntity<SignatureLine>(id);
        }
    }
}