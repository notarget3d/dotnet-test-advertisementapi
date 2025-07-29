namespace AdvertisementApi.Core.Interfaces
{
    public interface IAdvertisementModelFactory
    {
        AdvertisementModel Create(List<AdvertisementData> dataRoot);
    }
}
