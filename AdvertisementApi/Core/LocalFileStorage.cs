using Microsoft.Extensions.Options;
using AdvertisementApi.Core.Interfaces;

namespace AdvertisementApi.Core
{
    public sealed class LocalFileStorage : IFileStorage
    {
        private readonly string _fileCacheName = "data";

        public LocalFileStorage(IOptions<AdvertisementConfig> config)
        {
            _fileCacheName = config.Value.fileCacheName;
        }

        public void SaveFile(string path, MemoryStream memoryStream)
        {
            SaveFile(path, memoryStream.GetBuffer());
        }

        public void SaveFile(string path, byte[] buffer)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            File.WriteAllBytes(path, buffer);
        }

        public void SaveFile(MemoryStream memoryStream)
        {
            SaveFile(_fileCacheName, memoryStream.GetBuffer());
        }

        public byte[] ReadFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return [];
            }

            try
            {
                return File.ReadAllBytes(path);
            }
            catch
            {
                return [];
            }
        }

        public byte[] ReadFile()
        {
            return ReadFile(_fileCacheName);
        }
    }
}
