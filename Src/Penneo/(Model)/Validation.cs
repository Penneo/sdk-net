using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

        public async Task<byte[]> GetPdfAsync(PenneoConnector con)
        {
            return await GetFileAssetsAsync(con, ASSET_PDF);
        }

        public async Task SavePdfAsync(PenneoConnector con, string path)
        {
            var data = await GetPdfAsync(con);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllBytes(path, data);
        }

        public async Task<string> GetLinkAsync(PenneoConnector con)
        {
            return await GetTextAssetsAsync(con, ASSET_LINK);
        }

        public async Task<ValidationContents> GetContents(PenneoConnector con)
        {
            return await GetAssetAsync<ValidationContents>(con, ASSET_CONTENTS);
        }

        public async Task<bool> SendAsync(PenneoConnector con)
        {
            return (await PerformActionAsync(con, ACTION_SEND)).Success;
        }

        public async Task<IEnumerable<LogEntry>> GetEventLog(PenneoConnector con)
        {
            return (await GetLinkedEntitiesAsync<LogEntry>(con)).Objects;
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
