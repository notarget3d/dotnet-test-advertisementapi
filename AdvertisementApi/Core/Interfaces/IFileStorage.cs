namespace AdvertisementApi.Core.Interfaces
{
    public interface IFileStorage
    {
        void SaveFile(string path, MemoryStream memoryStream);
        void SaveFile(string path, byte[] buffer);
        void SaveFile(MemoryStream memoryStream);
        byte[] ReadFile(string path);
        byte[] ReadFile();
    }
}
