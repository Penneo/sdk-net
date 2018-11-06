using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Penneo
{
    public class Validation : Entity
    {
        private const string ASSET_PDF = "pdf";
        private const string ASSET_LINK = "link";
        private const string ASSET_CONTENTS = "contents";
        private const string ACTION_SEND = "send";

        public Validation()
        {
        }

        public Validation(string name)
        {
            Name = name;
        }

        public Validation(string name, string email)
            : this(name)
        {
            Email = email;
        }

        public string Name { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string EmailSubject { get; set; }
        public string EmailText { get; set; }
        public int? Status { get; set; }
        public string SuccessUrl { get; set; }
        public string CustomText { get; set; }
        public int? ReminderInterval { get; set; }

        public ValidationStatus GetStatus()
        {
            if (!Status.HasValue)
            {
                return ValidationStatus.New;
            }
            return (ValidationStatus) Status;
        }

        public byte[] GetPdf(PenneoConnector con)
        {
            return GetFileAssets(con, ASSET_PDF);
        }

        public void SavePdf(PenneoConnector con, string path)
        {
            var data = GetPdf(con);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllBytes(path, data);
        }

        public string GetLink(PenneoConnector con)
        {
            return GetTextAssets(con, ASSET_LINK);
        }

        public ValidationContents GetContents(PenneoConnector con)
        {
            return GetAsset<ValidationContents>(con, ASSET_CONTENTS);
        }

        public bool Send(PenneoConnector con)
        {
            return PerformAction(con, ACTION_SEND).Success;
        }

        public IEnumerable<LogEntry> GetEventLog(PenneoConnector con)
        {
            return GetLinkedEntities<LogEntry>(con).Objects;
        }
    }

    public enum ValidationStatus
    {
        New = 0,
        Pending = 1,
        Undeliverable = 2,
        Deleted = 3,
        Ready = 4,
        Completed = 5
    }
}