namespace AdvertisementApi.Core
{
    public class AdvertisementConfig
    {
        public long fileSizeLimit { get; init; } = 100_000;
        public string fileCacheName { get; init; } = "data";
    }
}
