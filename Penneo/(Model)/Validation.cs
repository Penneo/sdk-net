using System.IO;

namespace Penneo
{
    public class Validation : Entity
    {
        private const string ASSET_PDF = "pdf";
        private const string ASSET_LINK = "link";

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
        public string Email { get; set; }
        public string EmailText { get; set; }
        public int Status { get; internal set; }

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

        public string GetLink()
        {
            return GetTextAssets(ASSET_LINK);
        }
    }
}