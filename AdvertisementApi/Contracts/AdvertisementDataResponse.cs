namespace AdvertisementApi.Contracts
{
    // Advertisement platform list
    public record AdvertisementDataResponse(string[] advPlat);
    public record AdvertisementDataUploadResponse(string message);
}
