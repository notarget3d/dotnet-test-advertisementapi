using AdvertisementApi.Core.Interfaces;

namespace AdvertisementApi.Core
{
    public sealed class AdvertisementModelProvider : IAdvertisementModelProvider
    {
        private readonly object _lock = new object();
        private AdvertisementModel? _model;
        public bool initialized { get; private set; }

        public void SetModel(AdvertisementModel model)
        {
            lock (_lock)
            {
                initialized = true;
                _model = model;
            }
        }

        public string[] FindPlatforms(string locations)
        {
            if (_model == null)
            {
                return [];
            }

            return _model.Find(locations);
        }
    }
}
