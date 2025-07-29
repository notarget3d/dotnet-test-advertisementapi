using System.Collections.Frozen;
using System.Text;

namespace AdvertisementApi.Core
{
    public sealed class AdvertisementModel
    {
        private readonly FrozenDictionary<string, string[]> _data;

        public AdvertisementModel(FrozenDictionary<string, string[]> data)
        {
            _data = data;
        }

        public string[] Find(string locations)
        {
            if (_data.TryGetValue(locations, out string[]? platforms))
            {
                if (platforms == null)
                {
                    return [];
                }

                return platforms;
            }

            return [];
        }

        public bool IsValid()
        {
            return _data.Count > 0;
        }
    }
}
