namespace AdvertisementApi.Core.Interfaces
{
    public interface IAdvertisementModelProvider
    {
        void SetModel(AdvertisementModel model);
        string[] FindPlatforms(string locations);
        bool initialized { get; }
    }
}
