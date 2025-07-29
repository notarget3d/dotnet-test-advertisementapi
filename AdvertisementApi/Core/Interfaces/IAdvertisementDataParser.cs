namespace AdvertisementApi.Core.Interfaces
{
    public interface IAdvertisementDataParser
    {
        public bool TryParse(byte[] data, out string message, out List<AdvertisementData> dataList);
        public bool TryParse(string data, out string message, out List<AdvertisementData> dataList);
    }
}
