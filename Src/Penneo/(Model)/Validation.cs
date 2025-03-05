﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Penneo
{
    public class Validation : EntityWithIntId
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

        public Task<byte[]> GetPdfAsync(PenneoConnector con)
        {
            return GetFileAssetsAsync(con, ASSET_PDF);
        }

        public async Task SavePdfAsync(PenneoConnector con, string path)
        {
            var data = await GetPdfAsync(con).ConfigureAwait(false);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllBytes(path, data);
        }

        public Task<string> GetLinkAsync(PenneoConnector con)
        {
            return GetTextAssetsAsync(con, ASSET_LINK);
        }

        public Task<ValidationContents> GetContents(PenneoConnector con)
        {
            return GetAssetAsync<ValidationContents>(con, ASSET_CONTENTS);
        }

        public async Task<bool> SendAsync(PenneoConnector con)
        {
            return (await PerformActionAsync(con, ACTION_SEND).ConfigureAwait(false)).Success;
        }

        public async Task<IEnumerable<LogEntry>> GetEventLog(PenneoConnector con)
        {
            return (await GetLinkedEntitiesAsync<LogEntry>(con).ConfigureAwait(false)).Objects;
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
