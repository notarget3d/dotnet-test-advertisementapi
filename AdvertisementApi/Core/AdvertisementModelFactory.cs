using System.Collections.Frozen;
using System.Text;

using AdvertisementApi.Core.Interfaces;

namespace AdvertisementApi.Core
{
    public sealed class AdvertisementModelFactory : IAdvertisementModelFactory
    {
        public AdvertisementModel Create(List<AdvertisementData> dataRoot)
        {
            Dictionary<string, List<string>> baked = new(dataRoot.Count);

            foreach (var data in dataRoot)
            {
                string[] locations = data.locations;
                string[] platforms = Find(locations, dataRoot);

                string locationCombined = Combine(locations);

                if (baked.TryGetValue(locationCombined, out List<string>? valuePlatforms))
                {
                    if (valuePlatforms == null)
                    {
                        baked[locationCombined] = new List<string>(platforms);
                        continue;
                    }

                    valuePlatforms.AddRange(platforms);
                }
                else
                {
                    baked.Add(locationCombined, new(platforms));
                }
            }

            return new(FrozenDictionary.ToFrozenDictionary(baked.Select(x =>
                new KeyValuePair<string, string[]>(x.Key, x.Value.Distinct().ToArray()))));
        }

        private string Combine(string[] locations)
        {
            StringBuilder sb = new StringBuilder(128);

            foreach (var location in locations)
            {
                sb.Append('/');
                sb.Append(location);
            }

            return sb.ToString();
        }

        private string[] Find(string[] locations, List<AdvertisementData> dataRoot)
        {
            if (locations.Length == 0)
            {
                return [];
            }

            List<string> results = new(locations.Length);
            List<AdvertisementData> canditates = dataRoot.Where(x => x.locations.Length <= locations.Length).ToList();

            for (int i = 0; i < canditates.Count; i++)
            {
                if (!canditates[i].CompareLocations(locations))
                {
                    continue;
                }

                results.Add(canditates[i].platform);
            }

            return results.ToArray();
        }
    }
}
