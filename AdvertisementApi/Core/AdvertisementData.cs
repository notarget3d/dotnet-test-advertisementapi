namespace AdvertisementApi.Core
{
    public sealed class AdvertisementData
    {
        public readonly string platform;
        public readonly string[] locations;

        public AdvertisementData(string plat, string[] locs)
        {
            platform = plat;
            locations = locs;
        }

        public bool CompareLocations(string[] locationsToCompare)
        {
            int last = Math.Min(locations.Length, locationsToCompare.Length);

            for (int i = 0; i < last; i++)
            {
                if (locations[i] != locationsToCompare[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
